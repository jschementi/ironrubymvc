#region Usings

using System.Web.Mvc;
using IronRubyMvcLibrary.Core;

#endregion

namespace IronRubyMvcLibrary.Controllers
{
    public class RubyControllerActionInvoker : ControllerActionInvoker
    {
        public RubyControllerActionInvoker(string controllerName, IRubyEngine engine)
        {
            ControllerName = controllerName;
            RubyEngine = engine;
        }

        public string ControllerName { get; private set; }

        public IRubyEngine RubyEngine { get; private set; }

        protected override ControllerDescriptor GetControllerDescriptor(ControllerContext controllerContext)
        {
            var rubyController = (RubyController) controllerContext.Controller;
            return new RubyControllerDescriptor(rubyController.RubyType) {RubyEngine = RubyEngine};
        }

        protected override ActionDescriptor FindAction(ControllerContext controllerContext,
                                                       ControllerDescriptor controllerDescriptor, string actionName)
        {
            return controllerDescriptor.FindAction(controllerContext, actionName);
        }
        
    }
}