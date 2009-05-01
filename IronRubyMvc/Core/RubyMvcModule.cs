using System.Web.Mvc.IronRuby.Extensions;

namespace System.Web.Mvc.IronRuby.Core
{
    public class RubyMvcModule : IHttpModule
    {

        private IRubyEngine _rubyEngine;
        private MvcApplication _mvcApplication;
        private HttpApplication _application;

        #region Implementation of IHttpModule

        /// <summary>
        /// initializes a module and prepares it to handle requests.
        /// </summary>
        /// <param name="context">
        ///  An <see cref="T:System.Web.HttpApplication" /> that provides access to the methods, properties, and events common to all application objects within an ASP.NET application 
        /// </param>
        public void Init(HttpApplication context)
        {
            context.EnsureArgumentNotNull("context");

            if (!InitializeRubyEngine(context)) return;
            if (!InitializeRubyApplication(context)) return;
            InitializeRubyApplicationHooks(context);
            
            _application = context;
        }

        private void InitializeRubyApplicationHooks(HttpApplication context)
        {
            if (_mvcApplication.IsNull()) return;

            context.Error += (sender, args) => _mvcApplication.Error(sender, args);
            context.AcquireRequestState += (sender, args) => _mvcApplication.AcquireRequestState(sender, args);
            context.AuthenticateRequest += (sender, args) => _mvcApplication.AuthenticateRequest(sender, args);
            context.AuthorizeRequest += (sender, args) => _mvcApplication.AuthorizeRequest(sender, args);
            context.BeginRequest += (sender, args) => _mvcApplication.BeginRequest(sender, args);
            context.Disposed += (sender, args) => _mvcApplication.Disposed(sender, args);
            context.EndRequest += (sender, args) => _mvcApplication.EndRequest(sender, args);
            context.LogRequest += (sender, args) => _mvcApplication.LogRequest(sender, args);
            context.PostAcquireRequestState += (sender, args) => _mvcApplication.PostAcquireRequestState(sender, args);
            context.MapRequestHandler += (sender, args) => _mvcApplication.MapRequestHandler(sender, args);
            context.PostAuthenticateRequest += (sender, args) => _mvcApplication.PostAuthenticateRequest(sender, args);
            context.PostAuthorizeRequest += (sender, args) => _mvcApplication.PostAuthorizeRequest(sender, args);
            context.PostLogRequest += (sender, args) => _mvcApplication.PostLogRequest(sender, args);
            context.PostMapRequestHandler += (sender, args) => _mvcApplication.PostMapRequestHandler(sender, args);
            context.PostReleaseRequestState += (sender, args) => _mvcApplication.PostReleaseRequestState(sender, args);
            context.PostRequestHandlerExecute += (sender, args) => _mvcApplication.PostRequestHandlerExecute(sender, args);
            context.PostResolveRequestCache += (sender, args) => _mvcApplication.PostResolveRequestCache(sender, args);
            context.PostUpdateRequestCache += (sender, args) => _mvcApplication.PostUpdateRequestCache(sender, args);
            context.PreRequestHandlerExecute += (sender, args) => _mvcApplication.PreRequestHandlerExecute(sender, args);
            context.PreSendRequestContent += (sender, args) => _mvcApplication.PreSendRequestContent(sender, args);
            context.PreSendRequestHeaders += (sender, args) => _mvcApplication.PreSendRequestHeaders(sender, args);
            context.ReleaseRequestState += (sender, args) => _mvcApplication.ReleaseRequestState(sender, args);
            context.ResolveRequestCache += (sender, args) => _mvcApplication.ResolveRequestCache(sender, args);
            context.UpdateRequestCache += (sender, args) => _mvcApplication.UpdateRequestCache(sender, args);
           
        }

        private bool InitializeRubyApplication(HttpApplication context)
        {
            if (_mvcApplication.IsNotNull()) return false;

            _mvcApplication = _rubyEngine.ExecuteFile<MvcApplication>("~/mvc_application.rb",  false);
            if(_mvcApplication.IsNotNull()) _mvcApplication.Start(context, EventArgs.Empty);
            return true;
        }

        private bool InitializeRubyEngine(HttpApplication context)
        {
            var rubyMvcApp = context as RubyMvcApplication;
            if(rubyMvcApp.IsNull()) return false;
            
            if(rubyMvcApp.RubyEngine.IsNull())
                rubyMvcApp.RubyEngine = RubyEngine.InitializeIronRubyMvc(new VirtualPathProvider(), "~/routes.rb");

            if (_rubyEngine.IsNull())
                _rubyEngine = rubyMvcApp.RubyEngine;

            return true;
        }


        /// <summary>
        /// Disposes of the resources (other than memory) used by the module that implements <see cref="T:System.Web.IHttpModule" />.
        /// </summary>
        public void Dispose()
        {
            if (_application.IsNull()) return;

            _application.Error -= (sender, args) => _mvcApplication.Error(sender, args);
            _application.AcquireRequestState -= (sender, args) => _mvcApplication.AcquireRequestState(sender, args);
            _application.AuthenticateRequest -= (sender, args) => _mvcApplication.AuthenticateRequest(sender, args);
            _application.AuthorizeRequest -= (sender, args) => _mvcApplication.AuthorizeRequest(sender, args);
            _application.BeginRequest -= (sender, args) => _mvcApplication.BeginRequest(sender, args);
            _application.Disposed -= (sender, args) => _mvcApplication.Disposed(sender, args);
            _application.EndRequest -= (sender, args) => _mvcApplication.EndRequest(sender, args);
            _application.LogRequest -= (sender, args) => _mvcApplication.LogRequest(sender, args);
            _application.PostAcquireRequestState -= (sender, args) => _mvcApplication.PostAcquireRequestState(sender, args);
            _application.MapRequestHandler -= (sender, args) => _mvcApplication.MapRequestHandler(sender, args);
            _application.PostAuthenticateRequest -= (sender, args) => _mvcApplication.PostAuthenticateRequest(sender, args);
            _application.PostAuthorizeRequest -= (sender, args) => _mvcApplication.PostAuthorizeRequest(sender, args);
            _application.PostLogRequest -= (sender, args) => _mvcApplication.PostLogRequest(sender, args);
            _application.PostMapRequestHandler -= (sender, args) => _mvcApplication.PostMapRequestHandler(sender, args);
            _application.PostReleaseRequestState -= (sender, args) => _mvcApplication.PostReleaseRequestState(sender, args);
            _application.PostRequestHandlerExecute -= (sender, args) => _mvcApplication.PostRequestHandlerExecute(sender, args);
            _application.PostResolveRequestCache -= (sender, args) => _mvcApplication.PostResolveRequestCache(sender, args);
            _application.PostUpdateRequestCache -= (sender, args) => _mvcApplication.PostUpdateRequestCache(sender, args);
            _application.PreRequestHandlerExecute -= (sender, args) => _mvcApplication.PreRequestHandlerExecute(sender, args);
            _application.PreSendRequestContent -= (sender, args) => _mvcApplication.PreSendRequestContent(sender, args);
            _application.PreSendRequestHeaders -= (sender, args) => _mvcApplication.PreSendRequestHeaders(sender, args);
            _application.ReleaseRequestState -= (sender, args) => _mvcApplication.ReleaseRequestState(sender, args);
            _application.ResolveRequestCache -= (sender, args) => _mvcApplication.ResolveRequestCache(sender, args);
            _application.UpdateRequestCache -= (sender, args) => _mvcApplication.UpdateRequestCache(sender, args);
        }

        #endregion
    }
}