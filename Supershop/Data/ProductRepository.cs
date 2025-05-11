using Supershop.Data.Entities;

namespace Supershop.Data
{
    public class ProductRepository : GenericRepository<Product>, IProductsRepository
    {

        public ProductRepository(DataContext context) : base(context)
        {
          
        }
    }
}
