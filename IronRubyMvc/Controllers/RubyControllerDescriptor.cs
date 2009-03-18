#region Usings

using System.Web.Mvc.IronRuby.Core;
using System.Web.Mvc.IronRuby.Extensions;
using IronRuby.Builtins;

#endregion

namespace System.Web.Mvc.IronRuby.Controllers
{
    public class RubyControllerDescriptor : ControllerDescriptor
    {
        private readonly RubyActionMethodSelector _selector;
        public RubyControllerDescriptor(RubyClass rubyClass, IRubyEngine engine)
        {
            RubyControllerClass = rubyClass;
            _selector = new RubyActionMethodSelector(engine, rubyClass);
        }

        public override string ControllerName
        {
            get { return RubyControllerClass.Name; }
        }


        public override Type ControllerType
        {
            get { return typeof (RubyController); }
        }

        public RubyClass RubyControllerClass { get; private set; }

        public override ActionDescriptor FindAction(ControllerContext controllerContext, string actionName)
        {
            controllerContext.EnsureArgumentNotNull("controllerContext");
            actionName.EnsureArgumentNotNull("actionName");

            var selectedName = _selector.FindActionMethod(controllerContext, actionName);
            return selectedName.IsNotNullOrBlank() ?  new RubyActionDescriptor(actionName, this) : null;
        }

        public override ActionDescriptor[] GetCanonicalActions()
        {
            return _selector.GetAllActionMethods().Map(method => new RubyActionDescriptor(method, this)).ToArray();
        }
    }
}