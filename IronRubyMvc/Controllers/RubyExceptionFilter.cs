using System.Web.Mvc;

namespace IronRubyMvcLibrary.Controllers
{
    public abstract class RubyExceptionFilter : IExceptionFilter
    {
        #region Implementation of IExceptionFilter

        public abstract void OnException(ExceptionContext filterContext);

        #endregion
    }
}