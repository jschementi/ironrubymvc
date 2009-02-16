using System;
using System.Web.Mvc;

namespace IronRubyMvcLibrary.Tests.Controllers
{
    public class AfterActionFilter : DelegateActionFilter
    {
        public AfterActionFilter(Action<ActionExecutedContext> onActionExecuted)
            : base(null, onActionExecuted)
        {
        }
    }
}