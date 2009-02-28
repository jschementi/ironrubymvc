namespace IronRubyMvcLibrary.Core
{
    internal class Constants
    {
        public const string ACTION_NAME_REGEX = @"^(\w)+$";
        public const string CONTROLLER_PASCAL_PATH_FORMAT = @"~\Controllers\{0}Controller.rb";
        public const string CONTROLLER_UNDERSCORE_PATH_FORMAT = @"~\Controllers\{0}_controller.rb";
        public const string CONTROLLERCLASS_FORMAT = "{0}Controller";
        public const string CONTROLLERNAME_NAME_REGEX = @"^(\w)+$";

        public const string CONTROLLERS = "Controllers";

        public const string GET_ACTIONMETHOD_SCRIPT =
            @"controller = {0}.new
controller.Initialize $request_context
controller.method :{1}";

        public const string MODELS = "Models";
        public const string FILTERS = "Filters";
        public const string FILTERS_PASCAL_PATH_FORMAT = @"~\Filters\{0}.rb";
        public const string FILTERS_UNDERSCORE_PATH_FORMAT = @"~\Filters\{0}.rb";
        public const string REQUEST_CONTEXT_VARIABLE = "request_context";
        public const string RUBYCONTROLLER_FILE = "IronRubyMvcLibrary.Controllers.controller.rb";
        public const string SCRIPT_RUNTIME_VARIABLE = "script_runtime";
    }
}