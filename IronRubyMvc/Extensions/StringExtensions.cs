#region Usings

using System.Globalization;
using Microsoft.Scripting;

#endregion

namespace System.Web.Mvc.IronRuby.Extensions
{
    /// <summary>
    /// Contains the extension methods for <see cref="string"/>
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Determines whether the value is null or blank.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// 	<c>true</c> if the specified value is null or blank; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNullOrBlank(this string value)
        {
            return string.IsNullOrEmpty(value) || value.Trim().Length == 0;
        }

        /// <summary>
        /// Determines whether the specified value is not null or blank.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// 	<c>true</c> if the specified value is not null or blank; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNotNullOrBlank(this string value)
        {
            return !value.IsNullOrBlank();
        }

        /// <summary>
        /// Formats the specified format string with the provided parameters.
        /// </summary>
        /// <param name="value">The format string.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public static string FormattedWith(this string value, params object[] parameters)
        {
            return string.Format(CultureInfo.CurrentUICulture, value, parameters);
        }

        /// <summary>
        /// Converts the string to a case-sensitive <see cref="SymbolId"/>
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static SymbolId ToSymbolId(this string value)
        {
            return ToSymbolId(value, true);
        }

        /// <summary>
        /// Converts the string to a <see cref="SymbolId"/>
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="caseSensitive">if set to <c>true</c> the <see cref="SymbolId"/> will be case-sensitive.</param>
        /// <returns></returns>
        public static SymbolId ToSymbolId(this string value, bool caseSensitive)
        {
            return caseSensitive ? SymbolTable.StringToId(value) : SymbolTable.StringToCaseInsensitiveId(value);
        }

        /// <summary>
        /// Ensures the argument not empty.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="argumentName">Name of the argument.</param>
        public static void EnsureArgumentNotEmpty(this string value, string argumentName)
        {
            if (value.IsNullOrBlank()) throw new ArgumentNullException(argumentName, "Cannot be null");
        }
    }
}