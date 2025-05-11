using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Supershop.Data;

namespace Supershop.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : Controller
    {
        private readonly IProductsRepository _productsRepository;

        public ProductsController(IProductsRepository productsRepository)
        {
            _productsRepository = productsRepository;
        }



        [HttpGet]
        public IActionResult GetProducts()
        {
            return Ok(_productsRepository.GetAll());
        }
    }
}
