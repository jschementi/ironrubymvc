#region Usings

using System.IO;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Routing;
using IronRuby;
using IronRuby.Runtime;
using IronRubyMvcLibrary.Controllers;
using Microsoft.Scripting.Hosting;

#endregion

namespace IronRubyMvcLibrary
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

            app.Application.SetScriptRuntime(runtime);

            if (vpp.FileExists(routesPath))
                ProcessRubyRoutes(runtime, vpp, routesPath);

            var factory = new RubyControllerFactory(ControllerBuilder.Current.GetControllerFactory());
            ControllerBuilder.Current.SetControllerFactory(factory);
            ViewEngines.Engines.Add(new RubyViewEngine());
        }

        private static void ProcessRubyRoutes(ScriptRuntime runtime, VirtualPathProvider vpp, string routesPath)
        {
            var routeCollection = new RubyRouteCollection(RouteTable.Routes);

            ScriptEngine rubyEngine = Ruby.GetEngine(runtime);
            RubyContext rubyContext = Ruby.GetExecutionContext(runtime);

            rubyContext.DefineReadOnlyGlobalVariable("routes", routeCollection);

            // REVIEW: Should we pull this information from the loaded versions of these assemblies?
            string header =
                @"
require 'System.Web.Abstractions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35'
require 'System.Web.Routing, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35'
require 'System.Web.Mvc, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
            ";

            rubyEngine.CreateScriptSourceFromString(header).Execute();

            using (Stream stream = vpp.GetFile(routesPath).Open())
            using (var reader = new StreamReader(stream))
            {
                string routesText = reader.ReadToEnd();
                rubyEngine.CreateScriptSourceFromString(routesText).Execute();
            }
        }
    }
}