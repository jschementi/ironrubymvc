#region Usings

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Threading;
using System.Web.Mvc.IronRuby.Core;
using System.Web.Mvc.IronRuby.Extensions;
using System.Web.Mvc.IronRuby.Helpers;
using System.Web.Routing;
using IronRuby.Builtins;
using Microsoft.Scripting;

#endregion

namespace System.Web.Mvc.IronRuby.Controllers
{
    public class RubyController : Controller
    {
        private readonly Dictionary<object, object> _viewData = new Dictionary<object, object>();
        private IRubyEngine _engine;
        private IDictionary<object, object> _params;

        public string ControllerName { get; internal set; }
        public RubyClass RubyType { get; private set; }

        public string ControllerClassName
        {
            get { return Constants.CONTROLLERCLASS_FORMAT.FormattedWith(ControllerName); }
        }

        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Params")]
        public IDictionary<object, object> Params
        {
            get
            {
                if (_params == null)
                {
                    PopulateParams();
                }

                return _params;
            }
        }

        private void PopulateParams()
        {
            var request = ControllerContext.HttpContext.Request;
            _params =
                new Dictionary<object, object>(ControllerContext.RouteData.Values.Count +
                                               request.QueryString.Count + request.Form.Count);
            PopulateParamsWithRouteData();

            PopulateParamsWithQueryStringData(request);

            PopulateParamsWithFormData(request);
        }

        private void PopulateParamsWithFormData(HttpRequestBase request)
        {
            foreach (string key in request.Form.Keys)
            {
                var symbolKey = SymbolTable.StringToId(key);
                _params[symbolKey] = request.Form[key];
                ModelState.Add(key, new ModelState{Value = new ValueProviderResult(request.Form[key], request.Form[key], CultureInfo.CurrentCulture)});
            }
        }

        private void PopulateParamsWithQueryStringData(HttpRequestBase request)
        {
            foreach (string key in request.QueryString.Keys)
            {
                var symbolKey = SymbolTable.StringToId(key);
                _params[symbolKey] = request.QueryString[key];
            }
        }

        private void PopulateParamsWithRouteData()
        {
            foreach (var item in ControllerContext.RouteData.Values)
            {
                var key = SymbolTable.StringToId(item.Key);
                _params[key] = item.Value;
            }
        }

        internal void InternalInitialize(ControllerConfiguration config)
        {
            Initialize(config.Context);
            _engine = config.Engine;
            ControllerName = config.RubyClass.Name.Replace("Controller", string.Empty);
            RubyType = config.RubyClass;
        }

        protected override void Execute(RequestContext requestContext)
        {
            PopulateParams();
            ActionInvoker = new RubyControllerActionInvoker(ControllerClassName, _engine);
            base.Execute(requestContext);
        }

        [NonAction]
        public ActionResult RedirectToRoute(Hash values)
        {
            return RedirectToRoute(values.ToRouteDictionary());
        }

        [NonAction]
        public ActionResult RedirectToAction(string actionName, Hash values)
        { 
            return RedirectToAction(actionName, values.ToRouteDictionary());
        }

        [NonAction]
        public new ViewResult View()
        {
            return View(null /* viewName */, null /* masterName */, null /* model */);
        }

        [NonAction]
        public new ViewResult View(object model)
        {
            return View(null /* viewName */, null /* masterName */, model);
        }

        [NonAction]
        public new ViewResult View(string viewName)
        {
            return View(viewName, null /* masterName */, null /* model */);
        }

        [NonAction]
        public new ViewResult View(string viewName, string masterName)
        {
            return View(viewName, masterName, null /* model */);
        }

        [NonAction]
        public new ViewResult View(string viewName, object model)
        {
            return View(viewName, null /* masterName */, model);
        }

        public new ViewResult View(string viewName, string masterName, object model)
        {
            var vdd = new ViewDataDictionary();
            vdd["__scriptRuntime"] = ((RubyEngine) _engine).Runtime;

            _engine.CallMethod(this, "fill_view_data");
            foreach (var entry in _viewData)
                vdd[Convert.ToString(entry.Key, CultureInfo.InvariantCulture)] = entry.Value;
            
            var hash = model as Hash;
            vdd.Model = (hash != null) ? new HashWrapper(hash) : model;
            ModelState.ForEach(pair => vdd.ModelState.Add(pair.Key.ToString(), pair.Value));

            return new ViewResult {ViewName = viewName, MasterName = masterName, ViewData = vdd, TempData = TempData};
        }

        [NonAction]
        public new IDictionary ViewData()
        {
            return _viewData;
        }
    }
}