namespace IronRubyMvc {
    using System;
    using System.IO;
    using System.Web.Mvc;
    using System.Web.Routing;

    public class RubyViewEngine : VirtualPathProviderViewEngine {
        public RubyViewEngine() {
            PartialViewLocationFormats = new string[] {
                "~/Views/{1}/_{0}.rhtml", 
                "~/Views/Shared/_{0}.rhtml"
            };

            ViewLocationFormats = new string[] { 
                "~/Views/{1}/{0}.rhtml", 
                "~/Views/Shared/{0}.rhtml", 
            };

            MasterLocationFormats = new string[] { 
                "~/Views/{1}/{0}.rhtml", 
                "~/Views/Shared/{0}.rhtml"
            };
        }

        string GetContents(string path) {
            using (var stream = VirtualPathProvider.GetFile(path).Open())
            using (var reader = new StreamReader(stream))
                return reader.ReadToEnd();
        }

        RubyView GetView(RequestContext requestContext, string virtualPath, RubyView masterView) {
            if (String.IsNullOrEmpty(virtualPath))
                return null;

            return new RubyView(GetContents(virtualPath), masterView);
        }

        protected override IView CreatePartialView(ControllerContext controllerContext, string partialPath) {
            return GetView(controllerContext, partialPath, null);
        }

        protected override IView CreateView(ControllerContext controllerContext, string viewPath, string masterPath) {
            return GetView(controllerContext, viewPath, GetView(controllerContext, masterPath, null));
        }
    }
}
