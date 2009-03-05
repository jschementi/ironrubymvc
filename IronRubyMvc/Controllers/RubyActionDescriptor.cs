#region Usings

using System.Collections.Generic;
using System.Web.Mvc.IronRuby.Extensions;
using IronRuby.Builtins;

#endregion

namespace System.Web.Mvc.IronRuby.Controllers
{
    public class RubyActionDescriptor : ActionDescriptor
    {
        private readonly string _actionName;
        private readonly ControllerDescriptor _controllerDescriptor;

        public RubyActionDescriptor(string actionName, ControllerDescriptor controllerDescriptor)
        {
            _actionName = actionName;
            _controllerDescriptor = controllerDescriptor;
        }

        public override string ActionName
        {
            get { return _actionName; }
        }

        public override ControllerDescriptor ControllerDescriptor
        {
            get { return _controllerDescriptor; }
        }


        private RubyControllerDescriptor RubyControllerDescriptor
        {
            get { return ((RubyControllerDescriptor) ControllerDescriptor); }
        }

        public override ParameterDescriptor[] GetParameters()
        {
            return new ParameterDescriptor[0];
        }

        public override object Execute(ControllerContext controllerContext, IDictionary<string, object> parameters)
        {
            var engine = RubyControllerDescriptor.RubyEngine;
            return engine.CallMethod(controllerContext.Controller, ActionName);
        }

        public override ICollection<ActionSelector> GetSelectors()
        {
//            var selectors = RubyControllerDescriptor.RubyEngine.CallMethod(
//                RubyControllerDescriptor.RubyControllerClass, "action_selectors");
            return new ActionSelector[0];
        }

        public override FilterInfo GetFilters()
        {
            var filters = (Hash) RubyControllerDescriptor.RubyEngine.CallMethod(RubyControllerDescriptor.RubyControllerClass, "action_filters");

            var info = filters.ToFilterInfo(ActionName);
            return info;
        }

//        internal static RubyActionDescriptor Create(string actionName, ControllerDescriptor controllerDescriptor)
//        {
//            return new RubyActionDescriptor(actionName, controllerDescriptor);
//        }
    }
}