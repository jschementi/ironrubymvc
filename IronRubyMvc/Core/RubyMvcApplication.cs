#region Usings

#endregion

namespace System.Web.Mvc.IronRuby.Core
{
    public class RubyMvcApplication : HttpApplication
    {
        public IRubyEngine RubyEngine
        {
            get
            {
                return Application["___RubyEngine"] as IRubyEngine;
            }
            set
            {
                Application["___RubyEngine"] = value;
            }
        }
        protected void Application_Start(object sender, EventArgs e)
        {
            var pathProvider = new VirtualPathProvider();
            RubyEngine = Core.RubyEngine.InitializeIronRubyMvc(pathProvider, "~/routes.rb");
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