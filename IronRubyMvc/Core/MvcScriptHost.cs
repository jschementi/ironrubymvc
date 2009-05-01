using Microsoft.Scripting.Hosting;

namespace System.Web.Mvc.IronRuby.Core
{
    public class MvcScriptHost : ScriptHost
    {
        public MvcScriptHost(){}

        public override Microsoft.Scripting.PlatformAdaptationLayer PlatformAdaptationLayer
        {
            get
            {
                return PathProviderPal.PAL;
            }
        }
    }
}