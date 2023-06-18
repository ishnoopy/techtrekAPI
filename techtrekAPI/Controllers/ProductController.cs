using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Net;
using techtrekAPI.DTO.Product;
using techtrekAPI.Entities;

namespace techtrekAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : Controller
    {
        private readonly TechtrekContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _productImagesParentDirectory;


        public ProductController(TechtrekContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _productImagesParentDirectory = Directory.GetCurrentDirectory();
        }

        [HttpGet("getAllProducts")]
        public async Task<ActionResult<List<Product>>> GetAllProducts()
        {
            var list = await _context.products
                  .Join(
                _context.categories, product => product.category_id, category => category.id,
                (product, category) => new ProductDTO
                {
                    id = product.id,
                    category_id = product.category_id,
                    category_name = category.name,
                    name = product.name,
                    price = product.price,
                    description = product.description,
                    img_url = product.img_url,
                    stock = product.stock,
                    sold = product.sold,
                }
                ).ToListAsync();

            if (list.Count == 0)
            {
                return NotFound();
            }
            else
            {
                return Ok(list);
            }
        }

        [HttpGet("getProductById/{id}")]
        public async Task<ActionResult<ProductDTO>> GetProduct(int id)
        {
            var product = await _context.products
                .Where(product => product.id == id)
                .Join(
                _context.categories, product => product.category_id, category => category.id,
                (product, category) => new ProductDTO
                {
                    id = product.id,
                    category_id = product.category_id,
                    category_name = category.name,
                    name = product.name,
                    price = product.price,
                    description = product.description,
                    img_url = product.img_url,
                    stock = product.stock,
                    sold = product.sold,
                }
                ).SingleOrDefaultAsync();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (product == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(product);
            }
        }

        [HttpPost("createProduct")]
        [Authorize (Roles = "admin")]
        public async Task<HttpStatusCode> CreateProduct([FromForm] Product product)
        {
            var entity = new Product()
            {
                category_id = product.category_id,
                name = product.name,
                price = product.price,
                description = product.description,
                img = product.img,
                img_url = "",
                stock = product.stock,
                sold = 0
            };

            _context.products.Add(entity);
            await _context.SaveChangesAsync();

            if (product.img != null)
            {
                var mainImageFilePath = await SaveFile(product.img, entity.id);
                entity.img_url = mainImageFilePath;
            }

            // Check if any values are missing
            foreach (var property in entity.GetType().GetProperties())
            {
                if (property.GetValue(entity, null) == null)
                {
                    // Display an error message
                    var message = $"The {property.Name} property is missing.";
                    return HttpStatusCode.BadRequest;
                }
            }

            await _context.SaveChangesAsync();
            return HttpStatusCode.Created;
        }

        private async Task<string> SaveFile(IFormFile file, int productId)
        {
            var hostingEnvironment = _httpContextAccessor.HttpContext.RequestServices.GetRequiredService<IWebHostEnvironment>();
            var uploadDirectory = Path.Combine(_productImagesParentDirectory, "Uploads");
            var productDirectory = Path.Combine(uploadDirectory, productId.ToString());

            // Create the product directory if it doesn't exist
            if (!Directory.Exists(productDirectory))
            {
                Directory.CreateDirectory(productDirectory);
            }



            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(productDirectory, fileName);
            var relativePath = Path.Combine("Uploads/" + productId.ToString() + "/", fileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            return relativePath;
        }

        private async Task<string> DeleteFile(string path)
        {
            // Check if the file exists
            if (System.IO.File.Exists(path))
            {
                // Delete the file
                System.IO.File.Delete(path);
                Console.WriteLine("deleted file");
                return "deleted";
            }
            else
            {
                Console.WriteLine("file not exist");
                return "notExist";
            }
        }

        private async Task<string> DeleteFolder(string path)
        {
            // Check if the folder exists
            if (Directory.Exists(path))
            {
                // Delete the folder
                Directory.Delete(path, true);
                return "deleted";
            }
            else
            {
                return "notExist";
            }
        }

        [HttpDelete("deleteProduct/{id}")]
        [Authorize(Roles = "admin")]
        public async Task<HttpStatusCode> DeleteProduct(int id)
        {

            var entity = new Product()
            {
                id = id
            };


            var folderURL = Path.Combine(_productImagesParentDirectory, $"Uploads/{id}");
            await DeleteFolder(folderURL);


            _context.products.Remove(entity);
            await _context.SaveChangesAsync();

            return HttpStatusCode.OK;
        }

        [HttpGet("getProductsByCategory/{category_id}")]
        public async Task<ActionResult<List<ProductDTO>>> SearchProductsByCategory(int? category_id)
        {

            if (category_id != null)
            {

                var list = await _context.products
                   .Join(
                 _context.categories, product => product.category_id, category => category.id,
                 (product, category) => new ProductDTO
                 {
                     id = product.id,
                     category_id = product.category_id,
                     category_name = category.name,
                     name = product.name,
                     price = product.price,
                     description = product.description,
                     img_url = product.img_url,
                     stock = product.stock,
                     sold = product.sold
                 }
                 )
                   .Where(product => product.category_id == category_id).ToListAsync();
                return Ok(list);
            }

            return BadRequest();

        }


        [HttpPut("updateProduct/{id}")]
        [Authorize(Roles = "admin")]
        public async Task<HttpStatusCode> UpdateProduct(int id, [FromForm] Product product)
        {
            var entity = await _context.products.SingleOrDefaultAsync(product => product.id == id);
            if (entity == null)
            {
                return HttpStatusCode.NotFound;
            }

            entity.category_id = product.category_id;
            entity.name = product.name;
            entity.price = product.price;
            entity.description = product.description;
            entity.img = product.img;
            entity.img_url = entity.img_url;
            entity.stock = product.stock;
            entity.sold = 0;

            if (product.img != null)
            {
                await DeleteFile(Path.Combine(_productImagesParentDirectory, entity.img_url));
                var mainImageFilePath = await SaveFile(product.img, entity.id);
                entity.img_url = mainImageFilePath;

            }


            // Check if any values are missing
            foreach (var property in entity.GetType().GetProperties())
            {
                if (property.Name == "img") continue; // Skip the main_img property
                if (property.GetValue(entity, null) == null)
                {
                    // Display an error message
                    var message = $"The {property.Name} property is missing.";
                    return HttpStatusCode.BadRequest;
                }
            }

            await _context.SaveChangesAsync();
            return HttpStatusCode.OK;
        }

    }
}
