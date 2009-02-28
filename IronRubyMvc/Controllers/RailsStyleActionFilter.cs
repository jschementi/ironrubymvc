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
    public class RailsStyleActionFilter : RubyActionFilter
    {
//        public IEnumerable<string> OnlyForActions { get; set; }
//        public IEnumerable<string> ExceptForActions { get; set; }
        public Proc BeforeAction { get; set; }
        public Proc AfterAction { get; set; }
       
        #region Implementation of IActionFilter

        /// <summary>
        /// Called before the action executes
        /// </summary>
        /// <param name="filterContext">The filter context.</param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (BeforeAction.IsNotNull())// && CanExecute(filterContext))
                BeforeAction.Call(filterContext);
        }

        /// <summary>
        /// Called after an action executed
        /// </summary>
        /// <param name="filterContext">The filter context.</param>
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (AfterAction.IsNotNull())// && CanExecute(filterContext))
                AfterAction.Call(filterContext);
        }

        #endregion

       

//        #region Implementation of IRubyControllerFilter
//
//        private bool CanExecute(ActionExecutingContext context)
//        {
//            var actionName = context.ActionDescriptor.ActionName;
//            return CanExecute(actionName);
//        }
//
//        private bool CanExecute(ActionExecutedContext context)
//        {
//            var actionName = context.ActionDescriptor.ActionName;
//            return CanExecute(actionName);
//        }
//
//        private bool CanExecute(string actionName)
//        {
//            return (OnlyForActions.IsNull() || OnlyForActions.IsEmpty() || OnlyForActions.Contains(actionName))
//                   || ExceptForActions.IsNull() || ExceptForActions.IsEmpty() || ExceptForActions.DoesNotContain(actionName);
//        }
//
//        #endregion
    }
}