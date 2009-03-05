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
    public interface IConverter<TITarget>
    {
        TITarget Convert(Hash filterDescription);
        TITarget Convert();
    }

    public class HashToActionFilterConverter : HashConverter<IActionFilter>
    {
        private static readonly IEnumerable<SymbolId> _actionFilterDenominators = new[]
                                                                                      {
                                                                                          SymbolConstants.Before,
                                                                                          SymbolConstants.After,
                                                                                          SymbolConstants.Around, 
                                                                                          SymbolConstants.Class
                                                                                      };

        private static readonly IDictionary<OnExecuting, SymbolId> _actionWhen = new Dictionary<OnExecuting, SymbolId>
                                                                                     {
                                                                                         {OnExecuting.BeforeAction, SymbolConstants.Before},
                                                                                         {OnExecuting.AfterAction, SymbolConstants.After}
                                                                                     };

        public HashToActionFilterConverter()
        {
        }

        public HashToActionFilterConverter(IRubyEngine rubyEngine) : base(rubyEngine)
        {
        }

        public HashToActionFilterConverter(IRubyEngine rubyEngine, Hash filterDescription) : base(rubyEngine, filterDescription)
        {
        }

        protected override IActionFilter Build()
        {
            var beforeAction = FindProc(_actionWhen[OnExecuting.BeforeAction]);
            var afterAction = FindProc(_actionWhen[OnExecuting.AfterAction]);

            if(beforeAction.IsNotNull() || afterAction.IsNotNull())
            {
                return new RailsStyleActionFilter
                           {
                               BeforeAction = beforeAction,
                               AfterAction = afterAction
                           };
            }
            var filterClass = FilterDescription[SymbolConstants.Class] as RubyClass;
            return filterClass.IsNotNull() ? _rubyEngine.CreateInstance<IActionFilter>(filterClass, false) : null;
        }

        protected override bool IsFilter()
        {
            var key = (SymbolId) FilterDescription[SymbolConstants.When];
            return _actionFilterDenominators.Contains(key);
        }
    }
}