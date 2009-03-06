namespace System.Web.Mvc.IronRuby.Core
{
    internal class Constants
    {
        public const string CONTROLLER_PASCAL_PATH_FORMAT = @"~\Controllers\{0}Controller.rb";
        public const string CONTROLLER_UNDERSCORE_PATH_FORMAT = @"~\Controllers\{0}_controller.rb";
        public const string CONTROLLERCLASS_FORMAT = "{0}Controller";
        public const string CONTROLLERNAME_NAME_REGEX = @"^(\w)+$";

        public const string CONTROLLERS = "Controllers";

        public const string FILTERS = "Filters";
        public const string FILTERS_PASCAL_PATH_FORMAT = @"~\Filters\{0}.rb";
        public const string FILTERS_UNDERSCORE_PATH_FORMAT = @"~\Filters\{0}.rb";

        public const string MODELS = "Models";
        public const string RUBYCONTROLLER_FILE = "System.Web.Mvc.IronRuby.Controllers.controller.rb";
        public const string SCRIPT_RUNTIME_VARIABLE = "script_runtime";
    }
}