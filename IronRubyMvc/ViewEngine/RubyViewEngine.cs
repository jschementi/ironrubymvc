#region Usings

using System.IO;

#endregion

namespace System.Web.Mvc.IronRuby.ViewEngine
{
    public class RubyViewEngine : VirtualPathProviderViewEngine
    {
        public RubyViewEngine()
        {
            PartialViewLocationFormats = new[]
                                             {
                                                 "~/Views/{1}/_{0}.html.erb",
                                                 "~/Views/Shared/_{0}.html.erb"
                                             };

            ViewLocationFormats = new[]
                                      {
                                          "~/Views/{1}/{0}.html.erb",
                                          "~/Views/Shared/{0}.html.erb",
                                      };

            MasterLocationFormats = new[]
                                        {
                                            "~/Views/{1}/{0}.html.erb",
                                            "~/Views/Shared/{0}.html.erb"
                                        };
        }

        private string GetContents(string path)
        {
            using (var stream = VirtualPathProvider.GetFile(path).Open())
            using (var reader = new StreamReader(stream))
                return reader.ReadToEnd();
        }

        private RubyView GetView(ControllerContext requestContext, string virtualPath, RubyView masterView)
        {
            if (String.IsNullOrEmpty(virtualPath))
                return null;

            return new RubyView(GetContents(virtualPath), masterView, GetContents("~/Views/Helpers.rb"));
        }

        protected override IView CreatePartialView(ControllerContext controllerContext, string partialPath)
        {
            return GetView(controllerContext, partialPath, null);
        }

        protected override IView CreateView(ControllerContext controllerContext, string viewPath, string masterPath)
        {
            return GetView(controllerContext, viewPath, GetView(controllerContext, masterPath, null));
        }
    }
}