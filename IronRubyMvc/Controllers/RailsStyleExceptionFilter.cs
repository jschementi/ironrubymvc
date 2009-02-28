#region Usings

using System.Web.Mvc;
using IronRuby.Builtins;
using IronRubyMvcLibrary.Extensions;

#endregion

namespace IronRubyMvcLibrary.Controllers
{
    public class RailsStyleExceptionFilter : IExceptionFilter
    {
        public Proc Error { get; set; }

        #region Implementation of IExceptionFilter

        public void OnException(ExceptionContext filterContext)
        {
            if (Error.IsNotNull()) Error.Call(filterContext);
        }

        #endregion
    }
}