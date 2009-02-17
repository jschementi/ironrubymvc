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
            var filters = descriptions.ToActionFilters();

            AddActionFilters(filters);
            AddResultFilters(filters);
        }

        private void AddResultFilters(IEnumerable<RubyActionFilter> filters)
        {
            filters
                .Select(filter => filter.BeforeResult.IsNotNull() || filter.AfterResult.IsNotNull())
                .ForEach(filter => ActionFilters.Add(filter));
        }

        

        private void AddActionFilters(IEnumerable<RubyActionFilter> filters)
        {
            filters
                .Select(filter => filter.BeforeAction.IsNotNull() || filter.AfterAction.IsNotNull())
                .ForEach(filter => ActionFilters.Add(filter));
        }
    }
}