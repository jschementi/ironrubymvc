using System.Web.Mvc;
using IronRuby.Builtins;
using IronRubyMvcLibrary.Extensions;
using Microsoft.Scripting;

namespace IronRubyMvcLibrary.Controllers
{
    /// <summary>
    /// Converts a ruby hash to a <see cref="IExceptionFilter"/>
    /// </summary>
    public class HashToExceptionFilterConverter : HashConverter<RubyExceptionFilter>
    {

        private static readonly SymbolId errorKey = SymbolTable.StringToId("error");

        /// <summary>
        /// Initializes a new instance of the <see cref="HashToExceptionFilterConverter"/> class.
        /// </summary>
        /// <param name="filterDescription">The filter description.</param>
        public HashToExceptionFilterConverter(Hash filterDescription) : base(filterDescription)
        {
        }

        public HashToExceptionFilterConverter()
        {
        }

        #region Overrides of HashConverter<RubyExceptionFilter>

        protected override RubyExceptionFilter Build()
        {
            var error = FindProc(errorKey);
            return error.IsNull() ? null : new RubyExceptionFilter {Error = error};
        }

        protected override bool IsFilter()
        {
            return errorKey == (SymbolId)FilterDescription[whenKey];
        }

        #endregion
    }
}