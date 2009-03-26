using System.Collections.Generic;
using System.Web.Mvc.Html;
using System.Web.Mvc.IronRuby.Extensions;
using System.Web.Routing;
using IronRuby.Builtins;

namespace System.Web.Mvc.IronRuby.Helpers
{
    public partial class RubyHtmlHelper
    {

        public string ValidationMessage(string modelName)
        {
            return ValidationMessage(modelName, (object)null /* htmlAttributes */);
        }

        public string ValidationMessage(string modelName, object htmlAttributes)
        {
            return ValidationMessage(modelName, new RouteValueDictionary(htmlAttributes));
        }

        public string ValidationMessage(string modelName, string validationMessage)
        {
            return ValidationMessage(modelName, validationMessage, (object)null /* htmlAttributes */);
        }

        public string ValidationMessage(string modelName, string validationMessage, object htmlAttributes)
        {
            return ValidationMessage(modelName, validationMessage, new RouteValueDictionary(htmlAttributes));
        }

        public string ValidationMessage(string modelName, IDictionary<string, object> htmlAttributes)
        {
            return ValidationMessage(modelName, null /* validationMessage */, htmlAttributes);
        }

        public string ValidationMessage(string modelName, string validationMessage, Hash htmlAttributes)
        {
            return _helper.ValidationMessage(modelName, validationMessage, htmlAttributes.ToRouteDictionary());
        }

        public string ValidationSummary()
        {
            return ValidationSummary(null /* message */);
        }

        public string ValidationSummary(string message)
        {
            return ValidationSummary(message, (object)null /* htmlAttributes */);
        }

        public string ValidationSummary(string message, object htmlAttributes)
        {
            return ValidationSummary(message, new RouteValueDictionary(htmlAttributes));
        }

        public string ValidationSummary(string message, Hash htmlAttributes)
        {
            return _helper.ValidationSummary(message, htmlAttributes.ToRouteDictionary());
        }
    }
}