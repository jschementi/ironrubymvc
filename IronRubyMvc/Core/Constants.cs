namespace IronRubyMvc.Core
{
    internal class Constants
    {
        public const string RUBYCONTROLLER_FILE = "IronRubyMvc.Controllers.controller.rb";
        public const string CONTROLLER_PATH_FORMAT = @"~\Controllers\{0}.rb";
        public const string GET_ACTIONMETHOD_SCRIPT = @"controller = {0}.new
controller.Initialize $request_context
controller.method :{1}";

        public const string CONTROLLERS = "Controllers";
        public const string MODELS = "Models";
        public const string CONTROLLERCLASS_FORMAT = "{0}Controller";
        public const string REQUEST_CONTEXT_VARIABLE = "request_context";
        public const string SCRIPT_RUNTIME_VARIABLE = "script_runtime";

        public const string ACTION_NAME_REGEX = @"^(\w)+$";
        public const string CONTROLLERNAME_NAME_REGEX = @"^(\w)+$";
    }
}