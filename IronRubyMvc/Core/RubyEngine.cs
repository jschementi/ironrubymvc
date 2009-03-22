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
        private readonly Func<string, StreamContentProvider> _contentProviderFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="RubyEngine"/> class.
        /// </summary>
        /// <param name="runtime">The runtime.</param>
        /// <param name="pathProvider">The VPP.</param>
        public RubyEngine(ScriptRuntime runtime, IPathProvider pathProvider)
        {
            Runtime = runtime;
            PathProvider = pathProvider;
            _contentProviderFactory = path => new VirtualPathStreamContentProvider(path);
            Initialize();
        }

        internal RubyEngine(ScriptRuntime runtime, IPathProvider pathProvider, Func<string, StreamContentProvider> contentProviderFactory)
        {
            Runtime = runtime;
            PathProvider = pathProvider;
            _contentProviderFactory = contentProviderFactory;
            Initialize();
        }

        /// <summary>
        /// Gets the runtime.
        /// </summary>
        /// <value>The runtime.</value>
        internal ScriptRuntime Runtime { get; set; }

        /// <summary>
        /// Gets the context.
        /// </summary>
        /// <value>The context.</value>
        internal RubyContext Context { get; private set; }

        /// <summary>
        /// Gets the engine.
        /// </summary>
        /// <value>The engine.</value>
        internal ScriptEngine Engine { get; private set; }

        /// <summary>
        /// Gets the current scope.
        /// </summary>
        /// <value>The current scope.</value>
        internal ScriptScope CurrentScope { get; private set; }

        /// <summary>
        /// Gets the operations.
        /// </summary>
        /// <value>The operations.</value>
        internal ObjectOperations Operations { get; private set; }

        /// <summary>
        /// Gets the path provider.
        /// </summary>
        /// <value>The path provider.</value>
        internal IPathProvider PathProvider { get; private set; }

        #region IRubyEngine Members

        /// <summary>
        /// Loads the controller.
        /// </summary>
        /// <param name="requestContext">The request context.</param>
        /// <param name="controllerName">Name of the controller.</param>
        /// <returns></returns>
        public RubyController LoadController(RequestContext requestContext, string controllerName)
        {
            var controllerFilePath = GetControllerFilePath(controllerName);
            var controllerClassName = GetControllerClassName(controllerName);

            // Remove the current controller from the classes cache so it's completely renewed.
            if (Runtime.Globals.ContainsVariable(controllerClassName)) Runtime.Globals.RemoveVariable(controllerClassName);

            if (controllerFilePath.IsNullOrBlank())
                return null;


            Engine.CreateScriptSource( _contentProviderFactory(controllerFilePath), null, Encoding.UTF8).Execute(CurrentScope);

            var controllerClass = GetRubyClass(controllerClassName);
            var controller = ConfigureController(controllerClass, requestContext);

            return controller;
        }


        /// <summary>
        /// Configures the controller.
        /// </summary>
        /// <param name="rubyClass">The ruby class.</param>
        /// <param name="requestContext">The request context.</param>
        /// <returns></returns>
        public RubyController ConfigureController(RubyClass rubyClass, RequestContext requestContext)
        {
            var controller = (RubyController) Operations.CreateInstance(rubyClass);
            controller.InternalInitialize(new ControllerConfiguration {Context = requestContext, Engine = this, RubyClass = rubyClass});
            return controller;
        }

        public string GetMethodName(object receiver, string message)
        {
            var methodNames = Operations.GetMemberNames(receiver);
            
            if (methodNames.Contains(message.Pascalize())) return message.Pascalize();
            if (methodNames.Contains(message.Underscore())) return message.Underscore();

            return message;
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
        /// Determines whether the specified controller as the action.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <param name="actionName">Name of the action.</param>
        /// <returns>
        /// 	<c>true</c> if the specified controller has the action; otherwise, <c>false</c>.
        /// </returns>
        public bool HasControllerAction(RubyController controller, string actionName)
        {
            try
            {
                Operations.ContainsMember(controller, actionName.Underscore());
                return true;
            }
            catch (NullReferenceException)
            {
                return false;
            }
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
            controllerClass.EnumerateMethods((_, methodName, __) =>
                                                 {
                                                     names.Add(methodName);
                                                     return false;
                                                 });
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

        public object ExecuteScript(string script)
        {
            return Engine.Execute(script, CurrentScope);
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

        #endregion


        private void Initialize()
        {
            Engine = Ruby.GetEngine(Runtime);
            Context = Ruby.GetExecutionContext(Engine);
            CurrentScope = Engine.CreateScope();
            Operations = Engine.CreateOperations();
            LoadAssemblies(typeof (object), typeof (Uri), typeof (HttpResponseBase), typeof(MembershipCreateStatus), typeof (RouteTable), typeof (Controller), typeof (RubyController));
            AddLoadPaths();
            DefineReadOnlyGlobalVariable(Constants.SCRIPT_RUNTIME_VARIABLE, Engine);
            RequireControllerFile();
        }

        private void RequireControllerFile()
        {
            RequireRubyFile(Constants.RUBYCONTROLLER_FILE, ReaderType.AssemblyResource);
        }

        /// <summary>
        /// Gets the name of the controller class.
        /// </summary>
        /// <param name="controllerName">Name of the controller.</param>
        /// <returns></returns>
        internal static string GetControllerClassName(string controllerName)
        {
            return (controllerName.EndsWith("Controller", StringComparison.OrdinalIgnoreCase)
                        ? controllerName
                        : Constants.CONTROLLERCLASS_FORMAT.FormattedWith(controllerName)).Pascalize();
        }

        internal string GetControllerFilePath(string controllerName)
        {
            var fileName = Constants.CONTROLLER_PASCAL_PATH_FORMAT.FormattedWith(controllerName.Pascalize());
            if (PathProvider.FileExists(fileName))
                return fileName;

            fileName = Constants.CONTROLLER_UNDERSCORE_PATH_FORMAT.FormattedWith(controllerName.Underscore());

            return PathProvider.FileExists(fileName) ? fileName : string.Empty;
        }

        /// <summary>
        /// Requires the ruby file.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="readerType">Type of the reader.</param>
        internal void RequireRubyFile(string path, ReaderType readerType)
        {
            Engine.CreateScriptSource(readerType == ReaderType.File
                                          ? _contentProviderFactory(path)
                                          : new AssemblyStreamContentProvider(path, typeof (IRubyEngine).Assembly), null, Encoding.ASCII).Execute();
        }

        /// <summary>
        /// Sets the model and controllers path.
        /// </summary>
        public void AddLoadPaths()
        {
            var controllersDir = Path.Combine(PathProvider.ApplicationPhysicalPath, Constants.CONTROLLERS);
            var modelsDir = Path.Combine(PathProvider.ApplicationPhysicalPath, Constants.MODELS);
            var filtersDir = Path.Combine(PathProvider.ApplicationPhysicalPath, Constants.FILTERS);
            Context.Loader.SetLoadPaths(new[] {controllersDir, modelsDir, filtersDir});
        }


        /// <summary>
        /// Initializes the iron ruby MVC.
        /// </summary>
        /// <param name="pathProvider">The Path provider.</param>
        /// <param name="routesPath">The routes path.</param>
        public static RubyEngine InitializeIronRubyMvc(IPathProvider pathProvider, string routesPath)
        {
            return InitializeIronRubyMvc(pathProvider, routesPath, path => new VirtualPathStreamContentProvider(path));
        }

        public static RubyEngine InitializeIronRubyMvc(IPathProvider pathProvider, string routesPath, Func<string, StreamContentProvider> contentProviderFactory)
        {
            var engine = InitializeIronRuby(pathProvider, contentProviderFactory);
            ProcessRubyRoutes(engine, pathProvider, routesPath);
            IntializeMvc(engine);
            return engine;
        }

        private static void IntializeMvc(IRubyEngine engine)
        {
            var factory = new RubyControllerFactory(ControllerBuilder.Current.GetControllerFactory(), engine);
            ControllerBuilder.Current.SetControllerFactory(factory);
            ViewEngines.Engines.Add(new RubyViewEngine());
        }

        private static RubyEngine InitializeIronRuby(IPathProvider vpp, Func<string, StreamContentProvider> contentProviderFactory)
        {
            var rubySetup = Ruby.CreateRubySetup();
            var runtimeSetup = new ScriptRuntimeSetup();
            runtimeSetup.LanguageSetups.Add(rubySetup);
            runtimeSetup.DebugMode = true;

            var runtime = Ruby.CreateRuntime(runtimeSetup);
            return new RubyEngine(runtime, vpp, contentProviderFactory);
        }

        private static void ProcessRubyRoutes(RubyEngine engine, IPathProvider vpp, string routesPath)
        {
            if (!vpp.FileExists(routesPath)) return;
            var routeCollection = new RubyRoutes(RouteTable.Routes);
            engine.DefineReadOnlyGlobalVariable("routes", routeCollection);
            engine.RequireRubyFile(routesPath, ReaderType.File);
        }
    }
}
