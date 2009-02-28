using System.Web.Mvc;

namespace IronRubyMvcLibrary.Controllers
{
    public abstract class RubyResultFilter : IResultFilter
    {
        #region Implementation of IResultFilter

        public abstract void OnResultExecuting(ResultExecutingContext filterContext);
        public abstract void OnResultExecuted(ResultExecutedContext filterContext);

        #endregion
    }
}