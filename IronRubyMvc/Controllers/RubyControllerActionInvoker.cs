extern alias Core;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Routing;
using IronRuby;
using IronRuby.Builtins;
using IronRuby.Runtime;
using Microsoft.Scripting.Hosting;

namespace IronRubyMvc
{
    internal class RubyControllerActionInvoker : ControllerActionInvoker
    {
        private Core::System.Func<object> _action;

        public RubyControllerActionInvoker(RequestContext context, ScriptRuntime scriptRuntime, string controllerName)
        {
            Controller = controllerName;
            ScriptRuntime = scriptRuntime;
        }

        public string Controller { get; private set; }

        public ScriptRuntime ScriptRuntime { get; private set; }

        protected override ActionDescriptor FindAction(ControllerContext controllerContext,
                                                       ControllerDescriptor controllerDescriptor, string actionName)
        {
            // For now limit action name to alphanumeric characters
            if (!Regex.IsMatch(actionName, @"^(\w)+$"))
                return null;

            var rubyEngine = Ruby.GetEngine(ScriptRuntime);
            var rubyContext = Ruby.GetExecutionContext(ScriptRuntime);

            // add references (mscorlib, System, Mvc, and RubyController) + other headers
            foreach (Type type in new[] {typeof (object), typeof (Uri), typeof (Controller), typeof (RubyController)})
                ScriptRuntime.LoadAssembly(type.Assembly);

            rubyEngine.CreateScriptSourceFromString("Controller = IronRubyMvc::RubyController").Execute();

            // add Controllers + Models paths
            var controllersDir = Path.Combine(HostingEnvironment.ApplicationPhysicalPath, "Controllers");
            var modelsDir = Path.Combine(HostingEnvironment.ApplicationPhysicalPath, "Models");
            rubyContext.Loader.SetLoadPaths(new[] {controllersDir, modelsDir});
            //rubyContext.Loader.SetLoadPaths(controllersDir, modelsDir);

            // inject controller code
            var fileName = String.Format(@"~\Controllers\{0}.rb", Controller);
            if (!HostingEnvironment.VirtualPathProvider.FileExists(fileName))
            {
                return null;
            }

            var file = HostingEnvironment.VirtualPathProvider.GetFile(fileName);
            using (var stream = file.Open())
            {
                using (TextReader reader = new StreamReader(stream))
                {
                    string allScript = reader.ReadToEnd();
                    ScriptSource source = rubyEngine.CreateScriptSourceFromString(allScript);
                    source.Execute();
                }
            }

            rubyContext.DefineReadOnlyGlobalVariable("request_context", controllerContext.RequestContext);
            rubyContext.DefineReadOnlyGlobalVariable("script_runtime", ScriptRuntime);

            var controllerRubyClassName =
                ScriptRuntime.Globals.GetVariableNames().SingleOrDefault(
                    name => String.Equals(name, Controller, StringComparison.OrdinalIgnoreCase));
            if (String.IsNullOrEmpty(controllerRubyClassName))
            {
                // controller not found
                return null;
            }

            //this.ScriptRuntime.UseFile()
                var controllerRubyClass = ScriptRuntime.Globals.GetVariable<RubyModule>(controllerRubyClassName);
                string controllerRubyMethodName = null;
            using (rubyContext.ClassHierarchyLocker())
            {
                controllerRubyClass.EnumerateMethods((_, symbolId, __) =>
                                                         {
                                                             if (String.Equals(symbolId, actionName,
                                                                               StringComparison.OrdinalIgnoreCase))
                                                             {
                                                                 controllerRubyMethodName = symbolId;
                                                                 return true;
                                                             }
                                                             return false;
                                                         });
            }

            if (String.IsNullOrEmpty(controllerRubyMethodName))
            {
                // action not found
                return null;
            }

            //Instantiate controller.
            string actionScript =
                @"$controller = {0}.new
$controller.Initialize $request_context
$controller.method :{1}";

            // get explicit reference to action method object
            string code = String.Format(actionScript, controllerRubyClassName, controllerRubyMethodName);
            object action = rubyEngine.CreateScriptSourceFromString(code).Execute();
            _action = () => rubyEngine.Operations.Call(action);

            return new RubyActionDescriptor(RubyController.InvokeActionMethod, actionName, controllerDescriptor);
        }


        protected override object GetParameterValue(ControllerContext controllerContext,
                                                    ParameterDescriptor parameterDescriptor)
        {
            if (parameterDescriptor.ParameterName == "__action")
            {
                ((RubyParameterDescriptor)parameterDescriptor).SetParameterType(_action.GetType());
                return _action;
            }
            return base.GetParameterValue(controllerContext, parameterDescriptor);
        }


        private static string PascalCaseIt(string s)
        {
            return s[0].ToString().ToUpper() + s.Substring(1);
        }
    }
}