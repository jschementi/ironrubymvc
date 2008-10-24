namespace IronRubyMvcWeb {
    using System;
    using System.Web;
    using System.Web.Mvc;
    using IronRubyMvc;
    using System.Web.Routing;

    public class GlobalApplication : HttpApplication {
        protected void Application_Start(object sender, EventArgs e) {
            this.InitializeIronRubyMvc();
        }
    }
}