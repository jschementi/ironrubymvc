#region Usings

using System.Collections;
using System.Collections.Generic;
using System.Web.Mvc;
using IronRubyMvcLibrary.Core;
using IronRubyMvcLibrary.Extensions;

#endregion

namespace IronRubyMvcLibrary.Controllers
{
    public class RubyFilterInfo : FilterInfo
    {
        public RubyFilterInfo(IDictionary filterDescriptions)
        {
            ParseFilters(filterDescriptions);
        }

        private void ParseFilters(IDictionary descriptions)
        {
            var actionFilters = descriptions.ToActionFilters();
            var authFilters = descriptions.ToAuthorizationFilters();
            var errorFilters = descriptions.ToErrorFilters();

            AddActionFilters(actionFilters);
            AddResultFilters(actionFilters);
            AddAuthorizationFilters(authFilters);
            AddErrorFilters(errorFilters);
        }

        private void AddResultFilters(IEnumerable<RubyActionFilter> filters)
        {
            filters
                .Select(filter => filter.BeforeResult.IsNotNull() || filter.AfterResult.IsNotNull())
                .ForEach(filter => ResultFilters.Add(filter));
        }

        private void AddAuthorizationFilters(IEnumerable<RubyAuthorizationFilter> filters)
        {
            filters
                .Select(filter => filter.Authorize.IsNotNull())
                .ForEach(filter => AuthorizationFilters.Add(filter));
        }

        private void AddErrorFilters(IEnumerable<RubyErrorFilter> filters)
        {
            filters
                .Select(filter => filter.Error.IsNotNull())
                .ForEach(filter => ExceptionFilters.Add(filter));
        }

        private void AddActionFilters(IEnumerable<RubyActionFilter> filters)
        {
            filters
                .Select(filter => filter.BeforeAction.IsNotNull() || filter.AfterAction.IsNotNull())
                .ForEach(filter => ActionFilters.Add(filter));
        }
    }
}