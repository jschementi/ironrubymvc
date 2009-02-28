using System.Collections.Generic;
using System.IO;
using IronRuby.Builtins;
using Microsoft.Scripting;

namespace IronRubyMvcLibrary.Controllers
{
    public abstract class HashConverter<TToConvert> : IConverter<TToConvert> where TToConvert : class
    {
        protected static readonly SymbolId whenKey = SymbolTable.StringToId("when");

        public Hash FilterDescription { get; set; }

        protected HashConverter() : this(null){}

        protected HashConverter(Hash filterDescription)
        {
            FilterDescription = filterDescription;
        }

        #region Implementation of IConverter<TToConvert>

        public virtual TToConvert Convert(Hash filterDescription)
        {
            FilterDescription = filterDescription;
            return Convert();
        }

        public virtual TToConvert Convert()
        {
            return !IsFilter() ? null : Build();
        }

        #endregion

        protected abstract TToConvert Build();

        protected abstract bool IsFilter();

        protected Proc FindProc(SymbolId key)
        {
            return FilterDescription.ContainsKey(key) ? (Proc)FilterDescription[key] : null;
        }
    }
}