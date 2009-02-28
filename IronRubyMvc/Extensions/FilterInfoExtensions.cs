#region Usings

using System.Collections;
using System.Web.Mvc;
using IronRubyMvcLibrary.Core;

#endregion

namespace IronRubyMvcLibrary.Extensions
{
    internal static class FilterInfoExtensions
    {
        public static FilterInfo AddControllerFilters(this FilterInfo filterInfo, IDictionary filterDescriptions,
                                                      IRubyEngine engine)
        {
            filterDescriptions.ToActionFilters(engine).ForEach(filter => filterInfo.ActionFilters.Add(filter));
            filterDescriptions.ToAuthorizationFilters(engine).ForEach(
                filter => filterInfo.AuthorizationFilters.Add(filter));
            filterDescriptions.ToExceptionFilters(engine).ForEach(filter => filterInfo.ExceptionFilters.Add(filter));
            filterDescriptions.ToResultFilters(engine).ForEach(filter => filterInfo.ResultFilters.Add(filter));
            return filterInfo;
        }
    }
}