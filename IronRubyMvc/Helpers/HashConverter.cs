#region Usings

using System.Diagnostics.CodeAnalysis;
using IronRuby.Builtins;
using Microsoft.Scripting;

#endregion

namespace IronRubyMvcLibrary.Helpers
{
    public abstract class HashConverter<TToConvert> : IConverter<TToConvert> where TToConvert : class
    {
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "when")] protected static readonly SymbolId whenKey = SymbolTable.StringToId("when");

        protected HashConverter() : this(null)
        {
        }

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

        public Hash FilterDescription { get; private set; }

        protected abstract TToConvert Build();

        protected abstract bool IsFilter();

        protected Proc FindProc(SymbolId key)
        {
            return FilterDescription.ContainsKey(key) ? (Proc) FilterDescription[key] : null;
        }
    }
}