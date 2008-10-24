namespace IronRubyMvc {
    using System;
    using System.Text.RegularExpressions;
    using System.Web.Mvc;
    using System.Web.Routing;

    public class RubyControllerFactory : IControllerFactory {
        IControllerFactory _innerFactory;

        public RubyControllerFactory(IControllerFactory innerFactory) {
            _innerFactory = innerFactory;
        }

        public IController CreateController(RequestContext context, string controllerName) {
            IController result = null;

            try {
                result = _innerFactory.CreateController(context, controllerName);
            }
            catch (Exception) { }

            if (result == null && Regex.IsMatch(controllerName, @"^(\w)+$"))  // Limit to alphanum characters for now
                result = new RubyController { ControllerName = controllerName };

            return result;
        }

        public void ReleaseController(IController controller) {
            IDisposable disposable = controller as IDisposable;

            if (disposable != null)
                disposable.Dispose();
        }
    }
}