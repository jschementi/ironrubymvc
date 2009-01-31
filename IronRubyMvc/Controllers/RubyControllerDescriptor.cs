using System;
using System.Web.Mvc;
using IronRuby.Builtins;
using Microsoft.Scripting.Hosting;

namespace IronRubyMvc
{
    public class RubyControllerDescriptor : ControllerDescriptor
    {
        private readonly ScriptRuntime _scriptRuntime;

        public RubyControllerDescriptor(ScriptRuntime scriptRuntime)
        {
            _scriptRuntime = scriptRuntime;
        }

        public override ActionDescriptor FindAction(ControllerContext controllerContext, string actionName)
        {
            throw new NotImplementedError();
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