using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Supershop.Data.Entities;


namespace Supershop.Data
{
    public interface IProductsRepository : IGenericRepository<Product>
    {
        public IQueryable GetAllWithUser();


        IEnumerable<SelectListItem> GetComboProducts();

    }
}
