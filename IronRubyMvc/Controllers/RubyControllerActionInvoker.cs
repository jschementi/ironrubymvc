namespace IronRubyMvc {
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using System.Web.Hosting;
    using System.Web.Mvc;
    using System.Web.Routing;
    using Microsoft.Scripting.Hosting;
    using IronRuby;
    using IronRuby.Builtins;

    internal class RubyControllerActionInvoker : ControllerActionInvoker {

        Func<object> _action;

        public RubyControllerActionInvoker(RequestContext context, ScriptRuntime scriptRuntime, string controllerName) {
            Controller = controllerName;
            ScriptRuntime = scriptRuntime;
        }

        public string Controller { get; private set; }

        public ScriptRuntime ScriptRuntime { get; private set; }

        protected override MethodInfo FindActionMethod(string actionName) {
            // For now limit action name to alphanumeric characters
            if (!Regex.IsMatch(actionName, @"^(\w)+$"))
                return null;

            var rubyEngine = Ruby.GetEngine(ScriptRuntime);
            var rubyContext = Ruby.GetExecutionContext(ScriptRuntime);

            // add references (mscorlib, System, Mvc, and RubyController) + other headers
            foreach (Type type in new[] { typeof(object), typeof(Uri), typeof(Controller), typeof(RubyController) })
                ScriptRuntime.LoadAssembly(type.Assembly);

            rubyEngine.CreateScriptSourceFromString("Controller = IronRubyMvc::RubyController").Execute();

            // add Controllers + Models paths
            string controllersDir = Path.Combine(HostingEnvironment.ApplicationPhysicalPath, "Controllers");
            string modelsDir = Path.Combine(HostingEnvironment.ApplicationPhysicalPath, "Models");
            rubyContext.Loader.SetLoadPaths(new string[] { controllersDir, modelsDir });
            //rubyContext.Loader.SetLoadPaths(controllersDir, modelsDir);

            // inject controller code
            string fileName = String.Format(@"~\Controllers\{0}.rb", Controller);
            if (!HostingEnvironment.VirtualPathProvider.FileExists(fileName)) {
                return null;
            }

            var file = HostingEnvironment.VirtualPathProvider.GetFile(fileName);
            using (Stream stream = file.Open()) {
                using (TextReader reader = new StreamReader(stream)) {
                    var allScript = reader.ReadToEnd();
                    var source = rubyEngine.CreateScriptSourceFromString(allScript);
                    source.Execute();
                }
            }

            rubyContext.DefineReadOnlyGlobalVariable("controller_context", ControllerContext);
            rubyContext.DefineReadOnlyGlobalVariable("script_runtime", ScriptRuntime);

            string controllerRubyClassName = ScriptRuntime.Globals.GetVariableNames().SingleOrDefault(name => String.Equals(name, Controller, StringComparison.OrdinalIgnoreCase));
            if (String.IsNullOrEmpty(controllerRubyClassName)) {
                // controller not found
                return null;
            }

            RubyClass controllerRubyClass = ScriptRuntime.Globals.GetVariable<RubyClass>(controllerRubyClassName);
            string controllerRubyMethodName = null;
            controllerRubyClass.EnumerateMethods((_, symbolId, __) => {
                if (String.Equals(symbolId.ToString(), actionName, StringComparison.OrdinalIgnoreCase)) {
                    controllerRubyMethodName = symbolId.ToString();
                    return true;
                }
                return false;
            });

            if (String.IsNullOrEmpty(controllerRubyMethodName)) {
                // action not found
                return null;
            }

            //Instantiate controller.
            string actionScript = @"$controller = {0}.new
$controller.Initialize $controller_context
$controller.method :{1}";

            // get explicit reference to action method object
            var code = String.Format(actionScript, controllerRubyClassName, controllerRubyMethodName);
            object action = rubyEngine.CreateScriptSourceFromString(code).Execute();
            _action = () => rubyEngine.Operations.Call(action);

            return RubyController.InvokeActionMethod;
        }

        protected override object GetParameterValue(ParameterInfo parameterInfo) {
            if (parameterInfo.Name == "__action") {
                return _action;
            }
            return base.GetParameterValue(parameterInfo);
        }

        private static string PascalCaseIt(string s) {

            return s[0].ToString().ToUpper() + s.Substring(1);
        }
    }
}
