#region Usings

using System.Web.Mvc.IronRuby.Core;
using System.Web.Mvc.IronRuby.Extensions;
using IronRuby.Builtins;

#endregion

namespace System.Web.Mvc.IronRuby.Controllers
{
    /// <summary>
    /// The descriptor for a Ruby enabled Controller
    /// </summary>
    public class RubyControllerDescriptor : ControllerDescriptor
    {
        private readonly IRubyEngine _engine;
        private readonly RubyActionMethodSelector _selector;

        /// <summary>
        /// Initializes a new instance of the <see cref="RubyControllerDescriptor"/> class.
        /// </summary>
        /// <param name="rubyClass">The ruby class.</param>
        /// <param name="engine">The engine.</param>
        public RubyControllerDescriptor(RubyClass rubyClass, IRubyEngine engine)
        {
            _engine = engine;
            RubyControllerClass = rubyClass;
            _selector = new RubyActionMethodSelector(engine, rubyClass);
        }

        /// <summary>
        /// Gets the name of the controller.
        /// </summary>
        /// <value>The name of the controller.</value>
        public override string ControllerName
        {
            get { return RubyControllerClass.Name; }
        }


        /// <summary>
        /// Gets the type of the controller.
        /// </summary>
        /// <value>The type of the controller.</value>
        public override Type ControllerType
        {
            get { return typeof (RubyController); }
        }

        /// <summary>
        /// Gets or sets the class ruby controller.
        /// </summary>
        /// <value>The ruby controller class.</value>
        public RubyClass RubyControllerClass { get; private set; }

        /// <summary>
        /// Finds the action.
        /// </summary>
        /// <param name="controllerContext">The controller context.</param>
        /// <param name="actionName">The name of the action.</param>
        /// <returns>The information about the action.</returns>
        public override ActionDescriptor FindAction(ControllerContext controllerContext, string actionName)
        {
            controllerContext.EnsureArgumentNotNull("controllerContext");
            actionName.EnsureArgumentNotNull("actionName");

            var selectedName = _selector.FindActionMethod(controllerContext, actionName);
            return selectedName.IsNotNullOrBlank() ?  new RubyActionDescriptor(selectedName, this, _engine ) : null;
        }

        /// <summary>
        /// Gets the canonical actions.
        /// </summary>
        /// <returns>
        /// A list of action descriptors for the controller.
        /// </returns>
        public override ActionDescriptor[] GetCanonicalActions()
        {
            return _selector.GetAllActionMethods().Map(method => new RubyActionDescriptor(method, this, _engine)).ToArray();
        }
    }
}