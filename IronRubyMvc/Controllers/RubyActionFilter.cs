using System.Web.Mvc;

namespace IronRubyMvcLibrary.Controllers
{
    public abstract class RubyActionFilter : IActionFilter
    {
        #region Implementation of IActionFilter

        public abstract void OnActionExecuting(ActionExecutingContext filterContext);
        public abstract void OnActionExecuted(ActionExecutedContext filterContext);

        #endregion

        
    }
}