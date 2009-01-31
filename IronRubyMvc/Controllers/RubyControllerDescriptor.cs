using System;
using System.Web.Mvc;

namespace IronRubyMvc
{
    public class RubyControllerDescriptor : ControllerDescriptor
    {
        public override ActionDescriptor FindAction(ControllerContext controllerContext, string actionName)
        {
            throw new System.NotImplementedException();
        }

        public override ActionDescriptor[] GetCanonicalActions()
        {
            throw new System.NotImplementedException();
        }

        public override Type ControllerType
        {
            get { throw new System.NotImplementedException(); }
        }
    }
}