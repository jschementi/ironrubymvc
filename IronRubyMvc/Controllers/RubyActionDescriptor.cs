using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web.Mvc;
using IronRubyMvc.Core;
using IronRubyMvc.Helpers;

namespace IronRubyMvc
{
    public class RubyActionDescriptor : ActionDescriptor
    {
        private readonly MethodInfo _methodInfo;
        private readonly string _actionName;
        private readonly ControllerDescriptor _controllerDescriptor;
        private ParameterDescriptor[] _parametersCache;

        public RubyActionDescriptor(string actionName, ControllerDescriptor controllerDescriptor)
        {
            _actionName = actionName;
            _controllerDescriptor = controllerDescriptor;
            _methodInfo = RubyController.InvokeActionMethod;
        }


        public MethodInfo MethodInfo
        {
            get { return _methodInfo; }
        }

        public override object Execute(ControllerContext controllerContext, IDictionary<string, object> parameters)
        {
            return Action();
        }

        public override ParameterDescriptor[] GetParameters()
        {
            var parameters = LazilyFetchParametersCollection();

            // need to clone array so that user modifications aren't accidentally stored
            return (ParameterDescriptor[])parameters.Clone();
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

        private RubyMvcEngine Engine
        {
            get { return RubyControllerDescriptor.Engine; }
        }

        public Func<object> Action
        {
            get
            {
                var controllerRubyMethodName = Engine.GetMethodName(ActionName, RubyControllerDescriptor.RubyControllerClass);


                return String.IsNullOrEmpty(controllerRubyMethodName) 
                    ? null 
                    : Engine.GetControllerAction(RubyControllerDescriptor.ControllerName, controllerRubyMethodName);
            }
        }

        private ParameterDescriptor[] LazilyFetchParametersCollection()
        {
            return DescriptorUtil.LazilyFetchOrCreateDescriptors<ParameterInfo, ParameterDescriptor>(
                ref _parametersCache /* cacheLocation */,
                MethodInfo.GetParameters /* initializer */,
                parameterInfo => new RubyParameterDescriptor(parameterInfo, this) /* converter */);
        }


        internal static RubyActionDescriptor Create(string actionName, ControllerDescriptor controllerDescriptor)
        {
            return new RubyActionDescriptor(actionName, controllerDescriptor);
        }
    }
}