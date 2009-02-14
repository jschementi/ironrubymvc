#region Usings

using System.Collections.Generic;
using System.Web.Mvc;

#endregion

namespace IronRubyMvcLibrary.Controllers
{
    public class RubyActionDescriptor : ActionDescriptor
    {
        private readonly string _actionName;
        private readonly ControllerDescriptor _controllerDescriptor;
//        private readonly MethodInfo _methodInfo;
//        private ParameterDescriptor[] _parametersCache;

        public RubyActionDescriptor(string actionName, ControllerDescriptor controllerDescriptor)
        {
            _actionName = actionName;
            _controllerDescriptor = controllerDescriptor;
//            Action = action;
//            _methodInfo = RubyController.InvokeActionMethod;
        }


//        public MethodInfo MethodInfo
//        {
//            get { return _methodInfo; }
//        }

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


//        public Func<object> Action
//        {
//            get; private set;
//        }

        public override object Execute(ControllerContext controllerContext, IDictionary<string, object> parameters)
        {
            return RubyControllerDescriptor.RubyEngine.CallMethod(controllerContext.Controller, ActionName);
        }

        public override ParameterDescriptor[] GetParameters() //return an empty array for now
        {
            var parameters = new ParameterDescriptor[0]; //LazilyFetchParametersCollection();

            // need to clone array so that user modifications aren't accidentally stored
            return (ParameterDescriptor[]) parameters.Clone();
        }

//        private ParameterDescriptor[] LazilyFetchParametersCollection()
//        {
//            return DescriptorUtil.LazilyFetchOrCreateDescriptors<ParameterInfo, ParameterDescriptor>(
//                ref _parametersCache /* cacheLocation */,
//                MethodInfo.GetParameters /* initializer */,
//                parameterInfo => new RubyParameterDescriptor(parameterInfo, this) /* converter */);
//        }


        internal static RubyActionDescriptor Create(string actionName, ControllerDescriptor controllerDescriptor)
        {
            return new RubyActionDescriptor(actionName, controllerDescriptor);
        }
    }
}