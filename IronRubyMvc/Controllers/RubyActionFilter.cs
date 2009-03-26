#region Usings

#endregion

namespace System.Web.Mvc.IronRuby.Controllers
{
    public abstract class RubyActionFilter : IActionFilter
    {
        #region Implementation of IActionFilter

        public virtual void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // Intentionally left blank to allow for a better overriding experience
        }

        public virtual void OnActionExecuted(ActionExecutedContext filterContext)
        {
            // Intentionally left blank to allow for a better overriding experience
        }

        #endregion
    }
}