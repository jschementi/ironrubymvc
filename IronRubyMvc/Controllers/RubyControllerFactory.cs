#region Usings

using System.Text;
using System.Web.Mvc.IronRuby.Core;
using System.Web.Mvc.IronRuby.Extensions;
using System.Web.Routing;
using IronRuby.Builtins;

#endregion

namespace System.Web.Mvc.IronRuby.Controllers
{
    public class RubyControllerFactory : IControllerFactory
    {
        private readonly IRubyEngine _engine;
        private readonly IPathProvider _pathProvider;
        private readonly IControllerFactory _innerFactory;

        internal RubyControllerFactory(IPathProvider pathProvider, IControllerFactory innerFactory, IRubyEngine engine)
        {
            _pathProvider = pathProvider;
            _innerFactory = innerFactory;
            _engine = engine;
        }

        #region IControllerFactory Members

        public IController CreateController(RequestContext requestContext, string controllerName)
        {
            try
            {
                return _innerFactory.CreateController(requestContext, controllerName);
            }
            catch (InvalidOperationException)
            {
            }
            catch (HttpException)
            {
            }

            return LoadController(requestContext, controllerName);
        }


        public void ReleaseController(IController controller)
        {
            var disposable = controller as IDisposable;

            if (disposable != null)
                disposable.Dispose();
        }

        #endregion

        /// <summary>
        /// Loads the controller.
        /// </summary>
        /// <param name="requestContext">The request context.</param>
        /// <param name="controllerName">Name of the controller.</param>
        /// <returns></returns>
        private RubyController LoadController(RequestContext requestContext, string controllerName)
        {
            var controllerFilePath = GetControllerFilePath(controllerName);
            var controllerClassName = GetControllerClassName(controllerName);

            _engine.RemoveClassFromGlobals(controllerClassName);

            if (controllerFilePath.IsNullOrBlank())
                return null;

            _engine.RequireRubyFile(_pathProvider.MapPath(controllerFilePath));

            var controllerClass = _engine.GetRubyClass(controllerClassName);
            var controller = ConfigureController(controllerClass, requestContext);

            return controller;
        }


        /// <summary>
        /// Configures the controller.
        /// </summary>
        /// <param name="rubyClass">The ruby class.</param>
        /// <param name="requestContext">The request context.</param>
        /// <returns></returns>
        private RubyController ConfigureController(RubyClass rubyClass, RequestContext requestContext)
        {
            var controller = _engine.CreateInstance<RubyController>(rubyClass);
            controller.InternalInitialize(new ControllerConfiguration { Context = requestContext, Engine = _engine, RubyClass = rubyClass });
            return controller;
        }

        /// <summary>
        /// Gets the name of the controller class.
        /// </summary>
        /// <param name="controllerName">Name of the controller.</param>
        /// <returns></returns>
        private static string GetControllerClassName(string controllerName)
        {
            return (controllerName.EndsWith("Controller", StringComparison.OrdinalIgnoreCase)
                        ? controllerName
                        : Constants.ControllerclassFormat.FormattedWith(controllerName)).Pascalize();
        }

        private string GetControllerFilePath(string controllerName)
        {
            var fileName = Constants.ControllerPascalPathFormat.FormattedWith(controllerName.Pascalize());
            if (_pathProvider.FileExists(fileName))
                return fileName;

            fileName = Constants.ControllerUnderscorePathFormat.FormattedWith(controllerName.Underscore());

            return _pathProvider.FileExists(fileName) ? fileName : string.Empty;
        }
    }
}