#region Usings

using System.Web.Mvc;
using IronRubyMvcLibrary.Core;

#endregion

namespace IronRubyMvcLibrary.Controllers
{
    internal class RubyControllerActionInvoker : ControllerActionInvoker
    {
        public RubyControllerActionInvoker(string controllerName, RubyEngine engine)
        {
            ControllerName = controllerName;
            RubyEngine = engine;
        }

        public string ControllerName { get; private set; }

        public RubyEngine RubyEngine { get; private set; }

        protected override ControllerDescriptor GetControllerDescriptor(ControllerContext controllerContext)
        {
            return new RubyControllerDescriptor(((RubyController) controllerContext.Controller).RubyType,
                                                controllerContext) {RubyEngine = RubyEngine};
        }

        protected override ActionDescriptor FindAction(ControllerContext controllerContext,
                                                       ControllerDescriptor controllerDescriptor, string actionName)
        {
            return controllerDescriptor.FindAction(controllerContext, actionName);
        }
    }
}