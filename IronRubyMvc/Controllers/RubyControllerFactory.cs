#region Usings

using System.Web.Mvc.IronRuby.Core;
using System.Web.Routing;

#endregion

namespace System.Web.Mvc.IronRuby.Controllers
{
    public class RubyControllerFactory : IControllerFactory
    {
        private readonly IRubyEngine _engine;
        private readonly IControllerFactory _innerFactory;

        internal RubyControllerFactory(IControllerFactory innerFactory, IRubyEngine engine)
        {
            _innerFactory = innerFactory;
            _engine = engine;
        }

        #region IControllerFactory Members

        public IController CreateController(RequestContext requestContext, string controllerName)
        {
            IController result;

            try
            {
                return _innerFactory.CreateController(requestContext, controllerName);
            }
            catch
            {
            }

            result = _engine.LoadController(requestContext, controllerName);

            return result;
        }

        public void ReleaseController(IController controller)
        {
            var disposable = controller as IDisposable;

            if (disposable != null)
                disposable.Dispose();
        }

        #endregion
    }
}