#region Usings

using System.Web.Routing;
using IronRuby.Builtins;
using IronRubyMvcLibrary.Core;

#endregion

namespace IronRubyMvcLibrary.Controllers
{
    internal class ControllerConfiguration
    {
        public RequestContext Context { get; set; }
        public RubyClass RubyClass { get; set; }
        public RubyEngine Engine { get; set; }
    }
}