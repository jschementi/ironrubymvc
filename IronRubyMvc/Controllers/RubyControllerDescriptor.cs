using System;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using IronRuby.Builtins;
using IronRubyMvc.Core;

namespace IronRubyMvc
{
    public class RubyControllerDescriptor : ControllerDescriptor
    {
        private readonly string _controllerName;
        private RubyMediator _rubyMediator;
        public RubyControllerDescriptor(string controllerName)
        {
            _controllerName = controllerName;
        }

        public override string ControllerName
        {
            get { return _controllerName; }
        }

        internal RubyMediator RubyMediator
        {
            get
            {
                if (_rubyMediator.IsNull())
                    _rubyMediator = RubyMediator.Create();
                return _rubyMediator;
            }
        }

        internal void ResetRubyEngine()
        {
            _rubyMediator = RubyMediator.Create();
        }

        public override ActionDescriptor FindAction(ControllerContext controllerContext, string actionName)
        {
            // For now limit action name to alphanumeric characters
            if (!Regex.IsMatch(actionName, Constants.ACTION_NAME_REGEX))
                return null;

            RubyMediator.LoadController(ControllerName, controllerContext);


            var controllerRubyClassName = RubyMediator.GetGlobalVariableName(ControllerName);

            if (String.IsNullOrEmpty(controllerRubyClassName))
            {
                // controller not found
                return null;
            }

            //_rubyMediator.UseFile()
            RubyControllerClass = RubyMediator.GetRubyClass(controllerRubyClassName);
            

            return RubyActionDescriptor.Create(actionName, this);
        }

        public override ActionDescriptor[] GetCanonicalActions()
        {
            return new RubyActionDescriptor[0];
        }

        public override Type ControllerType
        {
            get { return typeof (RubyController); }
        }

        public RubyClass RubyControllerClass
        { get; private set; }
    }
}