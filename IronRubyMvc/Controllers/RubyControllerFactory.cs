#region Usings

using System;
using System.Web.Mvc;
using System.Web.Routing;
using IronRubyMvcLibrary.Core;
using Microsoft.Scripting.Hosting;

#endregion

namespace IronRubyMvcLibrary.Controllers
{
    public class RubyControllerFactory : IControllerFactory
    {
        private readonly IControllerFactory _innerFactory;
        private readonly IRubyEngine _engine;

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

//            RubyEngine engine = RubyEngine.Create(_engine);
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