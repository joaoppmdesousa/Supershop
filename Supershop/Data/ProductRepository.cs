using Microsoft.EntityFrameworkCore;
using Supershop.Data.Entities;
using System.Linq;

namespace Supershop.Data
{
    public class ProductRepository : GenericRepository<Product>, IProductsRepository
    {
        private readonly DataContext _context;

        public ProductRepository(DataContext context) : base(context)
        {
            _context = context;
        }


        public IQueryable GetAllWithUser()
        {
            return _context.Products
                .Include(p => p.User);
        }
    }
}
