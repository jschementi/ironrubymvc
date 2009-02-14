#region Usings

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using IronRuby.Builtins;
using IronRubyMvcLibrary.Core;
using IronRubyMvcLibrary.Extensions;
using IronRubyMvcLibrary.Helpers;
using Microsoft.Scripting;
using RubyModuleDefinition = IronRuby.Runtime.RubyModuleAttribute;
using RubyClassDefinition = IronRuby.Runtime.RubyClassAttribute;
using RubyMethodDefinition = IronRuby.Runtime.RubyMethodAttribute;

#endregion

namespace IronRubyMvcLibrary.Controllers
{
//    [RubyModuleDefinition("IronRubyMvc")]
//    public static class IronRubyMvcModule
//    {
//        #region Nested type: RubyControllerOps
//
//        [RubyClassDefinition("Controller", Extends = typeof (RubyController))]
//        public class RubyControllerOps : RubyController
//        {
//            [RubyMethodDefinition("info", RubyMethodAttributes.PublicInstance)]
//            public static void Info(RubyController self)
//            {
//                self.ViewData().Add("Platform", "IronRuby Mvc 1.0");
//            }
//
//           
//        }
//
//        #endregion
//
//        //    [IronRuby.Runtime.RubyClass("Controller", Extends = typeof(Controller))]
//    }
//    [RubyClassDefinition("RubyController", HideClrMembers = false, Extends = typeof(Controller))]
    public class RubyController : Controller
    {
        public static readonly MethodInfo InvokeActionMethod = typeof (RubyController).GetMethod("InvokeAction");
        private readonly Dictionary<object, object> _viewData = new Dictionary<object, object>();

        private RubyEngine _engine;
        private IDictionary<object, object> _params;

        public string ControllerName { get; internal set; }
        public RubyClass RubyType { get; private set; }

        public string ControllerClassName
        {
            get { return Constants.CONTROLLERCLASS_FORMAT.FormattedWith(ControllerName); }
        }

//        [RubyConstructor]
//        public static RubyController/*!*/ Create(RubyClass/*!*/ self)
//        {
//            return new RubyController();
//        }
//
//        [RubyConstructor]
//        public static RubyController Create(RubyClass/*!*/ self, string controllerName)
//        {
//            return new RubyController{ControllerName = controllerName};
//        }


        public IDictionary<object, object> Params
        {
            get
            {
                if (_params == null)
                {
                    HttpRequestBase request = ControllerContext.HttpContext.Request;
                    _params =
                        new Dictionary<object, object>(ControllerContext.RouteData.Values.Count +
                                                       request.QueryString.Count + request.Form.Count);

                    foreach (var item in ControllerContext.RouteData.Values)
                    {
                        SymbolId key = SymbolTable.StringToId(item.Key);
                        _params[key] = item.Value;
                    }

                    foreach (string key in request.QueryString.Keys)
                    {
                        SymbolId symbolKey = SymbolTable.StringToId(key);
                        _params[symbolKey] = request.QueryString[key];
                    }

                    foreach (string key in request.Form.Keys)
                    {
                        SymbolId symbolKey = SymbolTable.StringToId(key);
                        _params[symbolKey] = request.Form[key];
                    }
                }

                return _params;
            }
        }

        internal void InternalInitialize(ControllerConfiguration config)
        {
            Initialize(config.Context);
            SetMediator(config.Engine);
            ControllerName = config.RubyClass.Name.Replace("Controller", string.Empty);
            RubyType = config.RubyClass;
        }

        internal void SetMediator(RubyEngine engine)
        {
            if (engine == null) throw new ArgumentNullException("engine");
            _engine = engine;
        }


        protected override void Execute(RequestContext requestContext)
        {
            ActionInvoker = new RubyControllerActionInvoker(ControllerClassName, _engine);
            base.Execute(requestContext);
        }

//        [NonAction]
//        public object InvokeAction(Func<object> action)
//        {
//            return action();
//        }

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

        protected override ViewResult View(string viewName, string masterName, object model)
        {
            var vdd = new ViewDataDictionary();
            vdd["__scriptRuntime"] = _engine.Runtime;

            foreach (var entry in _viewData)
                vdd[Convert.ToString(entry.Key, CultureInfo.InvariantCulture)] = entry.Value;

            var hash = model as Hash;
            vdd.Model = (hash != null) ? new HashWrapper(hash) : model;

            return new ViewResult {ViewName = viewName, MasterName = masterName, ViewData = vdd, TempData = TempData};
        }

        [NonAction]
        public new IDictionary ViewData()
        {
            return _viewData;
        }
    }
}