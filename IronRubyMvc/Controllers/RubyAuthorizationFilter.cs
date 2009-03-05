namespace System.Web.Mvc.IronRuby.Controllers
{
    public abstract class RubyAuthorizationFilter : IAuthorizationFilter
    {
        #region Implementation of IAuthorizationFilter

        public abstract void OnAuthorization(AuthorizationContext filterContext);

        #endregion
    }
}