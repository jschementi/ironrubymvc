namespace System.Web.Mvc.IronRuby.Extensions
{
    public static class FilterInfoExtensions
    {
        public static FilterInfo MergedWith(this FilterInfo target, FilterInfo source)
        {
            source.ActionFilters.ForEach(filter => target.ActionFilters.Add(filter));
            source.AuthorizationFilters.ForEach(filter => target.AuthorizationFilters.Add(filter));
            source.ResultFilters.ForEach(filter => target.ResultFilters.Add(filter));
            source.ExceptionFilters.ForEach(filter => target.ExceptionFilters.Add(filter));

            return target;
        }
    }
}