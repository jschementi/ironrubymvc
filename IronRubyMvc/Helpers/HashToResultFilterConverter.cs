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
    public class HashToResultFilterConverter : HashConverter<IResultFilter>
    {
        private static readonly IEnumerable<SymbolId> _resultFilterDenominators = new[]
                                                                                      {
                                                                                          SymbolConstants.BeforeResult,
                                                                                          SymbolConstants.AfterResult,
                                                                                          SymbolConstants.Class
                                                                                      };

        private static readonly IDictionary<OnExecuting, SymbolId> _resultWhen = new Dictionary<OnExecuting, SymbolId>
                                                                                     {
                                                                                         { OnExecuting.BeforeResult, SymbolConstants.BeforeResult },
                                                                                         { OnExecuting.AfterResult, SymbolConstants.AfterResult },
                                                                                     };


        public HashToResultFilterConverter()
        {
        }


        public HashToResultFilterConverter(IRubyEngine rubyEngine) : base(rubyEngine)
        {
        }

        public HashToResultFilterConverter(IRubyEngine rubyEngine, Hash filterDescription) : base(rubyEngine, filterDescription)
        {
        }

        protected override IResultFilter Build()
        {
            var beforeResult = FindProc(_resultWhen[OnExecuting.BeforeResult]);
            var afterResult = FindProc(_resultWhen[OnExecuting.AfterResult]);
            if (beforeResult.IsNotNull() && afterResult.IsNotNull())
            {
                return new RailsStyleResultFilter
                           {
                               BeforeResult = beforeResult,
                               AfterResult = afterResult
                           };
            }

            var filterClass = FilterDescription[SymbolConstants.Class] as RubyClass;
            return filterClass.IsNotNull() ? _rubyEngine.CreateInstance<IResultFilter>(filterClass, false) : null;
        }

        protected override bool IsFilter()
        {
            var key = (SymbolId) FilterDescription[SymbolConstants.When];
            return _resultFilterDenominators.Contains(key);
        }
    }
}