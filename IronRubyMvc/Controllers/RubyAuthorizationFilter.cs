using System.Web.Mvc;

namespace IronRubyMvcLibrary.Controllers
{
    public abstract class RubyAuthorizationFilter : IAuthorizationFilter
    {
        #region Implementation of IAuthorizationFilter

        public abstract void OnAuthorization(AuthorizationContext filterContext);

        #endregion
    }
}