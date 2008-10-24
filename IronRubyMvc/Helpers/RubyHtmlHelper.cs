namespace IronRubyMvc {
    using System.Web.Mvc;
    using System.Web.Mvc.Html;
    using IronRuby.Builtins;

    // Note, it looks like DLR interop is not calling base methods. Until this is fixed, we might
    // want to come through and add pass-throughs (in addition to the Hash-specific versions).

    public class RubyHtmlHelper : HtmlHelper {
        HtmlHelper _helper;

        public RubyHtmlHelper(ViewContext context, IViewDataContainer viewDataContainer) : base(context, viewDataContainer) {
            _helper = new HtmlHelper(context, viewDataContainer);
        }

        public string ActionLink(string linkText, Hash values) {
            
            return _helper.RouteLink(linkText, values.ToRouteDictionary());
        }

        public string ActionLink(string linkText, string actionName, Hash values) {
            return _helper.ActionLink(linkText, actionName, values.ToRouteDictionary());
        }

        public string ActionLink(string linkText, string actionName, string controllerName) {
            return _helper.ActionLink(linkText, actionName, controllerName);
        }

        public string ActionLink(string linkText, string actionName) {
            return _helper.ActionLink(linkText, actionName);
        }

        public void RenderPartial(string partialViewName) {
            _helper.RenderPartial(partialViewName);
        }

        public void RenderPartial(string partialViewName, object model) {
            _helper.RenderPartial(partialViewName, model);
        }

        public void RenderPartial(string partialViewName, Hash viewData) {
            _helper.RenderPartial(partialViewName, viewData.ToViewDataDictionary());
        }

        public void RenderPartial(string partialViewName, object model, Hash viewData) {
            _helper.RenderPartial(partialViewName, model, viewData.ToViewDataDictionary());
        }

        public string TextBox(string name) {
            //Yeah, I know this is sooo wrong, but still.
            name = name.Replace("_", "");
            return _helper.TextBox(name);
        }

        public string TextBox(string name, object value) {
            //Yeah, I know this is sooo wrong, but still.
            name = name.Replace("_", "");
            return _helper.TextBox(name, value.ToString());
        }

        public string Hidden(string name, object value) {
            return _helper.Hidden(name, value.ToString());
        }
    }
}
