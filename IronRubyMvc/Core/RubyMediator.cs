using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using IronRuby;
using IronRuby.Builtins;
using IronRuby.Runtime;
using Microsoft.Scripting.Hosting;

namespace IronRubyMvc.Core
{
    /// <summary>
    /// A wrapper for ScriptEngine, Runtime and Context
    /// </summary>
    internal class RubyMediator
    {
        public RubyMediator() : this(HttpContext.Current.Application.GetScriptRuntime())
        { }

        public RubyMediator(ScriptRuntime runtime)
        {
            Runtime = runtime;
            Engine = Ruby.GetEngine(Runtime);
            Context = Ruby.GetExecutionContext(Engine);
            CurrentScope = Engine.CreateScope();
            Operations = Engine.CreateOperations();
            ScriptRunner = new ScopedScriptRunner(Engine, CurrentScope);
        }

        public ScriptRuntime Runtime { get; set; }
        public RubyContext Context { get; private set; }
        public ScriptEngine Engine { get; private set; }
        public IScriptRunner ScriptRunner { get; set; }
        public ScriptScope CurrentScope { get; private set; }
        public ObjectOperations Operations { get; private set; }
        

        public object LoadController(string controllerName, ControllerContext controllerContext)
        {
            var fileName = Constants.CONTROLLER_PATH_FORMAT.FormattedWith(controllerName);
            SetModelAndControllersPath();
            new ScopedScriptRunner(Engine, CurrentScope, ReaderType.AssemblyResource).ExecuteFile(Constants.RUBYCONTROLLER_FILE);
            
//            CurrentScope.SetVariable(Constants.REQUEST_CONTEXT_VARIABLE, controllerContext.RequestContext);
//            CurrentScope.SetVariable(Constants.SCRIPT_RUNTIME_VARIABLE, Engine);
//            DefineReadOnlyGlobalVariable(Constants.REQUEST_CONTEXT_VARIABLE, controllerContext.RequestContext);
            DefineReadOnlyGlobalVariable(Constants.SCRIPT_RUNTIME_VARIABLE, Engine);

            return ScriptRunner.ExecuteFile(fileName);
        }

        public void DefineScopedVariable(string name, object value)
        {
            
        }

        public void RequireRubyFile(string path, ReaderType readerType)
        {
            new ScopedScriptRunner(Engine, CurrentScope, readerType).ExecuteFile(path);
        }

        public Func<object> GetControllerAction(string controllerName, string actionName)
        {
            // get explicit reference to action method object
            var code = Constants.GET_ACTIONMETHOD_SCRIPT.FormattedWith(controllerName, actionName);
            var action = ExecuteScript(code);
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
            var controllersDir = Path.Combine(HostingEnvironment.ApplicationPhysicalPath, Constants.CONTROLLERS);
            var modelsDir = Path.Combine(HostingEnvironment.ApplicationPhysicalPath, Constants.MODELS);

            Context.Loader.SetLoadPaths(new[] { controllersDir, modelsDir });
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
            var result = String.Empty;
            
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
            var rubyEngine = new RubyMediator();

            rubyEngine.LoadAsssemblies(typeof (object), typeof (Uri), typeof (Controller), typeof (RubyController));
            rubyEngine.ExecuteScript("Controller = IronRubyMvc::RubyController");

            return rubyEngine;
        }
    }
}