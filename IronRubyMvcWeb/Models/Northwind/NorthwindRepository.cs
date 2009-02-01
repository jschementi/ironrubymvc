#region Usings

using System.Linq;
using IronRubyMvcWeb.Models.Northwind;

#endregion

namespace IronRubyMvcWeb.Models
{
    public class NorthwindRepository
    {
        private readonly NorthwindDataContext dataContext;

        public NorthwindRepository() : this(new NorthwindDataContext())
        {
        }

        public NorthwindRepository(NorthwindDataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public virtual IQueryable<Category> Categories
        {
            get { return dataContext.Categories; }
        }

        public virtual IQueryable<Product> Products
        {
            get { return dataContext.Products; }
        }

        public virtual IQueryable<Supplier> Suppliers
        {
            get { return dataContext.Suppliers; }
        }

        public virtual void SubmitChanges()
        {
            dataContext.SubmitChanges();
        }

        public virtual void InsertProductOnSubmit(Product p)
        {
            dataContext.Products.InsertOnSubmit(p);
        }
    }
}