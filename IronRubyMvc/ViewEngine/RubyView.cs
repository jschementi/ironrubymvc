#region Usings

using System;
using System.IO;
using System.Text;
using System.Web.Mvc;
using IronRuby;
using IronRuby.Runtime;
using IronRubyMvcLibrary.Helpers;
using Microsoft.Scripting.Hosting;

#endregion

namespace IronRubyMvcLibrary.ViewEngine
{
    public class RubyView : IView
    {
        private readonly string _contents;
        private readonly string _helpers;
        private readonly RubyView _master;
        private RubyTemplate _template;


        public RubyView(string viewContents, RubyView master, string helperContents)
        {
            _master = master;
            _contents = viewContents;
            _helpers = helperContents;
        }

        public RubyTemplate Template
        {
            get
            {
                if (_template == null)
                    _template = new RubyTemplate(_contents);
                return _template;
            }
        }

        #region IView Members

        public void Render(ViewContext context, TextWriter writer)
        {
            ScriptRuntime runtime = context.ViewData["__scriptRuntime"] as ScriptRuntime;
            ScriptEngine rubyEngine = Ruby.GetEngine(runtime);
            RubyContext rubyContext = Ruby.GetExecutionContext(runtime);

            ScriptScope scope = runtime.CreateScope();
            scope.SetVariable("view_data", context.ViewData);
            scope.SetVariable("model", context.ViewData.Model);
            scope.SetVariable("context", context);
            scope.SetVariable("response", context.HttpContext.Response);
            scope.SetVariable("url", new RubyUrlHelper(context.RequestContext));
            scope.SetVariable("html", new RubyHtmlHelper(context, new Container(context.ViewData)));
//            scope.SetVariable("ajax", new AjaxHelper(context, context.View));

            Template.AddRequire("System.Web, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a");
            Template.AddRequire(
                "System.Web.Abstractions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");
            Template.AddRequire("System.Web.Routing, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");
            Template.AddRequire("System.Web.Mvc, Version=1.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");

            // TODO: this should only be done once ... not on each view rendering.
            LoadHelpers(rubyEngine, scope, writer);

            var script = new StringBuilder();
            Template.ToScript("render_page", script);

            if (_master != null)
                _master.Template.ToScript("render_layout", script);
            else
                script.AppendLine("def render_layout; yield; end");

            script.AppendLine("def view_data.method_missing(methodname); get_Item(methodname.to_s); end");
            script.AppendLine("render_layout { |content| render_page }");

            try
            {
                ScriptSource source = rubyEngine.CreateScriptSourceFromString(script.ToString());
                source.Execute(scope);
            }
            catch (Exception e)
            {
                //writer.Write(script + "<br />");
                writer.Write(e.ToString());
            }
        }

        #endregion

        private void LoadHelpers(ScriptEngine engine, ScriptScope scope, TextWriter writer)
        {
            try
            {
                ScriptSource source = engine.CreateScriptSourceFromString(_helpers);
                source.Execute(scope);
            }
            catch (Exception e)
            {
                //writer.Write(_helpers + "<br />");
                writer.Write(e.ToString());
            }
        }

        #region Nested type: Container

        internal class Container : IViewDataContainer
        {
            internal Container(ViewDataDictionary viewData)
            {
                ViewData = viewData;
            }

            #region IViewDataContainer Members

            public ViewDataDictionary ViewData { get; set; }

            #endregion
        }

        #endregion
    }
}