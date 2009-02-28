using System.Web.Mvc;
using IronRuby.Builtins;

namespace IronRubyMvcLibrary.Controllers
{
    public abstract class RubyActionFilter : IActionFilter
    {
        #region Implementation of IActionFilter

        public virtual void OnActionExecuting(ActionExecutingContext filterContext)
        {
            throw new NotImplementedError();
        }
        public virtual void OnActionExecuted(ActionExecutedContext filterContext)
        {
            throw new NotImplementedError();
        }

        #endregion

        
    }
}