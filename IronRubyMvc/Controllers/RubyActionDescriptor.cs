using System.Reflection;
using System.Web.Mvc;
using IronRubyMvc.Helpers;

namespace IronRubyMvc
{
    public class RubyActionDescriptor : ReflectedActionDescriptor
    {
        private ParameterDescriptor[] _parametersCache;

        public RubyActionDescriptor(MethodInfo methodInfo, string actionName, ControllerDescriptor controllerDescriptor) : base(methodInfo, actionName, controllerDescriptor)
        {
        }

        public override ParameterDescriptor[] GetParameters()
        {
            ParameterDescriptor[] parameters = LazilyFetchParametersCollection();

            // need to clone array so that user modifications aren't accidentally stored
            return (ParameterDescriptor[])parameters.Clone();
        }

        private ParameterDescriptor[] LazilyFetchParametersCollection()
        {
            return DescriptorUtil.LazilyFetchOrCreateDescriptors<ParameterInfo, ParameterDescriptor>(
                ref _parametersCache /* cacheLocation */,
                MethodInfo.GetParameters /* initializer */,
                parameterInfo => new RubyParameterDescriptor(parameterInfo, this) /* converter */);
        }
    }
}