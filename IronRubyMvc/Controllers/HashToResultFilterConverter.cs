using System.Collections.Generic;
using IronRuby.Builtins;
using IronRubyMvcLibrary.Core;
using IronRubyMvcLibrary.Extensions;
using Microsoft.Scripting;

namespace IronRubyMvcLibrary.Controllers
{
    public class HashToResultFilterConverter : HashConverter<RubyResultFilter>
    {
        

        private static readonly IEnumerable<SymbolId> _actionFilterDenominators = new[]
                                                                 {
                                                                     SymbolTable.StringToId("before_result"),
                                                                     SymbolTable.StringToId("after_result")
                                                                 };

        private static readonly IDictionary<When, SymbolId> _actionWhen = new Dictionary<When, SymbolId>
                                                                              {
                                                                                  {When.BeforeResult, SymbolTable.StringToId("before_result")},
                                                                                  {When.AfterResult, SymbolTable.StringToId("after_result")},

                                                                              };


        public HashToResultFilterConverter()
        {
        }

        public HashToResultFilterConverter(Hash filterDescription)
            : base(filterDescription)
        {
            
        }

        
        protected override RubyResultFilter Build()
        {
            var beforeResult = FindProc(_actionWhen[When.BeforeResult]);
            var afterResult = FindProc(_actionWhen[When.AfterResult]);
            if (beforeResult.IsNull() && afterResult.IsNull()) return null;

            return new RubyResultFilter
                       {
                           BeforeResult = beforeResult,
                           AfterResult = afterResult
                       };
        }

        protected override bool IsFilter()
        {
            var key = (SymbolId)FilterDescription[whenKey];
            return _actionFilterDenominators.Contains(key);
        }

        
    }
}