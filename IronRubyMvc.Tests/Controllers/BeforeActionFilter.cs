using System;
using System.Web.Mvc;

namespace IronRubyMvcLibrary.Tests.Controllers
{
    public class BeforeActionFilter : DelegateActionFilter
    {
        public BeforeActionFilter(Action<ActionExecutingContext> onActionExecuting) : base(onActionExecuting, null)
        {
        }
    }

    
}