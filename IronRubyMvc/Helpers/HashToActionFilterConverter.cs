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
    public interface IConverter<TTarget>
    {
        TTarget Convert(Hash filterDescription);
        TTarget Convert();
    }

    public class HashToActionFilterConverter : HashConverter<RailsStyleActionFilter>
    {
        private static readonly IEnumerable<SymbolId> _actionFilterDenominators = new[]
                                                                                      {
                                                                                          SymbolTable.StringToId(
                                                                                              "before"),
                                                                                          SymbolTable.StringToId("after")
                                                                                          ,
                                                                                          SymbolTable.StringToId(
                                                                                              "around")
                                                                                      };

        private static readonly IDictionary<OnExecuting, SymbolId> _actionWhen = new Dictionary<OnExecuting, SymbolId>
                                                                                     {
                                                                                         {
                                                                                             OnExecuting.BeforeAction,
                                                                                             SymbolTable.StringToId(
                                                                                             "before")
                                                                                             },
                                                                                         {
                                                                                             OnExecuting.AfterAction,
                                                                                             SymbolTable.StringToId(
                                                                                             "after")
                                                                                             }
                                                                                     };


        public HashToActionFilterConverter()
        {
        }

        public HashToActionFilterConverter(Hash filterDescription) : base(filterDescription)
        {
        }


        protected override RailsStyleActionFilter Build()
        {
            var beforeAction = FindProc(_actionWhen[OnExecuting.BeforeAction]);
            var afterAction = FindProc(_actionWhen[OnExecuting.AfterAction]);
            if (beforeAction.IsNull() && afterAction.IsNull()) return null;

            return new RailsStyleActionFilter
                       {
                           BeforeAction = beforeAction,
                           AfterAction = afterAction
                       };
        }

        protected override bool IsFilter()
        {
            var key = (SymbolId) FilterDescription[whenKey];
            return _actionFilterDenominators.Contains(key);
        }
    }
}