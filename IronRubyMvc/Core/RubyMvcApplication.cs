#region Usings



#endregion

namespace System.Web.Mvc.IronRuby.Core
{
    public class RubyMvcApplication : HttpApplication
    {


        protected void Application_Start(object sender, EventArgs e)
        {
            RubyEngine.InitializeIronRubyMvc(new VirtualPathProvider(), "~/routes.rb");
            OnStart();
        }

        /// <summary>
        /// Called when the application is starting and the engine has been initialized.
        /// </summary>
        protected virtual void OnStart()
        {
            // override this to provide start behavior
        }

    }
}