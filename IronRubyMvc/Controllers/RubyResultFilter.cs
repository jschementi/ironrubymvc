#region Usings

using System.Web.Mvc;

#endregion

namespace IronRubyMvcLibrary.Controllers
{
    public abstract class RubyResultFilter : IResultFilter
    {
        #region Implementation of IResultFilter

        public virtual void OnResultExecuting(ResultExecutingContext filterContext)
        {
            // Intentionally left blank to allow for a better overriding experience
        }

        public virtual void OnResultExecuted(ResultExecutedContext filterContext)
        {
            // Intentionally left blank to allow for a better overriding experience
        }

        #endregion
    }
}