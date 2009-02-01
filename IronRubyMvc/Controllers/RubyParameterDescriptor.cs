#region Usings

using System;
using System.Reflection;
using System.Web.Mvc;

#endregion

namespace IronRubyMvcLibrary.Controllers
{
    public class RubyParameterDescriptor : ReflectedParameterDescriptor
    {
        private Type _parameterType;

        public RubyParameterDescriptor(ParameterInfo parameterInfo, ActionDescriptor actionDescriptor)
            : base(parameterInfo, actionDescriptor)
        {
        }


        public new Type ParameterType
        {
            get
            {
                if (_parameterType != null)
                    return _parameterType;
                return base.ParameterType;
            }
            internal set { _parameterType = value; }
        }
    }
}