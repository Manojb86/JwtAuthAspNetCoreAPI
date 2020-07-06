using JwtAuthAspNetCoreApi.Data.DatabaseContext;
using JwtAuthAspNetCoreApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JwtAuthAspNetCoreApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly InventoryDbContext inventoryDbContext;
        public ProductController(InventoryDbContext inventoryDbContext)
        {
            this.inventoryDbContext = inventoryDbContext;
        }

        [HttpGet]
        [Route("all")]
        public async Task<IEnumerable<Product>> GetProductsAsync()
        {
            IEnumerable<Product> products = await inventoryDbContext.Products.ToListAsync();

            return products;
        }

        [HttpPost]
        [Route("new")]
        public async Task<IActionResult> CreateProduct(Product product)
        {
            if (product != null && !string.IsNullOrEmpty(product.Name))
            {
                product.CreatedDate = DateTime.Now;
                product.UpdateDate = DateTime.Now;
                product.Version = 1;

                await inventoryDbContext.Products.AddAsync(product);
                await inventoryDbContext.SaveChangesAsync();
            }
            else
            {
                return BadRequest("Required data for new product creation not found");
            }

            return Ok("New product created");
        }
    }
}