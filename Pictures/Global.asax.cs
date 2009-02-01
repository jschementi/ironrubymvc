#region Usings

using System;
using System.Web;
using IronRubyMvcLibrary;

#endregion

namespace Pictures
{
    public class GlobalApplication : HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            this.InitializeIronRubyMvc();
        }
    }
}