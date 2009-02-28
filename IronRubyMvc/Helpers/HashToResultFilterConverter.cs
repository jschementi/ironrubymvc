#region Usings

using System.Collections.Generic;
using IronRuby.Builtins;
using IronRubyMvcLibrary.Controllers;
using IronRubyMvcLibrary.Core;
using IronRubyMvcLibrary.Extensions;
using Microsoft.Scripting;

#endregion

namespace IronRubyMvcLibrary.Helpers
{
    public class HashToResultFilterConverter : HashConverter<RailsStyleResultFilter>
    {
        private static readonly IEnumerable<SymbolId> _actionFilterDenominators = new[]
                                                                                      {
                                                                                          SymbolTable.StringToId(
                                                                                              "before_result"),
                                                                                          SymbolTable.StringToId(
                                                                                              "after_result")
                                                                                      };

        private static readonly IDictionary<OnExecuting, SymbolId> _actionWhen = new Dictionary<OnExecuting, SymbolId>
                                                                                     {
                                                                                         {
                                                                                             OnExecuting.BeforeResult,
                                                                                             SymbolTable.StringToId(
                                                                                             "before_result")
                                                                                             },
                                                                                         {
                                                                                             OnExecuting.AfterResult,
                                                                                             SymbolTable.StringToId(
                                                                                             "after_result")
                                                                                             },
                                                                                     };


        public HashToResultFilterConverter()
        {
        }

        public HashToResultFilterConverter(Hash filterDescription)
            : base(filterDescription)
        {
        }


        protected override RailsStyleResultFilter Build()
        {
            var beforeResult = FindProc(_actionWhen[OnExecuting.BeforeResult]);
            var afterResult = FindProc(_actionWhen[OnExecuting.AfterResult]);
            if (beforeResult.IsNull() && afterResult.IsNull()) return null;

            return new RailsStyleResultFilter
                       {
                           BeforeResult = beforeResult,
                           AfterResult = afterResult
                       };
        }

        protected override bool IsFilter()
        {
            var key = (SymbolId) FilterDescription[whenKey];
            return _actionFilterDenominators.Contains(key);
        }
    }
}