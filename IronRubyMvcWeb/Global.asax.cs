#region Usings

using System;
using System.Web;
using IronRubyMvcLibrary;
using IronRubyMvcLibrary.Core;

#endregion

namespace IronRubyMvcWeb
{
    public class GlobalApplication : RubyMvcApplication
    {

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
        }
    }
}