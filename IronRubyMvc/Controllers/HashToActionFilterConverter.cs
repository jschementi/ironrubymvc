using System.Collections;
using System.Collections.Generic;
using System.IO;
using IronRuby.Builtins;
using IronRubyMvcLibrary.Core;
using IronRubyMvcLibrary.Extensions;
using Microsoft.Scripting;

namespace IronRubyMvcLibrary.Controllers
{
    public interface IConverter<TTarget>
    {
        TTarget Convert(Hash filterDescriptions);
        TTarget Convert();
    }
    public class HashToActionFilterConverter : HashConverter<RubyRailsStyleActionFilter>
    {
        

        private static readonly IEnumerable<SymbolId> _actionFilterDenominators = new[]
                                                                 {
                                                                     SymbolTable.StringToId("before"),
                                                                     SymbolTable.StringToId("after"),
                                                                     SymbolTable.StringToId("around")
                                                                 };

        private static readonly IDictionary<When, SymbolId> _actionWhen = new Dictionary<When, SymbolId>
                                                                              {
                                                                                  {When.BeforeAction, SymbolTable.StringToId("before")},
                                                                                  {When.AfterAction, SymbolTable.StringToId("after")}
                                                                              };


        public HashToActionFilterConverter()
        {
        }

        public HashToActionFilterConverter(Hash filterDescription) : base(filterDescription)
        {
            
        }

        
        protected override RubyRailsStyleActionFilter Build()
        {
            var beforeAction = FindProc(_actionWhen[When.BeforeAction]);
            var afterAction = FindProc(_actionWhen[When.AfterAction]);
            if(beforeAction.IsNull() && afterAction.IsNull()) return null;

            return new RubyRailsStyleActionFilter
                       {
                           BeforeAction = beforeAction,
                           AfterAction = afterAction
                       };
        }

        protected override bool IsFilter()
        {
            var key = (SymbolId)FilterDescription[whenKey];
            return _actionFilterDenominators.Contains(key);
        }

        
    }
}