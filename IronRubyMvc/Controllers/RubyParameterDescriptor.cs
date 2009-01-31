using System;
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

        public Func<object> Action
        {
            get
            {
                var rubyActionDescriptor = ActionDescriptor as RubyActionDescriptor;
                if (rubyActionDescriptor.IsNull())
                    return null;
                
                return rubyActionDescriptor.Action;
            }
        }

        public new System.Type ParameterType
        {
            get
            {
                if (_parameterType != null)
                    return _parameterType;
                return base.ParameterType;
            }
            internal set{ _parameterType = value;}
        }


    }
}