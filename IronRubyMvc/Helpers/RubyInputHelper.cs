#region Usings

using System.Web.Mvc.Html;
using System.Web.Mvc.IronRuby.Extensions;
using IronRuby.Builtins;

#endregion

namespace System.Web.Mvc.IronRuby.Helpers
{
    public partial class RubyHtmlHelper
    {
        public string CheckBox(string name)
        {
            return _helper.CheckBox(name, (object) null /* htmlAttributes */);
        }

        public string CheckBox(string name, bool isChecked)
        {
            return _helper.CheckBox(name, isChecked, (object) null /* htmlAttributes */);
        }

        public string CheckBox(string name, bool isChecked, Hash htmlAttributes)
        {
            return _helper.CheckBox(name, isChecked, htmlAttributes.ToDictionary());
        }

        public string CheckBox(string name, Hash htmlAttributes)
        {
            return _helper.CheckBox(name, htmlAttributes.ToDictionary());
        }

        public string Hidden(string name)
        {
            return _helper.Hidden(name, null /* value */);
        }

        public string Hidden(string name, object value)
        {
            return _helper.Hidden(name, value, (object) null /* hmtlAttributes */);
        }

        public string Hidden(string name, object value, Hash htmlAttributes)
        {
            return _helper.Hidden(name, value, htmlAttributes.ToDictionary());
        }

        public string Password(string name)
        {
            return _helper.Password(name, null /* value */);
        }

        public string Password(string name, object value)
        {
            return _helper.Password(name, value, (object) null /* htmlAttributes */);
        }

        public string Password(string name, object value, Hash htmlAttributes)
        {
            return _helper.Password(name, value, htmlAttributes.ToDictionary());
        }

        public string RadioButton(string name, object value)
        {
            return _helper.RadioButton(name, value, (object) null /* htmlAttributes */);
        }

        public string RadioButton(string name, object value, Hash htmlAttributes)
        {
            return _helper.RadioButton(name, value, htmlAttributes.ToDictionary());
        }

        public string RadioButton(string name, object value, bool isChecked)
        {
            return _helper.RadioButton(name, value, isChecked, (object) null /* htmlAttributes */);
        }

        public string RadioButton(string name, object value, bool isChecked, Hash htmlAttributes)
        {
            return _helper.RadioButton(name, value, isChecked, htmlAttributes.ToDictionary());
        }


        public string TextBox(string name, object value, Hash htmlAttributes)
        {
            return _helper.TextBox(name, value, htmlAttributes.ToDictionary());
        }

        public string TextBox(string name)
        {
            //Yeah, I know this is sooo wrong, but still.
            name = name.Replace("_", "");
            return _helper.TextBox(name);
        }

        public string TextBox(string name, object value)
        {
            //Yeah, I know this is sooo wrong, but still.
            name = name.Replace("_", "");
            return _helper.TextBox(name, value.ToString());
        }
    }
}