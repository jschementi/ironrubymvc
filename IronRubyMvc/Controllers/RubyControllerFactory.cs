#region Usings

using System;
using System.Web.Mvc;
using System.Web.Routing;
using IronRubyMvcLibrary.Core;

#endregion

namespace IronRubyMvcLibrary.Controllers
{
    public class RubyControllerFactory : IControllerFactory
    {
        private readonly IControllerFactory _innerFactory;

        public RubyControllerFactory(IControllerFactory innerFactory)
        {
            _innerFactory = innerFactory;
        }

        #region IControllerFactory Members

        public IController CreateController(RequestContext context, string controllerName)
        {
            IController result = null;

            try
            {
                result = _innerFactory.CreateController(context, controllerName);
            }
            catch
            {
            }

            RubyMediator mediator = RubyMediator.Create();
            result = mediator.LoadController(context, controllerName);

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