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
        private RubyMvcEngine _engine;
        public RubyControllerDescriptor(string controllerName)
        {
            _controllerName = controllerName;
        }

        public override string ControllerName
        {
            get { return _controllerName; }
        }

        internal RubyMvcEngine Engine
        {
            get
            {
                if (_engine.IsNull())
                    _engine = RubyMvcEngine.Create();
                return _engine;
            }
        }

        internal void ResetRubyEngine()
        {
            _engine = RubyMvcEngine.Create();
        }

        public override ActionDescriptor FindAction(ControllerContext controllerContext, string actionName)
        {
            // For now limit action name to alphanumeric characters
            if (!Regex.IsMatch(actionName, Constants.ACTION_NAME_REGEX))
                return null;

            Engine.LoadController(ControllerName, controllerContext);


            var controllerRubyClassName = Engine.GetGlobalVariableName(ControllerName);

            if (String.IsNullOrEmpty(controllerRubyClassName))
            {
                // controller not found
                return null;
            }

            //_engine.UseFile()
            RubyControllerClass = Engine.GetRubyClass(controllerRubyClassName);
            

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