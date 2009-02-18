#region Usings

using System.Web.Mvc;
using IronRuby.Builtins;
using IronRubyMvcLibrary.Extensions;

#endregion

namespace IronRubyMvcLibrary.Controllers
{
    public class RubyAuthorizationFilter : IAuthorizationFilter
    {
        public Proc Authorize { get; set; }

        #region Implementation of IAuthorizationFilter

        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if (Authorize.IsNotNull()) Authorize.Call(filterContext);
        }

        #endregion
    }
}