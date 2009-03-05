#region Usings

using System.Web.Mvc.IronRuby.Core;
using System.Web.Routing;
using IronRuby.Builtins;

#endregion

namespace System.Web.Mvc.IronRuby.Controllers
{
    internal class ControllerConfiguration
    {
        public RequestContext Context { get; set; }
        public RubyClass RubyClass { get; set; }
        public IRubyEngine Engine { get; set; }
    }
}