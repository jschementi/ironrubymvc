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

            return _params;
        }

        #endregion

    }
}