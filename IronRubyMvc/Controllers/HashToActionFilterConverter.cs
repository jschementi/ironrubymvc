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
        TTarget Convert();
    }
    public class HashToActionFilterConverter : HashConverter<RubyActionFilter>
    {
        

        private static readonly IEnumerable<SymbolId> _actionFilterDenominators = new[]
                                                                 {
                                                                     SymbolTable.StringToId("before"),
                                                                     SymbolTable.StringToId("after"),
                                                                     SymbolTable.StringToId("around"),
                                                                     SymbolTable.StringToId("before_result"),
                                                                     SymbolTable.StringToId("after_result")
                                                                 };

        private static readonly IDictionary<When, SymbolId> _actionWhen = new Dictionary<When, SymbolId>
                                                                              {
                                                                                  {When.BeforeAction, SymbolTable.StringToId("before")},
                                                                                  {When.AfterAction, SymbolTable.StringToId("after")},
                                                                                  {When.BeforeResult, SymbolTable.StringToId("before_result")},
                                                                                  {When.AfterResult, SymbolTable.StringToId("after_result")},

                                                                              };

        
        
        

        public HashToActionFilterConverter(Hash filterDescription) : base(filterDescription)
        {
            
        }

        
        protected override RubyActionFilter Build()
        {
            return new RubyActionFilter
            {
                BeforeAction = FindProc(_actionWhen[When.BeforeAction]),
                AfterAction = FindProc(_actionWhen[When.AfterAction]),
                BeforeResult = FindProc(_actionWhen[When.BeforeResult]),
                AfterResult = FindProc(_actionWhen[When.AfterResult])
            };
        }

        protected override bool IsFilter()
        {
            var key = (SymbolId)_filterDescription[whenKey];
            return _actionFilterDenominators.Contains(key);
        }

        
    }
}