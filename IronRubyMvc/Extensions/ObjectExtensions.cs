extern alias clr3;
#region Usings

using System.Collections.Generic;
using IronRuby.Builtins;
using Microsoft.Scripting.Runtime;
using clr3::System.Linq;
#endregion

namespace System.Web.Mvc.IronRuby.Extensions
{
    /// <summary>
    /// Contains the extension methods for <see cref="object" />
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// Determines whether [is not null] [the specified value].
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// 	<c>true</c> if [is not null] [the specified value]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNotNull(this object value)
        {
            return value != null;
        }

        /// <summary>
        /// Determines whether the specified value is null.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// 	<c>true</c> if the specified value is null; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNull(this object value)
        {
            return value == null;
        }

        /// <summary>
        /// Ensures the argument is not null.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="argumentName">Name of the argument.</param>
        internal static void EnsureArgumentNotNull(this object value, string argumentName)
        {
            if (value.IsNull()) throw new ArgumentNullException(argumentName, "Cannot be null");
        }

        /// <summary>
        /// Converts the object to a string collection.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        internal static IEnumerable<string> ToStringCollection(this object value)
        {
            var array = (RubyArray) value;
            return array.Cast<string>();
        }
    }
}