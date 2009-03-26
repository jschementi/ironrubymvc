#region Usings

using System.Web.Mvc.Html;
using System.Web.Mvc.IronRuby.Extensions;
using IronRuby.Builtins;

#endregion

namespace System.Web.Mvc.IronRuby.Helpers
{
    public partial class RubyHtmlHelper
    {
        public string ValidationMessage(string modelName)
        {
            return _helper.ValidationMessage(modelName);
        }

        public string ValidationMessage(string modelName, Hash htmlAttributes)
        {
            return _helper.ValidationMessage(modelName, htmlAttributes.ToDictionary());
        }

        public string ValidationMessage(string modelName, string validationMessage)
        {
            return _helper.ValidationMessage(modelName, validationMessage);
        }

        public string ValidationMessage(string modelName, string validationMessage, Hash htmlAttributes)
        {
            return _helper.ValidationMessage(modelName, validationMessage, htmlAttributes.ToDictionary());
        }

        public string ValidationSummary()
        {
            return _helper.ValidationSummary();
        }

        public string ValidationSummary(string message)
        {
            return _helper.ValidationSummary(message);
        }

        public string ValidationSummary(string message, Hash htmlAttributes)
        {
            return _helper.ValidationSummary(message, htmlAttributes.ToDictionary());
        }
    }
}