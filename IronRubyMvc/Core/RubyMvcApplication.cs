#region Usings



#endregion

namespace System.Web.Mvc.IronRuby.Core
{
    public class RubyMvcApplication : HttpApplication
    {
//        private bool _isFirstRequest;
//        private bool _hasBeenReset;

        protected virtual void Application_Start(object sender, EventArgs e)
        {
            RubyEngine.InitializeIronRubyMvc(new VirtualPathProvider(), "~/routes.rb");
        }

//        protected virtual void Application_BeginRequest(object sender, EventArgs e)
//        {
//            var rubyEngine = Application["___RubyEngine"] as IRubyEngine;
//            if(rubyEngine.IsNotNull() && !_isFirstRequest && !_hasBeenReset)
//            {
//                rubyEngine.ResetEngine();
//                _hasBeenReset = true;
//            }
//        }
    }
}