#region Usings

using System.Web.Mvc.Html;
using System.Web.Mvc.IronRuby.Extensions;
using IronRuby.Builtins;

#endregion

namespace System.Web.Mvc.IronRuby.Helpers
{
    public partial class RubyHtmlHelper
    {
        public string DropDownList(string name, string optionLabel)
        {
            return _helper.DropDownList(name, optionLabel);
        }

        public string DropDownList(string name, RubyArray selectList, string optionLabel)
        {
            return _helper.DropDownList(name, selectList.ToSelectListItemList(), optionLabel);
        }

        public string DropDownList(string name, RubyArray selectList, string optionLabel, Hash htmlAttributes)
        {
            return _helper.DropDownList(name, selectList.ToSelectListItemList(), optionLabel, htmlAttributes.ToDictionary());
        }

        public string DropDownList(string name)
        {
            return _helper.DropDownList(name);
        }

        public string DropDownList(string name, RubyArray selectList)
        {
            return _helper.DropDownList(name, selectList.ToSelectListItemList());
        }

        public string DropDownList(string name, RubyArray selectList, Hash htmlAttributes)
        {
            return _helper.DropDownList(name, selectList.ToSelectListItemList(), htmlAttributes.ToDictionary());
        }

        public string ListBox(string name)
        {
            return _helper.ListBox(name);
        }

        public string ListBox(string name, RubyArray selectList)
        {
            return _helper.ListBox(name, selectList.ToSelectListItemList());
        }

        public string ListBox(string name, RubyArray selectList, Hash htmlAttributes)
        {
            return _helper.ListBox(name, selectList.ToSelectListItemList(), htmlAttributes.ToDictionary());
        }
    }
}