#region Usings

using System.Collections.Generic;
using System.Globalization;
using System.Web.Mvc.IronRuby.Extensions;
using Microsoft.Scripting;

#endregion

namespace System.Web.Mvc.IronRuby.Controllers
{
    public class ParamsBinder : IModelBinder
    {
        private IDictionary<SymbolId, object> _params;

        #region IModelBinder Members

        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            controllerContext.EnsureArgumentNotNull("controllerContext");
            bindingContext.EnsureArgumentNotNull("bindingContext");

            _params = (bindingContext.Model as IDictionary<SymbolId, object>) ?? new Dictionary<SymbolId, object>();
            bindingContext.ValueProvider.ForEach(pair =>
                                                     {
                                                         bindingContext.ModelState.SetModelValue(pair.Key, pair.Value);
                                                         _params.Add(pair.Key.ToSymbolId(), pair.Value.AttemptedValue);
                                                     });
//            var request = controllerContext.HttpContext.Request;
//            var modelState = controllerContext.Controller.ViewData.ModelState;
//
//            PopulateParamsWithFormData(request, modelState);
//            PopulateParamsWithQueryStringData(request, modelState);
//            PopulateParamsWithRouteData(controllerContext.RouteData.Values);

            return _params;
        }

        #endregion

//        private void PopulateParamsWithFormData(HttpRequestBase request, IDictionary<string, ModelState> modelState)
//        {
//            foreach (string key in request.Form.Keys)
//            {
//                var symbolKey = SymbolTable.StringToId(key);
//                _params[symbolKey] = request.Form[key];
//                modelState.Add(key, CreateModelState(request.Form[key]));
//            }
//        }
//
//        private void PopulateParamsWithQueryStringData(HttpRequestBase request, IDictionary<string, ModelState> modelState)
//        {
//            foreach (string key in request.QueryString.Keys)
//            {
//                var symbolKey = SymbolTable.StringToId(key);
//                var value = request.QueryString[key];
//                _params[symbolKey] = value;
//                modelState.Add(key, CreateModelState(value));
//            }
//        }
//
//        private void PopulateParamsWithRouteData(IEnumerable<KeyValuePair<string, object>> routeValueDictionary)
//        {
//            foreach (var item in routeValueDictionary)
//            {
//                var key = SymbolTable.StringToId(item.Key);
//                _params[key] = item.Value;
//            }
//        }
//
//        private static ModelState CreateModelState(string value)
//        {
//            return new ModelState {Value = CreateValueProviderResult(value)};
//        }
//
//        private static ValueProviderResult CreateValueProviderResult(string value)
//        {
//            return new ValueProviderResult(value, value, CultureInfo.CurrentCulture);
//        }
    }
}