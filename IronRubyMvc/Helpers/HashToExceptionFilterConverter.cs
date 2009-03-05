#region Usings

using System.Collections.Generic;
using System.Web.Mvc;
using IronRuby.Builtins;
using IronRubyMvcLibrary.Controllers;
using IronRubyMvcLibrary.Core;
using IronRubyMvcLibrary.Extensions;
using Microsoft.Scripting;

#endregion

namespace IronRubyMvcLibrary.Helpers
{
    /// <summary>
    /// Converts a ruby hash to a <see cref="IExceptionFilter"/>
    /// </summary>
    public class HashToExceptionFilterConverter : HashConverter<IExceptionFilter>
    {
        private static readonly IEnumerable<SymbolId> _exceptionFilterDenominators = new[]
                                                                                      {
                                                                                          SymbolConstants.Error,
                                                                                          SymbolConstants.Class
                                                                                      };

        public HashToExceptionFilterConverter()
        {
        }

        public HashToExceptionFilterConverter(IRubyEngine rubyEngine) : base(rubyEngine)
        {
        }

        public HashToExceptionFilterConverter(IRubyEngine rubyEngine, Hash filterDescription) : base(rubyEngine, filterDescription)
        {
        }

        #region Overrides of HashConverter<RailsStyleExceptionFilter>

        protected override IExceptionFilter Build()
        {
            var error = FindProc(SymbolConstants.Error);
            if(error.IsNotNull()) return new RailsStyleExceptionFilter {Error = error};

            var filterClass = FilterDescription[SymbolConstants.Class] as RubyClass;
            return filterClass.IsNotNull() ? _rubyEngine.CreateInstance<IExceptionFilter>(filterClass, false) : null;
        }

        protected override bool IsFilter()
        {
            var key = (SymbolId)FilterDescription[SymbolConstants.When];
            return _exceptionFilterDenominators.Contains(key);
        }

        #endregion
    }
}