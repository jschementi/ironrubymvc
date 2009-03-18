#region Usings

using System.Web.Mvc.IronRuby.Core;
using System.Web.Mvc.IronRuby.Extensions;
using IronRuby.Builtins;

#endregion

namespace System.Web.Mvc.IronRuby.Controllers
{
    /// <summary>
    /// Takes care of invoking controller actions, their filters and returning the result.
    /// </summary>
    public class RubyControllerActionInvoker : ControllerActionInvoker
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RubyControllerActionInvoker"/> class.
        /// </summary>
        /// <param name="controllerName">Name of the controller.</param>
        /// <param name="engine">The engine.</param>
        public RubyControllerActionInvoker(string controllerName, IRubyEngine engine)
        {
            ControllerName = controllerName;
            RubyEngine = engine;
        }

        /// <summary>
        /// Gets the name of the controller.
        /// </summary>
        /// <value>The name of the controller.</value>
        internal string ControllerName { get; private set; }

        /// <summary>
        /// Gets the ruby engine.
        /// </summary>
        /// <value>The ruby engine.</value>
        internal IRubyEngine RubyEngine { get; private set; }

        protected override ControllerDescriptor GetControllerDescriptor(ControllerContext controllerContext)
        {
            var rubyController = (RubyController) controllerContext.Controller;
            return new RubyControllerDescriptor(rubyController.RubyType, RubyEngine);
        }

        protected override ActionDescriptor FindAction(ControllerContext controllerContext,
                                                       ControllerDescriptor controllerDescriptor, string actionName)
        {
            return controllerDescriptor.FindAction(controllerContext, actionName);
        }

        protected override FilterInfo GetFilters(ControllerContext controllerContext, ActionDescriptor actionDescriptor)
        {
            var rubyType = ((RubyController) controllerContext.Controller).RubyType;
            var controllerFilters = (Hash) RubyEngine.CallMethod(rubyType, "action_filters");

            var info = controllerFilters.ToFilterInfo("controller").MergedWith(actionDescriptor.GetFilters());
            return info;
        }
    }
}