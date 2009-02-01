#region Usings

using System;
using System.Web;
using IronRubyMvcLibrary;

#endregion

namespace IronRubyMvcWeb
{
    public class GlobalApplication : HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            this.InitializeIronRubyMvc();
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
        }
    }
}