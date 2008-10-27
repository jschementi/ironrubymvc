namespace IronRubyMvc {
    using System;
    using System.IO;
    using System.Text;
    using System.Web.Mvc;
    using Microsoft.Scripting.Hosting;
    using IronRuby;
    using IronRuby.Runtime;

    public class RubyView : IView {
        string _contents;
        RubyView _master;
        RubyTemplate _template;
        string _helpers;

        public RubyView(string viewContents, RubyView master, string helperContents) {
            _master = master;
            _contents = viewContents;
            _helpers = helperContents;
        }

        public RubyTemplate Template {
            get {
                if (_template == null)
                    _template = new RubyTemplate(_contents);
                return _template;
            }
        }

        public void Render(ViewContext context, TextWriter writer) {
            ScriptRuntime runtime = context.HttpContext.Application.GetScriptRuntime();
            ScriptEngine rubyEngine = Ruby.GetEngine(runtime);
            RubyExecutionContext rubyContext = Ruby.GetExecutionContext(runtime);

            ScriptScope scope = runtime.CreateScope();
            scope.SetVariable("view_data", context.ViewData);
            scope.SetVariable("model", context.ViewData.Model);
            scope.SetVariable("context", context);
            scope.SetVariable("response", context.HttpContext.Response);
            scope.SetVariable("url", new RubyUrlHelper(context));
            scope.SetVariable("html", new RubyHtmlHelper(context, new Container(context.ViewData)));
            scope.SetVariable("ajax", new AjaxHelper(context));

            Template.AddRequire("System.Web, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a");
            Template.AddRequire("System.Web.Abstractions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");
            Template.AddRequire("System.Web.Routing, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");
            Template.AddRequire("System.Web.Mvc, Version=1.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");

            // TODO: this should only be done once ... not on each view rendering.
            LoadHelpers(rubyEngine, scope, writer);

            StringBuilder script = new StringBuilder();
            Template.ToScript("render_page", script);

            if (_master != null)
                _master.Template.ToScript("render_layout", script);
            else
                script.AppendLine("def render_layout; yield; end");

            script.AppendLine("def view_data.method_missing(methodname); get_Item(methodname.to_s); end");
            script.AppendLine("render_layout { |content| render_page }");

            try {
                ScriptSource source = rubyEngine.CreateScriptSourceFromString(script.ToString());
                source.Execute(scope);
            }
            catch (Exception e) {
                //writer.Write(script + "<br />");
                writer.Write(e.ToString());
            }
        }

        private void LoadHelpers(ScriptEngine engine, ScriptScope scope, TextWriter writer) {
            try {
                ScriptSource source = engine.CreateScriptSourceFromString(_helpers);
                source.Execute(scope);
            } catch (Exception e) {
                //writer.Write(_helpers + "<br />");
                writer.Write(e.ToString());
            }
        }

        internal class Container : IViewDataContainer {
            internal Container(ViewDataDictionary viewData) {
                ViewData = viewData;
            }

            public ViewDataDictionary ViewData { get; set; }
        }
    }
}