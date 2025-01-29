using ECommerce.SharedLibrary.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProduitApi.Application.DTOs;
using ProduitApi.Application.DTOs.Conversion;
using ProduitApi.Application.Interfaces;

namespace ProduitApi.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class ProductController(IProduct productInterface) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProducts()
        {
            var products = await productInterface.GetAllAsync();
            if (!products.Any())
            {
                return NotFound("No products detected in the database");
            }

            var (_, list) = ProductConversion.FromEntity(null!, products);
            return list!.Any() ? Ok(list) : NotFound("No product found");
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProduct(int id)
        {
            var product = await productInterface.FindByIdAsync(id);
            if (product is null)
            {
                return NotFound("Product requested not found");
            }

            var (p, _) = ProductConversion.FromEntity(product, null!);

            return p is not null ? Ok(product) : NotFound("Product not found");
        }

        [HttpPost]
        [Authorize(Roles ="Admin")]
        public async Task<ActionResult<Response>> CreateProduct(ProductDTO productDTO)
        {
            // check model state is all data annotations are passed
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // convert to entity
            var getEntity = ProductConversion.ToEntity(productDTO);
            var response = await productInterface.CreateAsync(getEntity);
            return response.Flag is true ? Ok(response) : BadRequest(response);
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Response>> UpdateProduct(ProductDTO productDTO)
        {
            // check model state is all data annotations are passed
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // convert to entity
            var getEntity = ProductConversion.ToEntity(productDTO);
            var response = await productInterface.UpdateAsync(getEntity);
            return response.Flag is true ? Ok(response) : BadRequest(response);
        }

        [HttpDelete]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Response>> DeleteProduct(ProductDTO productDTO)
        {
            // convert to entity
            var getEntity = ProductConversion.ToEntity(productDTO);
            var response = await productInterface.DeleteAsync(getEntity);
            return response.Flag is true ? Ok(response) : BadRequest(response);
        }
    }
}
