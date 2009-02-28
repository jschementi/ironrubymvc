#region Usings

using System.Web.Mvc;
using IronRuby.Builtins;
using IronRubyMvcLibrary.Controllers;
using IronRubyMvcLibrary.Extensions;
using Microsoft.Scripting;

#endregion

namespace IronRubyMvcLibrary.Helpers
{
    /// <summary>
    /// Converts a ruby hash to a <see cref="IExceptionFilter"/>
    /// </summary>
    public class HashToExceptionFilterConverter : HashConverter<RailsStyleExceptionFilter>
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

        #region Overrides of HashConverter<RailsStyleExceptionFilter>

        protected override RailsStyleExceptionFilter Build()
        {
            var error = FindProc(errorKey);
            return error.IsNull() ? null : new RailsStyleExceptionFilter {Error = error};
        }

        protected override bool IsFilter()
        {
            return errorKey == (SymbolId) FilterDescription[whenKey];
        }

        #endregion
    }
}