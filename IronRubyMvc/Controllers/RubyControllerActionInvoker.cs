#region Usings

using System.Web.Mvc;
using IronRubyMvcLibrary.Core;

#endregion

namespace IronRubyMvcLibrary.Controllers
{
    internal class RubyControllerActionInvoker : ControllerActionInvoker
    {
        public RubyControllerActionInvoker(string controllerName, RubyMediator mediator)
        {
            ControllerName = controllerName;
            RubyMediator = mediator;
        }

        public string ControllerName { get; private set; }

        public RubyMediator RubyMediator { get; private set; }

        protected override ControllerDescriptor GetControllerDescriptor(ControllerContext controllerContext)
        {
            return new RubyControllerDescriptor(((RubyController) controllerContext.Controller).RubyType,
                                                controllerContext) {RubyMediator = RubyMediator};
        }

        protected override ActionDescriptor FindAction(ControllerContext controllerContext,
                                                       ControllerDescriptor controllerDescriptor, string actionName)
        {
            return controllerDescriptor.FindAction(controllerContext, actionName);
        }
    }
}