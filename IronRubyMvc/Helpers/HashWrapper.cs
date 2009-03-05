#region Usings

using System.ComponentModel;
using System.Globalization;
using IronRuby.Builtins;

#endregion

namespace System.Web.Mvc.IronRuby.Helpers
{
    public class HashWrapper : CustomTypeDescriptor
    {
        public HashWrapper(Hash hash)
        {
            Object = hash;
        }

        public Hash Object { get; private set; }

        public override PropertyDescriptorCollection GetProperties()
        {
            var descriptors = new PropertyDescriptor[Object.Count];
            var i = 0;
            foreach (var entry in Object)
            {
                descriptors[i++] =
                    new HashPropertyDescriptor(Convert.ToString(entry.Key, CultureInfo.InvariantCulture), entry.Value);
            }
//            PropertyDescriptor[] descriptors = Object.Select(
//                entry => new HashPropertyDescriptor(Convert.ToString(entry.Key, CultureInfo.InvariantCulture), entry.Value)).ToArray();
            return new PropertyDescriptorCollection(descriptors);
        }

        #region Nested type: HashPropertyDescriptor

        private class HashPropertyDescriptor : PropertyDescriptor
        {
            private readonly object _value;

            public HashPropertyDescriptor(string name, object value)
                : base(name, null /* attributes */)
            {
                _value = value;
            }

            public override Type ComponentType
            {
                get { throw new NotImplementedException(); }
            }

            public override bool IsReadOnly
            {
                get { return true; }
            }

            public override Type PropertyType
            {
                get { return typeof (object); }
            }

            public override bool CanResetValue(object component)
            {
                return false;
            }

            public override object GetValue(object component)
            {
                return _value;
            }

            public override void ResetValue(object component)
            {
                throw new NotImplementedException();
            }

            public override void SetValue(object component, object value)
            {
                throw new NotImplementedException();
            }

            public override bool ShouldSerializeValue(object component)
            {
                throw new NotImplementedException();
            }
        }

        #endregion
    }
}