using System.Linq;
using Supershop.Data.Entities;


namespace Supershop.Data
{
    public interface IProductsRepository : IGenericRepository<Product>
    {
        public IQueryable GetAllWithUser();
    }
}
