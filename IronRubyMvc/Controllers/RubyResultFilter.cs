using System.Web.Mvc;
using IronRuby.Builtins;
using IronRubyMvcLibrary.Extensions;

namespace IronRubyMvcLibrary.Controllers
{
    public class RubyResultFilter : IResultFilter
    {
        public Proc BeforeResult { get; set; }
        public Proc AfterResult { get; set; }

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
    }
}