#region Usings

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Routing;
using IronRuby;
using IronRuby.Builtins;
using IronRuby.Runtime;
using IronRubyMvcLibrary.Controllers;
using IronRubyMvcLibrary.Extensions;
using Microsoft.Scripting.Hosting;

#endregion

namespace IronRubyMvcLibrary.Core
{
    /// <summary>
    /// A wrapper for ScriptEngine, Runtime and Context
    /// </summary>
    internal class RubyMediator
    {
//        public RubyMediator() : this(HttpContext.Current.Application.GetScriptRuntime())
//        {
//        }

        public RubyMediator(ScriptRuntime runtime)
        {
            Runtime = runtime;
            Initialize();
        }

        public ScriptRuntime Runtime { get; set; }
        public RubyContext Context { get; private set; }
        public ScriptEngine Engine { get; private set; }
        internal IScriptRunner ScriptRunner { get; set; }
        public ScriptScope CurrentScope { get; private set; }
        public ObjectOperations Operations { get; private set; }

        private void Initialize()
        {
            Engine = Ruby.GetEngine(Runtime);
            Context = Ruby.GetExecutionContext(Engine);
            CurrentScope = Engine.CreateScope();
            Operations = Engine.CreateOperations();
            ScriptRunner = new ScopedScriptRunner(Engine, CurrentScope);
            SetModelAndControllersPath();
            DefineReadOnlyGlobalVariable(Constants.SCRIPT_RUNTIME_VARIABLE, Engine);
            RequireControllerFile();
        }

        private void RequireControllerFile()
        {
            RequireRubyFile(Constants.RUBYCONTROLLER_FILE, ReaderType.AssemblyResource);
        }


        public RubyController LoadController(RequestContext requestContext, string controllerName)
        {
            string controllerFilePath = GetControllerFilePath(controllerName);

            if (controllerFilePath.IsNullOrBlank())
                return null;

            ScriptRunner.ExecuteFile(GetControllerFilePath(controllerName));

            RubyClass controllerClass = GetRubyClass(GetControllerClassName(controllerName));
            RubyController controller = ConfigureController(controllerClass, requestContext);

            return controller;
        }

        public static string GetControllerClassName(string controllerName)
        {
            return (controllerName.EndsWith("Controller")
                        ? controllerName
                        : Constants.CONTROLLERCLASS_FORMAT.FormattedWith(controllerName)).Pascalize();
        }

        public static string GetControllerFilePath(string controllerName)
        {
            string fileName = Constants.CONTROLLER_PASCAL_PATH_FORMAT.FormattedWith(controllerName.Pascalize());
            if (HostingEnvironment.VirtualPathProvider.FileExists(fileName))
                return fileName;

            fileName = Constants.CONTROLLER_UNDERSCORE_PATH_FORMAT.FormattedWith(controllerName.Underscore());

            return HostingEnvironment.VirtualPathProvider.FileExists(fileName) ? fileName : string.Empty;
        }

        internal void RequireRubyFile(string path, ReaderType readerType)
        {
            new DefaultScriptRunner(Engine, readerType).ExecuteFile(path);
        }

        public RubyController ConfigureController(RubyClass rubyClass, RequestContext requestContext)
        {
            var controller = (RubyController) Operations.CreateInstance(rubyClass);
//            var controller = Operations.ConvertTo<RubyController>(obj);
//            controller.SetMediator(this);
//            CallMethod(controller, "old_initialize", requestContext);
//            CallMethod(controller, "SetMediator", this);
            var initializeMethod = Operations.GetMember<RubyMethod>(controller, "internal_initialize".Pascalize());
            Operations.Call(initializeMethod,
                            new ControllerConfiguration
                                {Context = requestContext, Mediator = this, RubyClass = rubyClass});
            return controller;
        }

        public RubyMethod GetMethod(object obj, string name)
        {
            return Operations.GetMember<RubyMethod>(obj, name.Underscore());
        }

        public object CallMethod(object obj, string name, params object[] args)
        {
            return Operations.Call(GetMethod(obj, name), args);
        }

        public Func<object> GetControllerAction(RubyController controller,
                                                string actionName)
        {
            // get explicit reference to action method object
//            var code = Constants.GET_ACTIONMETHOD_SCRIPT.FormattedWith(controllerName, actionName);
            RubyMethod action = GetMethod(controller, actionName);
            return GetDelegate(action);
        }

        public void LoadAssembly(Assembly assembly)
        {
            Runtime.LoadAssembly(assembly);
        }

        public object ExecuteScript(string script)
        {
            return ScriptRunner.ExecuteScript(script);
        }

        public T ExecuteScript<T>(string script)
        {
            return ScriptRunner.ExecuteScript<T>(script);
        }

        public void SetModelAndControllersPath()
        {
            string controllersDir = Path.Combine(HostingEnvironment.ApplicationPhysicalPath, Constants.CONTROLLERS);
            string modelsDir = Path.Combine(HostingEnvironment.ApplicationPhysicalPath, Constants.MODELS);

            Context.Loader.SetLoadPaths(new[] {controllersDir, modelsDir});
        }

        public void DefineReadOnlyGlobalVariable(string variableName, object value)
        {
            Context.DefineReadOnlyGlobalVariable(variableName, value);
        }

        public bool VariableExists(string variable)
        {
            return GetGlobalVariableName(variable).IsNotNullOrBlank();
        }

        public string GetGlobalVariableName(string nameProposal)
        {
            foreach (var variableName in Runtime.Globals.GetVariableNames())
            {
                if (String.Equals(variableName, nameProposal, StringComparison.OrdinalIgnoreCase))
                    return variableName;
            }
            return String.Empty;
        }

        public bool MethodExists(string methodName, RubyClass rubyClass)
        {
            return GetMethodName(methodName, rubyClass).IsNotNullOrBlank();
        }

        public string GetMethodName(string methodName, RubyClass rubyClass)
        {
            string result = String.Empty;

            using (Context.ClassHierarchyLocker())
            {
                rubyClass.EnumerateMethods((_, symbolId, __) =>
                                               {
                                                   if (String.Equals(symbolId, methodName,
                                                                     StringComparison.OrdinalIgnoreCase))
                                                   {
                                                       result = symbolId;
                                                       return true;
                                                   }

                                                   return false;
                                               });
            }
            return result;
        }

        public IList<string> GetMethodNames(RubyClass rubyClass)
        {
            return Operations.GetMemberNames(rubyClass);
        }

        public Func<object> GetDelegate(object obj)
        {
            Func<object> func = () => Engine.Operations.Call(obj);
            return func;
        }

        public RubyClass GetRubyClass(string className)
        {
            return GetGlobalVariable<RubyClass>(className);
        }

        public T GetGlobalVariable<T>(string name)
        {
            return Runtime.Globals.GetVariable<T>(name);
        }

        public void LoadAsssemblies(params Type[] assemblies)
        {
            assemblies.ForEach(type => LoadAssembly(type.Assembly));
        }

        public static RubyMediator Create()
        {
            ScriptRuntime runtime = HttpContext.Current.Application.GetScriptRuntime();
            new[] {typeof (object), typeof (Uri), typeof (Controller), typeof (RubyController)}
                .ForEach(type => runtime.LoadAssembly(type.Assembly));
            var rubyEngine = new RubyMediator(runtime);

//            rubyEngine.ExecuteScript("Controller = IronRubyMvc::RubyController");

            return rubyEngine;
        }
    }
}