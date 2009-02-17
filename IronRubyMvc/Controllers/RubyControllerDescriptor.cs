#region Usings

using System;
using System.Web.Mvc;
using IronRuby.Builtins;
using IronRubyMvcLibrary.Core;
using IronRubyMvcLibrary.Extensions;

#endregion

namespace IronRubyMvcLibrary.Controllers
{
    public class RubyControllerDescriptor : ControllerDescriptor
    {

        public RubyControllerDescriptor(RubyClass rubyClass)
        {
            RubyControllerClass = rubyClass;
        }

        public override string ControllerName
        {
            get { return RubyControllerClass.Name; }
        }

        internal IRubyEngine RubyEngine { get; set; }

        public override Type ControllerType
        {
            get { return typeof (RubyController); }
        }

        public RubyClass RubyControllerClass { get; private set; }

        public override ActionDescriptor FindAction(ControllerContext controllerContext, string actionName)
        {
            ((RubyEngine) RubyEngine).Operations.GetMemberNames(controllerContext.Controller);
            var hasControllerAction = RubyEngine.HasControllerAction((RubyController) controllerContext.Controller, actionName);

            return !hasControllerAction ? null : new RubyActionDescriptor(actionName, this);
        }

        public override ActionDescriptor[] GetCanonicalActions()
        {
            return new RubyActionDescriptor[0];
        }
    }
}