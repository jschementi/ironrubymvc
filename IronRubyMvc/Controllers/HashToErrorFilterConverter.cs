using System.Web.Mvc;
using IronRuby.Builtins;
using Microsoft.Scripting;

namespace IronRubyMvcLibrary.Controllers
{
    /// <summary>
    /// Converts a ruby hash to a <see cref="IExceptionFilter"/>
    /// </summary>
    public class HashToErrorFilterConverter : HashConverter<RubyErrorFilter>
    {

        private static readonly SymbolId errorKey = SymbolTable.StringToId("error");

        /// <summary>
        /// Initializes a new instance of the <see cref="HashToErrorFilterConverter"/> class.
        /// </summary>
        /// <param name="filterDescription">The filter description.</param>
        public HashToErrorFilterConverter(Hash filterDescription) : base(filterDescription)
        {
        }

        #region Overrides of HashConverter<RubyErrorFilter>

        protected override RubyErrorFilter Build()
        {
            return new RubyErrorFilter {Error = FindProc(errorKey)};
        }

        protected override bool IsFilter()
        {
            return errorKey == (SymbolId)_filterDescription[whenKey];
        }

        #endregion
    }
}