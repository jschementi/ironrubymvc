#region Usings

using System.Collections.Generic;
using System.Web.Mvc;
using IronRuby.Builtins;
using IronRubyMvcLibrary.Core;
using IronRubyMvcLibrary.Extensions;

#endregion

namespace IronRubyMvcLibrary.Controllers
{
    /// <summary>
    /// Implements <see cref="IActionFilter"/> and <see cref="IResultFilter"/>
    /// This class hooks the ruby defined filters with
    /// </summary>
    public class RubyActionFilter : IActionFilter, IResultFilter
    {
        public IEnumerable<string> OnlyForActions { get; set; }
        public IEnumerable<string> ExceptForActions { get; set; }
        public Proc BeforeAction { get; set; }
        public Proc AfterAction { get; set; }
        public Proc BeforeResult { get; set; }
        public Proc AfterResult { get; set; }
        public When FilterType { get; set; }

        #region Implementation of IActionFilter

        /// <summary>
        /// Called before the action executes
        /// </summary>
        /// <param name="filterContext">The filter context.</param>
        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (BeforeAction.IsNotNull() && CanExecute(filterContext))
                BeforeAction.Call(filterContext);
        }

        /// <summary>
        /// Called after an action executed
        /// </summary>
        /// <param name="filterContext">The filter context.</param>
        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (AfterAction.IsNotNull() && CanExecute(filterContext))
                AfterAction.Call(filterContext);
        }

        #endregion

        #region Implementation of IResultFilter

        public void OnResultExecuting(ResultExecutingContext filterContext)
        {
            if (BeforeResult.IsNotNull())
                BeforeResult.Call(filterContext);
        }

        public void OnResultExecuted(ResultExecutedContext filterContext)
        {
            if (AfterResult.IsNotNull())
                AfterResult.Call(filterContext);
        }

        #endregion

        #region Implementation of IRubyControllerFilter

        private bool CanExecute(ActionExecutingContext context)
        {
            var actionName = context.ActionDescriptor.ActionName;
            return CanExecute(actionName);
        }

        private bool CanExecute(ActionExecutedContext context)
        {
            var actionName = context.ActionDescriptor.ActionName;
            return CanExecute(actionName);
        }

        private bool CanExecute(string actionName)
        {
            return OnlyForActions.IsNull() || OnlyForActions.IsEmpty() || OnlyForActions.Contains(actionName)
                   || ExceptForActions.IsNull() || ExceptForActions.IsEmpty() || ExceptForActions.DoesNotContain(actionName);
        }

        #endregion
    }
}