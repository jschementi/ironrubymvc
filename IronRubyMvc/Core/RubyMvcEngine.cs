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
    internal class RubyMvcEngine
    {
        public RubyMvcEngine() : this(HttpContext.Current.Application.GetScriptRuntime())
        { }

        public RubyMvcEngine(ScriptRuntime runtime)
        {
            Runtime = runtime;
            Engine = Ruby.GetEngine(Runtime);
            Context = Ruby.GetExecutionContext(Engine);
            ScriptRunner = new DefaultScriptRunner(Engine);
        }

        public ScriptRuntime Runtime { get; set; }
        public RubyContext Context { get; private set; }
        public ScriptEngine Engine { get; private set; }
        public IScriptRunner ScriptRunner { get; set; }
        

        public object LoadController(string controllerName, ControllerContext controllerContext)
        {
            var fileName = String.Format(Constants.CONTROLLER_PATH_FORMAT, controllerName);
            SetModelAndControllersPath();
            new DefaultScriptRunner(Engine, ReaderType.AssemblyResource).ExecuteFile(Constants.RUBYCONTROLLER_FILE);
            
            DefineReadOnlyGlobalVariable(Constants.REQUEST_CONTEXT_VARIABLE, controllerContext.RequestContext);
            DefineReadOnlyGlobalVariable(Constants.SCRIPT_RUNTIME_VARIABLE, Engine);

            return ScriptRunner.ExecuteFile(fileName);
        }

        public Func<object> GetControllerAction(string controllerName, string actionName)
        {
            // get explicit reference to action method object
            var code = String.Format(Constants.GET_ACTIONMETHOD_SCRIPT, controllerName, actionName);
            var action = ExecuteScript(code);
            return () => Engine.Operations.Call(action);
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

        public string[] GetMethodNames(RubyClass rubyClass)
        {
            var result = new List<string>();
            using (Context.ClassHierarchyLocker())
            {
                rubyClass.ForEachInstanceMethod(true, (_, symbolId, __) =>
                {
                    result.Add(symbolId);
                    return true;
                });
            }
            return result.ToArray();
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

        public static RubyMvcEngine Create()
        {
            var rubyEngine = new RubyMvcEngine();

            foreach (Type type in new[] {typeof (object), typeof (Uri), typeof (Controller), typeof (RubyController)})
                rubyEngine.LoadAssembly(type.Assembly);

            rubyEngine.ExecuteScript("Controller = IronRubyMvc::RubyController");

            return rubyEngine;
        }
    }
}