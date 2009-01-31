using IronRubyMvc.Core;

namespace IronRubyMvc {
    using System;
    using System.Text.RegularExpressions;
    using System.Web.Mvc;
    using System.Web.Routing;

    public class RubyControllerFactory : IControllerFactory {
        readonly IControllerFactory _innerFactory;

        public RubyControllerFactory(IControllerFactory innerFactory) {
            _innerFactory = innerFactory;
        }

        public IController CreateController(RequestContext context, string controllerName) {
            IController result = null;

            try {
                result = _innerFactory.CreateController(context, controllerName);
            }
            catch { }

            if (result == null && Regex.IsMatch(controllerName, Constants.CONTROLLERNAME_NAME_REGEX))  // Limit to alphanum characters for now
                result = new RubyController { ControllerName = controllerName };

            return result;
        }

        public void ReleaseController(IController controller) {
            var disposable = controller as IDisposable;

            if (disposable != null)
                disposable.Dispose();
        }
    }
}