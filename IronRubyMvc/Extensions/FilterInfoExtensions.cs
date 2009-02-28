#region Usings

using System.Collections;
using System.Collections.Generic;
using System.Web.Mvc;
using IronRubyMvcLibrary.Core;
using IronRubyMvcLibrary.Extensions;

#endregion

namespace IronRubyMvcLibrary.Extensions
{
    internal static class FilterInfoExtensions 
    {

        public static FilterInfo AddControllerFilters(this FilterInfo filterInfo, IDictionary filterDescriptions)
        {
            filterDescriptions.ToActionFilters().ForEach(filter => filterInfo.ActionFilters.Add(filter));
            filterDescriptions.ToAuthorizationFilters().ForEach(filter => filterInfo.AuthorizationFilters.Add(filter));
            filterDescriptions.ToExceptionFilters().ForEach(filter => filterInfo.ExceptionFilters.Add(filter));
            filterDescriptions.ToResultFilters().ForEach(filter => filterInfo.ResultFilters.Add(filter));
            return filterInfo;
        }
        
        
    }
}