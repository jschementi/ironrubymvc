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
    public class HashToActionFilterConverter : IConverter<RubyActionFilter>
    {
        private readonly Hash _filterDescription;

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

        private static readonly SymbolId whenKey = SymbolTable.StringToId("when");
        private static readonly SymbolId authorizeKey = SymbolTable.StringToId("authorize");
        private static readonly SymbolId errorKey = SymbolTable.StringToId("error");

        public HashToActionFilterConverter(Hash filterDescription)
        {
            _filterDescription = filterDescription;
        }

        #region Implementation of IConverter<RubyActionFilter>

        public RubyActionFilter Convert()
        {
            if (!IsActionFilter(_filterDescription))
                throw new InvalidDataException("The filter description is invalid.");

            return new RubyActionFilter
            {
                BeforeAction = FindProc(_filterDescription, _actionWhen[When.BeforeAction]),
                AfterAction = FindProc(_filterDescription, _actionWhen[When.AfterAction]),
                BeforeResult = FindProc(_filterDescription, _actionWhen[When.BeforeResult]),
                AfterResult = FindProc(_filterDescription, _actionWhen[When.AfterResult])
            };
        }

        #endregion

        private static bool IsActionFilter(IDictionary<object, object> hash)
        {
            var key = (SymbolId)hash[whenKey];
            return _actionFilterDenominators.Contains(key);
        }

        private static bool IsAuthorizeFilter(IDictionary<object, object> hash)
        {
            return authorizeKey == (SymbolId) hash[whenKey];
        }

        private static bool IsErrorFilter(IDictionary<object, object> hash)
        {
            return errorKey == (SymbolId) hash[whenKey];
        }

        private static Proc FindProc(IDictionary<object, object> hash, SymbolId key)
        {
            return hash.ContainsKey(key) ? (Proc)hash[key] : null;
        }
    }
}