namespace System.Web.Mvc.IronRuby.Controllers
{
    public abstract class RubyExceptionFilter : IExceptionFilter
    {
        #region Implementation of IExceptionFilter

        public abstract void OnException(ExceptionContext filterContext);

        #endregion
    }
}