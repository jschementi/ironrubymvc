using System;
using System.Web.Mvc;

namespace IronRubyMvc.Core
{
    internal enum ReaderType
    {
        File,
        AssemblyResource
    }


    internal class RubyEngineFactory
    {
        public static RubyMvcEngine Create()
        {
            var rubyEngine = new RubyMvcEngine();

            foreach (Type type in new[] {typeof (object), typeof (Uri), typeof (Controller), typeof (RubyController)})
                rubyEngine.LoadAssembly(type.Assembly);

            rubyEngine.ExecuteScript("Controller = IronRubyMvc::RubyController");

            return rubyEngine;
        }
    }
}