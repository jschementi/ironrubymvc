#region Usings

using System;
using System.Web.Mvc;
using IronRubyMvcLibrary.Extensions;

#endregion

namespace IronRubyMvcLibrary.Tests.Controllers
{
    

    public abstract class DelegateActionFilter : IActionFilter, IResultFilter
    {
        public Action<ActionExecutingContext> BeforeAction { get; set; }
        public Action<ActionExecutedContext> AfterAction { get; set; }
        public Action<ResultExecutingContext> BeforeResult { get; set; }
        public Action<ResultExecutedContext> AfterResult { get; set; }

        #region IActionFilter Members

        public virtual void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if(BeforeAction.IsNotNull()) BeforeAction(filterContext);
        }

        public virtual void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if(AfterAction.IsNotNull()) AfterAction(filterContext);
        }

        #endregion

        #region Implementation of IResultFilter

        public virtual void OnResultExecuting(ResultExecutingContext filterContext)
        {
            if (BeforeResult.IsNotNull()) BeforeResult(filterContext);
        }

        public virtual void OnResultExecuted(ResultExecutedContext filterContext)
        {
            if (AfterResult.IsNotNull()) AfterResult(filterContext);
        }

        #endregion
    }
}