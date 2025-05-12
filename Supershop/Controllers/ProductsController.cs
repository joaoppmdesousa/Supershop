using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Supershop.Data;
using Supershop.Data.Entities;
using Supershop.Helpers;
using Supershop.Models;


namespace Supershop.Controllers
{
    public class ProductsController : Controller
    {
       
        private readonly IProductsRepository _productsRepository;
        private readonly IUserHelper _userHelper;

        public ProductsController(
            IProductsRepository productsRepository,
            IUserHelper userHelper)
        {
            _productsRepository = productsRepository;
            _userHelper = userHelper;
        }

        // GET: Products
        public IActionResult Index()
        {
            return View(_productsRepository.GetAll().OrderBy(p => p.Name));
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _productsRepository.GetByIdAsync(id.Value);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductViewModel model)
        {
            if (ModelState.IsValid)
            {
                var path = string.Empty;

                if(model.ImageFile != null && model.ImageFile.Length > 0)
                {
                    var guid = Guid.NewGuid().ToString();
                    var file = $"{guid}.jpg";



                    path = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot\\images\\products",
                        file);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await model.ImageFile.CopyToAsync(stream);
                    }

                    path = $"~/images/products/{file}";
                }

                var product = this.ToProduct(model, path);

                //TODO: Modificar para o user que estiver logado
                product.User = await _userHelper.GetUserByEmailAsync("joaopedropsousa@gmail.com");
                await _productsRepository.CreateAsync(product);
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        private Product ToProduct(ProductViewModel model, string path)
        {
            return new Product
            {
                Id = model.Id,
                ImageUrl = path,
                IsAvailable = model.IsAvailable,
                LastPurchase = model.LastPurchase,
                LastSale = model.LastSale,
                Name = model.Name,
                Price = model.Price,                
                Stock = model.Stock,
                User = model.User
            };
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _productsRepository.GetByIdAsync(id.Value);
                if (product == null)
            {
                return NotFound();
            }

                var model = this.ToProductViewModel(product);

            return View(model);
        }

        private ProductViewModel ToProductViewModel(Product product)
        {
            return new ProductViewModel
            {
                Id = product.Id,
                IsAvailable = product.IsAvailable,
                LastPurchase = product.LastPurchase,
                LastSale = product.LastSale,
                ImageUrl = product.ImageUrl,
                Name = product.Name,
                Price = product.Price,
                Stock = product.Stock,
                User = product.User,
            };
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductViewModel model)
        {
          
            if (ModelState.IsValid)
            {
                try
                {

                    var path = model.ImageUrl;

                    if(model.ImageFile != null && model.ImageFile.Length > 0)
                    {
                        var guid = Guid.NewGuid().ToString();
                        var file = $"{guid}.jpg";

                        path = Path.Combine(
                            Directory.GetCurrentDirectory(),
                            "wwwroot\\images\\products",
                            file);



                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            await model.ImageFile.CopyToAsync(stream);
                        }

                        path = $"~/images/products/{file}";
                    }

                    var product = this.ToProduct(model, path);


                    //TODO: Modificar para o user que estiver logado
                    product.User = await _userHelper.GetUserByEmailAsync("joaopedropsousa@gmail.com");
                    await _productsRepository.UpdateAsync(product);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (! await _productsRepository.ExistAsync(model.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await  _productsRepository.GetByIdAsync(id.Value);


            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _productsRepository.GetByIdAsync(id);
            await _productsRepository.DeleteAsync(product);
            return RedirectToAction(nameof(Index));
        }

    }
}
