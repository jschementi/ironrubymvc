using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace IronRubyMvcWeb.Models.Northwind
{
    public partial class NorthwindDataContext
    {
        private readonly IList<Category> categories;

        public NorthwindDataContext(IList<Category> categories)
            : base(ConfigurationManager.ConnectionStrings["NORTHWNDConnectionString"].ConnectionString, mappingSource)
        {
            this.categories = categories;
        }

        public NorthwindDataContext(IList<Category> categories, string connectionString)
            : base(connectionString, mappingSource)
        {
            this.categories = categories;
        }

        public virtual IList<Category> GetCategories()
        {
            if (categories == null)
                return Categories.ToList();
            else
                return categories;
        }
    }
}