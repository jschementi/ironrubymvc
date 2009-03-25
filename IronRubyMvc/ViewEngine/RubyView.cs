#region Usings

using System.IO;
using System.Text;
using System.Web.Mvc.IronRuby.Core;
using System.Web.Mvc.IronRuby.Helpers;
using IronRuby;
using Microsoft.Scripting.Hosting;

#endregion

namespace System.Web.Mvc.IronRuby.ViewEngine
{
    public class RubyView : IView
    {
        private readonly string _contents;
        private readonly IRubyEngine _rubyEngine;
        private readonly RubyView _master;
        private RubyTemplate _template;


        public RubyView(IRubyEngine rubyEngine, string viewContents, RubyView master)
        {
            _rubyEngine = rubyEngine;
            _master = master;
            _contents = viewContents;
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
            _rubyEngine.ExecuteInScope(scope => RenderView(scope, context, writer));

        }

        private void RenderView(ScriptScope scope, ViewContext context, TextWriter writer)
        {
            scope.SetVariable("view_data", context.ViewData);
            scope.SetVariable("model", context.ViewData.Model);
            scope.SetVariable("context", context);
            scope.SetVariable("response", context.HttpContext.Response);
            scope.SetVariable("url", new RubyUrlHelper(context.RequestContext));
            scope.SetVariable("html", new RubyHtmlHelper(context, new Container(context.ViewData)));

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
                _rubyEngine.ExecuteScript(script.ToString(), scope);
            }
            catch (Exception e)
            {
                writer.Write(e.ToString());
            }
        }

        #endregion
        
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