using System.Web.Mvc.IronRuby.Extensions;

namespace System.Web.Mvc.IronRuby.Core
{
    public class RubyMvcModule : IHttpModule
    {
        #region Implementation of IHttpModule

        /// <summary>
        ///   nitializes a module and prepares it to handle requests.
        /// </summary>
        /// <param name="context">
        ///  An <see cref="T:System.Web.HttpApplication" /> that provides access to the methods, properties, and events common to all application objects within an ASP.NET application 
        /// </param>
        public void Init(HttpApplication context)
        {
            context.Error += context_Error;
        }

        void context_Error(object sender, EventArgs e)
        {
            var application = sender as RubyMvcApplication;
            if(sender.IsNotNull())
            {
                application.Context.Error 
            }
        }

        /// <summary>
        ///                     Disposes of the resources (other than memory) used by the module that implements <see cref="T:System.Web.IHttpModule" />.
        /// </summary>
        public void Dispose()
        {
            throw new System.NotImplementedException();
        }

        #endregion
    }
}