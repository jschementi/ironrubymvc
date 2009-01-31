using System;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Web.Routing;
using IronRuby.Builtins;
using IronRubyMvc.Core;

namespace IronRubyMvc
{
    internal class RubyControllerActionInvoker : ControllerActionInvoker
    {
        private Func<object> _action;

        public RubyControllerActionInvoker(string controllerName)
            : this(RubyEngineFactory.Create(), controllerName)
        {
        }

        public RubyControllerActionInvoker(RubyMvcEngine engine, string controllerName)
        {
            
            ControllerName = controllerName;
            Engine = engine;
        }

        public string ControllerName { get; private set; }

        public RubyMvcEngine Engine { get; private set; }

        protected override ActionDescriptor FindAction(ControllerContext controllerContext,
                                                       ControllerDescriptor controllerDescriptor, string actionName)
        {
            // For now limit action name to alphanumeric characters
            if (!Regex.IsMatch(actionName, @"^(\w)+$"))
                return null;

            Engine.LoadController(ControllerName);

            Engine.DefineReadOnlyGlobalVariable("request_context", controllerContext.RequestContext);
            Engine.DefineReadOnlyGlobalVariable("script_runtime", Engine);

            var controllerRubyClassName = Engine.GetVariableName(ControllerName);

            if (String.IsNullOrEmpty(controllerRubyClassName))
            {
                // controller not found
                return null;
            }

            //this.Engine.UseFile()
            var controllerRubyClass = Engine.GetRubyClass(controllerRubyClassName);
            var controllerRubyMethodName = Engine.GetMethodName(actionName, controllerRubyClass);


            if (String.IsNullOrEmpty(controllerRubyMethodName))
            {
                // action not found
                return null;
            }

            _action = Engine.GetControllerAction(controllerRubyClassName, controllerRubyMethodName);

            return new RubyActionDescriptor(RubyController.InvokeActionMethod, actionName, controllerDescriptor);
        }


        protected override object GetParameterValue(ControllerContext controllerContext,
                                                    ParameterDescriptor parameterDescriptor)
        {
            if (parameterDescriptor.ParameterName == "__action")
            {
                ((RubyParameterDescriptor) parameterDescriptor).SetParameterType(_action.GetType());
                return _action;
            }
            return base.GetParameterValue(controllerContext, parameterDescriptor);
        }


        private static string PascalCaseIt(string s)
        {
            return s[0].ToString().ToUpper() + s.Substring(1);
        }
    }
}