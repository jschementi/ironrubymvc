#region Usings

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Text;
using System.Web.Mvc.IronRuby.Controllers;
using System.Web.Mvc.IronRuby.Extensions;
using System.Web.Mvc.IronRuby.ViewEngine;
using System.Web.Routing;
using System.Web.Security;
using IronRuby;
using IronRuby.Builtins;
using IronRuby.Runtime;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;

#endregion

namespace System.Web.Mvc.IronRuby.Core
{
    /// <summary>
    /// A wrapper for ScriptEngine, Runtime and Context
    /// This class handles all the interaction with IronRuby
    /// </summary>
    public class RubyEngine : IRubyEngine
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RubyEngine"/> class.
        /// </summary>
        /// <param name="runtime">The runtime.</param>
        /// <param name="pathProvider">The VPP.</param>
        public RubyEngine(ScriptRuntime runtime, IPathProvider pathProvider)
        {
            Runtime = runtime;
            PathProvider = pathProvider;
            Initialize();
        }

        /// <summary>
        /// Gets the runtime.
        /// </summary>
        /// <value>The runtime.</value>
        private ScriptRuntime Runtime { get; set; }

        /// <summary>
        /// Gets the context.
        /// </summary>
        /// <value>The context.</value>
        internal RubyContext Context { get; set; }

        /// <summary>
        /// Gets the engine.
        /// </summary>
        /// <value>The engine.</value>
        private ScriptEngine Engine { get; set; }

        /// <summary>
        /// Gets the current scope.
        /// </summary>
        /// <value>The current scope.</value>
        private ScriptScope CurrentScope { get; set; }

        /// <summary>
        /// Gets the operations.
        /// </summary>
        /// <value>The operations.</value>
        private ObjectOperations Operations { get; set; }

        /// <summary>
        /// Gets the path provider.
        /// </summary>
        /// <value>The path provider.</value>
        private IPathProvider PathProvider { get; set; }

        #region IRubyEngine Members

        public void RemoveClassFromGlobals(string className)
        {
            // Remove the current controller from the classes cache so it's completely renewed.
            if (Runtime.Globals.ContainsVariable(className)) Runtime.Globals.RemoveVariable(className);
        }

        public T CreateInstance<T>(RubyClass rubyClass)
        {
            return (T) Operations.CreateInstance(rubyClass);
        }

        public void ExecuteInScope(Action<ScriptScope> block)
        {
            var scope = Engine.CreateScope();
            block(scope);
        }

        /// <summary>
        /// Calls the method.
        /// </summary>
        /// <param name="receiver">The receiver.</param>
        /// <param name="message">The message.</param>
        /// <param name="args">The args.</param>
        /// <returns></returns>
        public object CallMethod(object receiver, string message, params object[] args)
        {
            return Operations.InvokeMember(receiver, GetMethodName(receiver, message));
        }

        /// <summary>
        /// Gets a list of method names that are defined on the controller.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <returns></returns>
        public IEnumerable<string> MethodNames(IController controller)
        {
            return Operations.GetMemberNames(controller);
        }

        /// <summary>
        /// Methods the names.
        /// </summary>
        /// <param name="controllerClass">The controller class.</param>
        /// <returns></returns>
        public IEnumerable<string> MethodNames(RubyClass controllerClass)
        {
            var names = new List<string>();
            using (Context.ClassHierarchyLocker())
            {
                controllerClass.EnumerateMethods((_, methodName, __) =>
                                                     {
                                                         names.Add(methodName);
                                                         return false;
                                                     });
            }
            return names;
        }

        /// <summary>
        /// Loads the assembly.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        public void LoadAssembly(Assembly assembly)
        {
            Runtime.LoadAssembly(assembly);
        }

        /// <summary>
        /// Executes the script.
        /// </summary>
        /// <param name="script">The script.</param>
        /// <returns></returns>
        public object ExecuteScript(string script)
        {
            return ExecuteScript(script, CurrentScope);
        }

        /// <summary>
        /// Executes the script.
        /// </summary>
        /// <param name="script">The script.</param>
        /// <param name="scope">The scope.</param>
        /// <returns></returns>
        public object ExecuteScript(string script, ScriptScope scope)
        {
            return Engine.Execute(script, scope ?? CurrentScope);
        }

        public object ExecuteFile(string path, ScriptScope scope, bool throwIfNotExist)
        {
            path.EnsureArgumentNotNull("path");
            scope.EnsureArgumentNotNull("scope");

            if (throwIfNotExist && !PathProvider.FileExists(path)) throw new FileNotFoundException("Can't find the file", path);
            
            if(!PathProvider.FileExists(path)) return null;

            var source = Engine.CreateScriptSourceFromFile(path); 

            return source.Execute(scope);
        }

        public T ExecuteFile<T>(string path)
        {
            return (T) ExecuteFile(path);
        }

        public T ExecuteFile<T>(string path, bool throwIfNotExist)
        {
            return (T)ExecuteFile(path, CurrentScope, throwIfNotExist);
        }


        public T ExecuteScript<T>(string script)
        {
            return (T) ExecuteScript(script);
        }

        public T ExecuteScript<T>(string script, ScriptScope scope)
        {
            return (T) ExecuteScript(script, scope);
        }

        public T ExecuteFile<T>(string path, ScriptScope scope, bool throwIfNotExist)
        {
            return (T) ExecuteFile(path, scope, throwIfNotExist);
        }

        public object ExecuteFile(string path)
        {
            return Engine.ExecuteFile(path, CurrentScope);
        }

        /// <summary>
        /// Defines the read only global variable.
        /// </summary>
        /// <param name="variableName">Name of the variable.</param>
        /// <param name="value">The value.</param>
        public void DefineReadOnlyGlobalVariable(string variableName, object value)
        {
            Context.DefineReadOnlyGlobalVariable(variableName, value);
        }

        /// <summary>
        /// Gets the ruby class.
        /// </summary>
        /// <param name="className">Name of the class.</param>
        /// <returns></returns>
        public RubyClass GetRubyClass(string className)
        {
            var klass = GetGlobalVariable<RubyClass>(className);
            return klass;
        }

        /// <summary>
        /// Gets the global variable.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public T GetGlobalVariable<T>(string name)
        {
            return Runtime.Globals.GetVariable<T>(name);
        }

        /// <summary>
        /// Loads the assemblies.
        /// </summary>
        /// <param name="assemblies">The assemblies.</param>
        public void LoadAssemblies(params Type[] assemblies)
        {
            assemblies.ForEach(type => LoadAssembly(type.Assembly));
        }

        /// <summary>
        /// Requires the ruby file.
        /// </summary>
        /// <param name="path">The path.</param>
        public void RequireRubyFile(string path)
        {
            Engine.RequireRubyFile(path);
        }

        /// <summary>
        /// Requires the ruby file.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="readerType">Type of the reader.</param>
        public void RequireRubyFile(string path, ReaderType readerType)
        {
            Engine.CreateScriptSource(readerType == ReaderType.File
                                          ? (StreamContentProvider)new VirtualPathStreamContentProvider(path, PathProvider)
                                          : new AssemblyStreamContentProvider(path, typeof (IRubyEngine).Assembly), null, Encoding.ASCII).Execute();
        }

        #endregion

        public string GetMethodName(object receiver, string message)
        {
            var methodNames = Operations.GetMemberNames(receiver);

            if (methodNames.Contains(message.Pascalize())) return message.Pascalize();
            if (methodNames.Contains(message.Underscore())) return message.Underscore();

            return message;
        }

        private void Initialize()
        {
            Engine = Ruby.GetEngine(Runtime);
            Context = Ruby.GetExecutionContext(Engine);
            CurrentScope = Engine.CreateScope();
            Operations = Engine.CreateOperations();
            LoadAssemblies(typeof (object), typeof (Uri), typeof (HttpResponseBase), typeof (MembershipCreateStatus), typeof (RouteTable), typeof (Controller), typeof (RubyController));
            AddLoadPaths();
            DefineReadOnlyGlobalVariable(Constants.ScriptRuntimeVariable, Engine);
            RequireControllerFile();
        }

        private void RequireControllerFile()
        {
//            RequireRubyFile(PathProvider.MapPath("~/Controllers/controller.rb"));
            Engine.CreateScriptSource(new AssemblyStreamContentProvider("System.Web.Mvc.IronRuby.Controllers.controller.rb", typeof(IRubyEngine).Assembly), null, Encoding.ASCII).Execute(CurrentScope);
        }

        /// <summary>
        /// Sets the model and controllers path.
        /// </summary>
        private void AddLoadPaths()
        {
            var controllersDir = Path.Combine(PathProvider.ApplicationPhysicalPath, Constants.Controllers);
            var modelsDir = Path.Combine(PathProvider.ApplicationPhysicalPath, Constants.Models);
            var filtersDir = Path.Combine(PathProvider.ApplicationPhysicalPath, Constants.Filters);
            var helpersDir = Path.Combine(PathProvider.ApplicationPhysicalPath, Constants.Helpers);

            Context.Loader.SetLoadPaths(new[] {PathProvider.ApplicationPhysicalPath, controllersDir, modelsDir, filtersDir, helpersDir});
        }


        /// <summary>
        /// Initializes ironruby MVC.
        /// </summary>
        /// <param name="pathProvider">The Path provider.</param>
        /// <param name="routesPath">The routes path.</param>
        public static RubyEngine InitializeIronRubyMvc(IPathProvider pathProvider, string routesPath)
        {
            var engine = InitializeIronRuby(pathProvider);
            ProcessRubyRoutes(engine, pathProvider, routesPath);
            IntializeMvc(pathProvider, engine);
            return engine;
        }

         private static void IntializeMvc(IPathProvider pathProvider, IRubyEngine engine)
        {
            var factory = new RubyControllerFactory(pathProvider, ControllerBuilder.Current.GetControllerFactory(), engine);
            ControllerBuilder.Current.SetControllerFactory(factory);
            ViewEngines.Engines.Add(new RubyViewEngine(engine));
        }

        private static RubyEngine InitializeIronRuby(IPathProvider vpp)
        {
            var rubySetup = Ruby.CreateRubySetup();
            var runtimeSetup = new ScriptRuntimeSetup();
            runtimeSetup.LanguageSetups.Add(rubySetup);
            runtimeSetup.DebugMode = true;
//            runtimeSetup.HostType = typeof (MvcScriptHost);

            var runtime = Ruby.CreateRuntime(runtimeSetup);
            return new RubyEngine(runtime, vpp);
        }

        private static void ProcessRubyRoutes(IRubyEngine engine, IPathProvider vpp, string routesPath)
        {
            if (!vpp.FileExists(routesPath)) return;
            var routeCollection = new RubyRoutes(RouteTable.Routes);
            engine.DefineReadOnlyGlobalVariable("routes", routeCollection);
            engine.RequireRubyFile(routesPath, ReaderType.File);
        }
    }
}