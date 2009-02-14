#region Usings

using System.IO;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Routing;
using IronRuby;
using IronRuby.Runtime;
using IronRubyMvcLibrary.Controllers;
using IronRubyMvcLibrary.Core;
using IronRubyMvcLibrary.ViewEngine;
using Microsoft.Scripting.Hosting;
using VirtualPathProvider=System.Web.Hosting.VirtualPathProvider;

#endregion

namespace IronRubyMvcLibrary.Extensions
{
    public static class InitializeIronRubyMvcExtensions
    {
        public static void InitializeIronRubyMvc(this HttpApplication app)
        {
            InitializeIronRubyMvc(app, HostingEnvironment.VirtualPathProvider);
        }

        public static void InitializeIronRubyMvc(this HttpApplication app, VirtualPathProvider vpp)
        {
            InitializeIronRubyMvc(app, vpp, "~/routes.rb");
        }

        public static void InitializeIronRubyMvc(this HttpApplication app, VirtualPathProvider vpp, string routesPath)
        {
            LanguageSetup rubySetup = Ruby.CreateRubySetup();
            rubySetup.Options["InterpretedMode"] = true;

            var runtimeSetup = new ScriptRuntimeSetup();
            runtimeSetup.LanguageSetups.Add(rubySetup);
            runtimeSetup.DebugMode = true;

            ScriptRuntime runtime = Ruby.CreateRuntime(runtimeSetup);
            RubyEngine engine = RubyEngine.Create(runtime);

            ProcessRubyRoutes(engine, routesPath);

            var factory = new RubyControllerFactory(ControllerBuilder.Current.GetControllerFactory(), engine);
            ControllerBuilder.Current.SetControllerFactory(factory);
            ViewEngines.Engines.Add(new RubyViewEngine());
        }

        private static void ProcessRubyRoutes(RubyEngine engine, string routesPath)
        {
            var routeCollection = new RubyRouteCollection(RouteTable.Routes);

            engine.DefineReadOnlyGlobalVariable("routes", routeCollection);

            engine.LoadAssembly(typeof (HttpResponseBase).Assembly);
            engine.LoadAssembly(typeof (RouteTable).Assembly);
            engine.LoadAssembly(typeof (ActionDescriptor).Assembly);

            engine.RequireRubyFile(routesPath, ReaderType.File);
        }
    }
}