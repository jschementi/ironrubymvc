namespace System.Web.Mvc.IronRuby.Core
{
    internal class Constants
    {
        public const string ControllerPascalPathFormat = @"~\Controllers\{0}Controller.rb";
        public const string ControllerUnderscorePathFormat = @"~\Controllers\{0}_controller.rb";
        public const string ControllerclassFormat = "{0}Controller";
        public const string ControllernameNameRegex = @"^(\w)+$";

        public const string Controllers = "Controllers";

        public const string Filters = "Filters";
        public const string FiltersPascalPathFormat = @"~\Filters\{0}.rb";
        public const string FiltersUnderscorePathFormat = @"~\Filters\{0}.rb";

        public const string Models = "Models";
        public const string RubycontrollerFile = "System.Web.Mvc.IronRuby.Controllers.controller.rb";
        public const string ScriptRuntimeVariable = "script_runtime";

        public const string Helpers = "Helpers";
        public const string HelpersPascalPathFormat = @"~\Filters\{0}.rb";
        public const string HelpersUnderscorePathFormat = @"~\Filters\{0}.rb";
    }
}