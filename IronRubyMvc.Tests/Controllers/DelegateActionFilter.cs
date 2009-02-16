#region Usings

using System;
using System.Web.Mvc;
using IronRubyMvcLibrary.Extensions;

#endregion

namespace IronRubyMvcLibrary.Tests.Controllers
{
    public abstract class DelegateActionFilter : IActionFilter
    {
        private readonly Action<ActionExecutedContext> _onActionExecuted;
        private readonly Action<ActionExecutingContext> _onActionExecuting;

        protected DelegateActionFilter(Action<ActionExecutingContext> onActionExecuting,
                                    Action<ActionExecutedContext> onActionExecuted)
        {
            _onActionExecuting = onActionExecuting;
            _onActionExecuted = onActionExecuted;
        }

        #region IActionFilter Members

        public virtual void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if(_onActionExecuting.IsNotNull()) _onActionExecuting(filterContext);
        }

        public virtual void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if(_onActionExecuted.IsNotNull()) _onActionExecuted(filterContext);
        }

        #endregion
    }
}