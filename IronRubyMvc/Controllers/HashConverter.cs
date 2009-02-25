using System.Collections.Generic;
using System.IO;
using IronRuby.Builtins;
using Microsoft.Scripting;

namespace IronRubyMvcLibrary.Controllers
{
    public abstract class HashConverter<TToConvert> : IConverter<TToConvert> where TToConvert : class
    {
        protected readonly Hash _filterDescription;
        protected static readonly SymbolId whenKey = SymbolTable.StringToId("when");

        protected HashConverter(Hash filterDescription)
        {
            _filterDescription = filterDescription;
        }

        #region Implementation of IConverter<RubyActionFilter>

        public TToConvert Convert()
        {
            return !IsFilter() ? null : Build();
        }

        #endregion

        protected abstract TToConvert Build();

        protected abstract bool IsFilter();

        protected Proc FindProc(SymbolId key)
        {
            return _filterDescription.ContainsKey(key) ? (Proc)_filterDescription[key] : null;
        }
    }
}