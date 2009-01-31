using System.Reflection;
using System.Web.Mvc;

namespace IronRubyMvc
{
    public class RubyParameterDescriptor : ReflectedParameterDescriptor
    {
        private System.Type _parameterType;
        public RubyParameterDescriptor(ParameterInfo parameterInfo, ActionDescriptor actionDescriptor) : base(parameterInfo, actionDescriptor)
        {
        }

        public override System.Type ParameterType
        {
            get
            {
                if (_parameterType != null)
                    return _parameterType;
                return base.ParameterType;
            }
        }

        public void SetParameterType(System.Type parameterType)
        {
            _parameterType = parameterType;
        }
    }
}