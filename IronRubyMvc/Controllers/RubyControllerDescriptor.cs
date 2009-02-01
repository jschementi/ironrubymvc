#region Usings

using System;
using System.Web.Mvc;
using IronRuby.Builtins;
using IronRubyMvcLibrary.Core;

#endregion

namespace IronRubyMvcLibrary.Controllers
{
    public class RubyControllerDescriptor : ControllerDescriptor
    {
        private readonly ControllerContext _context;

        public RubyControllerDescriptor(RubyClass rubyClass, ControllerContext context)
        {
            RubyControllerClass = rubyClass;
            _context = context;
        }

        public ControllerContext Context
        {
            get { return _context; }
        }

        public override string ControllerName
        {
            get { return RubyControllerClass.Name; }
        }

        internal RubyMediator RubyMediator { get; set; }

        public override Type ControllerType
        {
            get { return typeof (RubyController); }
        }

        public RubyClass RubyControllerClass { get; private set; }

        public override ActionDescriptor FindAction(ControllerContext controllerContext, string actionName)
        {
            Func<object> action = RubyMediator.GetControllerAction((RubyController) controllerContext.Controller,
                                                                   actionName);

            if (action.IsNull()) return null;

            return RubyActionDescriptor.Create(actionName, this);
        }

        public override ActionDescriptor[] GetCanonicalActions()
        {
            return new RubyActionDescriptor[0];
        }
    }
}