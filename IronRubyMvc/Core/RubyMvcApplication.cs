#region Usings

using System;
using System.Web;
using IronRubyMvcLibrary.Extensions;

#endregion

namespace IronRubyMvcLibrary.Core
{
    public class RubyMvcApplication : HttpApplication
    {
        protected virtual void Application_Start(object sender, EventArgs e)
        {
            this.InitializeIronRubyMvc();
        }
    }
}