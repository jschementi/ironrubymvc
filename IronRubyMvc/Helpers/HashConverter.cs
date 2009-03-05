#region Usings

using System.Diagnostics.CodeAnalysis;
using IronRuby.Builtins;
using IronRubyMvcLibrary.Core;
using IronRubyMvcLibrary.Extensions;
using Microsoft.Scripting;

#endregion

namespace IronRubyMvcLibrary.Helpers
{
    public abstract class HashConverter<TIToConvert> : IConverter<TIToConvert> 
        where TIToConvert : class
    {
        protected IRubyEngine _rubyEngine;

        protected HashConverter(){}

        protected HashConverter(IRubyEngine rubyEngine) 
        {
            _rubyEngine = rubyEngine;
        }

        protected HashConverter(IRubyEngine rubyEngine, Hash filterDescription) : this(rubyEngine)
        {
            NamedFilterDescription = filterDescription;
        }

        #region Implementation of IConverter<TToConvert>

        public virtual TIToConvert Convert(Hash filterDescription)
        {
            NamedFilterDescription = filterDescription;
            return Convert();
        }

        public virtual TIToConvert Convert(Hash filterDescription, IRubyEngine engine)
        {
            _rubyEngine = engine;
            return Convert(filterDescription);
        }

        public virtual TIToConvert Convert()
        {
            if (NamedFilterDescription.IsNull()) return null;
            var description = NamedFilterDescription[SymbolConstants.Options];

            if(description is Hash)
            {
                FilterDescription = description as Hash;
                
                return IsFilter() ? Build() : null;
            }
            if(description is RubyClass)
            {
                return _rubyEngine.CreateInstance<TIToConvert>(description as RubyClass, false);
            }
            if (FilterDescription is TIToConvert)
            {
                return FilterDescription as TIToConvert;
            }

            return null; // out of luck
        }

        #endregion

        public Hash NamedFilterDescription { get; private set; }
        public Hash FilterDescription { get; private set; }

        protected abstract TIToConvert Build();

        protected abstract bool IsFilter();

        protected Proc FindProc(SymbolId key)
        {
            return FilterDescription.ContainsKey(key) ? (Proc) FilterDescription[key] : null;
        }
    }
}