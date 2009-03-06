#region Usings



#endregion

namespace System.Web.Mvc.IronRuby.Core
{
    public class RubyMvcApplication : HttpApplication
    {
//        private bool _isFirstRequest;
//        private bool _hasBeenReset;

        protected void Application_Start(object sender, EventArgs e)
        {
            RubyEngine.InitializeIronRubyMvc(new VirtualPathProvider(), "~/routes.rb");
            OnStart();
        }

        protected virtual void OnStart()
        {
            // override this to provide start behavior
        }

    }
}