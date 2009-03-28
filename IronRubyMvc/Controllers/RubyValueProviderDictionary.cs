#region Usings

using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Web.Mvc.IronRuby.Extensions;

#endregion

namespace System.Web.Mvc.IronRuby.Controllers
{
    public class RubyValueProviderDictionary : IDictionary<string, ValueProviderResult>
    {
        private readonly IDictionary<string, ValueProviderResult> _dictionary = new Dictionary<string, ValueProviderResult>();

        public RubyValueProviderDictionary(ControllerContext controllerContext)
        {
            ControllerContext = controllerContext;
            if (controllerContext.IsNotNull()) PopulateDictionary();
        }

        public ControllerContext ControllerContext { get; set; }

        private void PopulateDictionary()
        {
            var request = ControllerContext.HttpContext.Request;
            PopulateParamsWithRequestData(request.Form);
            PopulateParamsWithRequestData(request.QueryString);
            PopulateParamsWithRouteData(ControllerContext.RouteData.Values);
        }

        private void PopulateParamsWithRouteData(IEnumerable<KeyValuePair<string, object>> dictionary)
        {
            if (dictionary.IsNull()) return;
            dictionary.ForEach(pair =>
                                   {
                                       var rawValue = pair.Value;
                                       var attempted = Convert.ToString(pair.Value, CultureInfo.InvariantCulture);
                                       AddToDictionaryIfNotPresent(
                                           pair.Key,
                                           new ValueProviderResult(rawValue, attempted, CultureInfo.InvariantCulture)
                                           );
                                   });
        }

        private void PopulateParamsWithRequestData(NameValueCollection collection)
        {
            if(collection.IsNull()) return;
            collection.AllKeys.ForEach(key =>
                                     {
                                         var rawValue = collection.GetValues(key);
                                         var attempted = collection.Get(key);
                                         AddToDictionaryIfNotPresent(
                                             key, 
                                             new ValueProviderResult(rawValue, attempted, CultureInfo.InvariantCulture)
                                         );
                                     });
        }

        private void AddToDictionaryIfNotPresent(string key, ValueProviderResult result)
        {
            if (key.IsNullOrBlank() || _dictionary.ContainsKey(key)) return;
            _dictionary.Add(key, result);
        }

        #region Implementation of IEnumerable

        public IEnumerator<KeyValuePair<string, ValueProviderResult>> GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Implementation of ICollection<KeyValuePair<string,ValueProviderResult>>

        public void Add(KeyValuePair<string, ValueProviderResult> item)
        {
            item.EnsureArgumentNotNull("item");
            _dictionary.Add(item);
        }

        public void Clear()
        {
            _dictionary.Clear();
        }

        public bool Contains(KeyValuePair<string, ValueProviderResult> item)
        {
            item.EnsureArgumentNotNull("item");

            return _dictionary.Contains(item);
        }

        public void CopyTo(KeyValuePair<string, ValueProviderResult>[] array, int arrayIndex)
        {
            array.EnsureArgumentNotNull("array");

            _dictionary.CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<string, ValueProviderResult> item)
        {
            item.EnsureArgumentNotNull("item");

            return _dictionary.Remove(item);
        }

        public int Count
        {
            get { return _dictionary.Count; }
        }

        public bool IsReadOnly
        {
            get { return _dictionary.IsReadOnly; }
        }

        #endregion

        #region Implementation of IDictionary<string,ValueProviderResult>

        public bool ContainsKey(string key)
        {
            return _dictionary.ContainsKey(key);
        }

        public void Add(string key, ValueProviderResult value)
        {
            _dictionary.Add(key, value);
        }

        public bool Remove(string key)
        {
            return _dictionary.Remove(key);
        }

        public bool TryGetValue(string key, out ValueProviderResult value)
        {
            return _dictionary.TryGetValue(key, out value);
        }

        public ValueProviderResult this[string key]
        {
            get { return _dictionary[key]; }
            set { _dictionary[key] = value; }
        }

        public ICollection<string> Keys
        {
            get { return _dictionary.Keys; }
        }

        public ICollection<ValueProviderResult> Values
        {
            get { return _dictionary.Values; }
        }

        #endregion
    }
}