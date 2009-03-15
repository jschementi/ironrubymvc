namespace System.Web.Mvc.IronRuby.Controllers
{
    public abstract class RubyActionSelector
    {
        public abstract bool IsValidForRequest(ControllerContext controllerContext);
    }
}