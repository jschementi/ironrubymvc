using System;
using System.IO;
using System.Reflection;
using System.Web;
using System.Web.Hosting;
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
        

        public object LoadController(string controllerName)
        {
            var fileName = String.Format(@"~\Controllers\{0}.rb", controllerName);
            SetModelAndControllersPath();
            new DefaultScriptRunner(Engine, ReaderType.AssemblyResource).ExecuteFile(string.Format("IronRubyMvc.Controllers.controller.rb"));

            return ScriptRunner.ExecuteFile(fileName);
        }

        public Func<object> GetControllerAction(string controllerName, string actionName)
        {
            //Instantiate controller.
            var actionScript =
                @"$controller = {0}.new
$controller.Initialize $request_context
$controller.method :{1}";

            // get explicit reference to action method object
            var code = String.Format(actionScript, controllerName, actionName);
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
            var controllersDir = Path.Combine(HostingEnvironment.ApplicationPhysicalPath, "Controllers");
            var modelsDir = Path.Combine(HostingEnvironment.ApplicationPhysicalPath, "Models");

            Context.Loader.SetLoadPaths(new[] { controllersDir, modelsDir });
        }

        public void DefineReadOnlyGlobalVariable(string variableName, object value)
        {
            Context.DefineReadOnlyGlobalVariable(variableName, value);
        }

        public bool VariableExists(string variable)
        {
            return GetVariableName(variable).IsNotNullOrBlank();
        }

        public string GetVariableName(string nameProposal)
        {
            foreach (var variableName in Runtime.Globals.GetVariableNames())
            {
                if (String.Equals(variableName, nameProposal, StringComparison.OrdinalIgnoreCase))
                    return variableName;
            }
            return string.Empty;
        }

        public bool MethodExists(string methodName, RubyClass rubyClass)
        {
            return GetMethodName(methodName, rubyClass).IsNotNullOrBlank();
        }

        public string GetMethodName(string methodName, RubyClass rubyClass)
        {
            var result = string.Empty;
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

    }
}