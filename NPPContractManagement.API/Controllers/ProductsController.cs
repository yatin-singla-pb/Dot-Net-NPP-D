using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NPPContractManagement.API.DTOs;
using NPPContractManagement.API.Services;
using System.Security.Claims;

namespace NPPContractManagement.API.Controllers
{
    /// <summary>
    /// Controller for managing Products
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        /// <summary>
        /// Get paginated Products (active only by default)
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<NPPContractManagement.API.Repositories.PaginatedResult<ProductDto>>> GetAllProducts(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? sortBy = "Name",
            [FromQuery] string sortDirection = "asc",
            [FromQuery] string? searchTerm = null,
            [FromQuery] string? status = null,
            [FromQuery] int? manufacturerId = null,
            [FromQuery] string? brand = null)
        {
            try
            {
                if (pageNumber < 1) pageNumber = 1;
                if (pageSize < 1 || pageSize > 100) pageSize = 10;

                int? statusInt = null;
                if (!string.IsNullOrWhiteSpace(status))
                {
                    var val = status.Trim();
                    if (val.Equals("Active", StringComparison.OrdinalIgnoreCase)) statusInt = 1;
                    else if (val.Equals("Inactive", StringComparison.OrdinalIgnoreCase)) statusInt = 2;
                    else if (val.Equals("Pending", StringComparison.OrdinalIgnoreCase)) statusInt = 3;
                    else if (val.Equals("Discontinued", StringComparison.OrdinalIgnoreCase)) statusInt = 4;
                }

                var (products, totalCount) = await _productService.SearchProductsAsync(
                    searchTerm ?? string.Empty,
                    manufacturerId,
                    statusInt,
                    null,
                    brand,
                    pageNumber,
                    pageSize
                );

                // Optional server-side sorting fallback (Name by default)
                IEnumerable<ProductDto> sorted = products;
                if (!string.IsNullOrWhiteSpace(sortBy))
                {
                    var dir = sortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase) ? -1 : 1;
                    sorted = sortBy switch
                    {
                        "Name" => dir == 1 ? products.OrderBy(p => p.Name) : products.OrderByDescending(p => p.Name),
                        "ManufacturerProductCode" => dir == 1 ? products.OrderBy(p => p.ManufacturerProductCode) : products.OrderByDescending(p => p.ManufacturerProductCode),
                        "Category" => dir == 1 ? products.OrderBy(p => p.Category) : products.OrderByDescending(p => p.Category),
                        _ => products
                    };
                }

                var response = new NPPContractManagement.API.Repositories.PaginatedResult<ProductDto>
                {
                    Items = sorted.ToList(),
                    TotalCount = totalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving Products", error = ex.Message });
            }
        }

        /// <summary>
        /// Get Product by ID
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <returns>Product details</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> GetProductById(int id)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(id);
                if (product == null)
                {
                    return NotFound(new { message = "Product not found" });
                }
                return Ok(product);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the Product", error = ex.Message });
            }
        }

        /// <summary>
        /// Create a new Product
        /// </summary>
        /// <param name="createProductDto">Product creation data</param>
        /// <returns>Created Product</returns>
        [HttpPost]
        public async Task<ActionResult<ProductDto>> CreateProduct([FromBody] CreateProductDto createProductDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var createdBy = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "System";
                var product = await _productService.CreateProductAsync(createProductDto, createdBy);
                return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, product);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the Product", error = ex.Message });
            }
        }

        /// <summary>
        /// Update an existing Product
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <param name="updateProductDto">Product update data</param>
        /// <returns>Updated Product</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<ProductDto>> UpdateProduct(int id, [FromBody] UpdateProductDto updateProductDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var modifiedBy = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "System";
                var product = await _productService.UpdateProductAsync(id, updateProductDto, modifiedBy);
                return Ok(product);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the Product", error = ex.Message });
            }
        }

        /// <summary>
        /// Soft-delete (deactivate) a Product
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <returns>Success status</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            try
            {
                var modifiedBy = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "System";
                var result = await _productService.DeactivateProductAsync(id, modifiedBy);
                if (!result)
                {
                    return NotFound(new { message = "Product not found" });
                }
                return Ok(new { message = "Product deactivated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the Product", error = ex.Message });
            }
        }

        /// <summary>
        /// Activate a Product
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <returns>Success status</returns>
        [HttpPatch("{id}/activate")]
        public async Task<ActionResult> ActivateProduct(int id)
        {
            try
            {
                var modifiedBy = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "System";
                var result = await _productService.ActivateProductAsync(id, modifiedBy);
                if (!result)
                {
                    return NotFound(new { message = "Product not found" });
                }
                return Ok(new { message = "Product activated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while activating the Product", error = ex.Message });
            }
        }

        /// <summary>
        /// Deactivate a Product
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <returns>Success status</returns>
        [HttpPatch("{id}/deactivate")]
        public async Task<ActionResult> DeactivateProduct(int id)
        {
            try
            {
                var modifiedBy = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "System";
                var result = await _productService.DeactivateProductAsync(id, modifiedBy);
                if (!result)
                {
                    return NotFound(new { message = "Product not found" });
                }
                return Ok(new { message = "Product deactivated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deactivating the Product", error = ex.Message });
            }
        }

        /// <summary>
        /// Get Product by SKU
        /// </summary>
        /// <param name="sku">Product SKU</param>
        /// <returns>Product details</returns>
        [HttpGet("sku/{sku}")]
        public async Task<ActionResult<ProductDto>> GetProductBySKU(string sku)
        {
            try
            {
                var product = await _productService.GetProductBySKUAsync(sku);
                if (product == null)
                {
                    return NotFound(new { message = "Product not found" });
                }
                return Ok(product);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the Product", error = ex.Message });
            }
        }

        /// <summary>
        /// Get Product by GTIN
        /// </summary>
        /// <param name="gtin">Product GTIN</param>
        /// <returns>Product details</returns>
        [HttpGet("gtin/{gtin}")]
        public async Task<ActionResult<ProductDto>> GetProductByGTIN(string gtin)
        {
            try
            {
                var product = await _productService.GetProductByGTINAsync(gtin);
                if (product == null)
                {
                    return NotFound(new { message = "Product not found" });
                }
                return Ok(product);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the Product", error = ex.Message });
            }
        }

        /// <summary>
        /// Get Product by UPC
        /// </summary>
        /// <param name="upc">Product UPC</param>
        /// <returns>Product details</returns>
        [HttpGet("upc/{upc}")]
        public async Task<ActionResult<ProductDto>> GetProductByUPC(string upc)
        {
            try
            {
                var product = await _productService.GetProductByUPCAsync(upc);
                if (product == null)
                {
                    return NotFound(new { message = "Product not found" });
                }
                return Ok(product);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the Product", error = ex.Message });
            }
        }

        /// <summary>
        /// Get Product by manufacturer product code
        /// </summary>
        /// <param name="manufacturerProductCode">Manufacturer Product Code</param>
        /// <returns>Product details</returns>
        [HttpGet("manufacturer-code/{manufacturerProductCode}")]
        public async Task<ActionResult<ProductDto>> GetProductByManufacturerProductCode(string manufacturerProductCode)
        {
            try
            {
                var product = await _productService.GetProductByManufacturerProductCodeAsync(manufacturerProductCode);
                if (product == null)
                {
                    return NotFound(new { message = "Product not found" });
                }
                return Ok(product);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the Product", error = ex.Message });
            }
        }

        /// <summary>
        /// Get Products by manufacturer ID
        /// </summary>
        /// <param name="manufacturerId">Manufacturer ID</param>
        /// <returns>List of Products for the manufacturer</returns>
        [HttpGet("manufacturer/{manufacturerId}")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProductsByManufacturerId(int manufacturerId)
        {
            try
            {
                var products = await _productService.GetProductsByManufacturerIdAsync(manufacturerId);
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving Products", error = ex.Message });
            }
        }

        /// <summary>
        /// Get Products by status
        /// </summary>
        /// <param name="status">Status (1=Active, 2=Inactive)</param>
        /// <returns>List of Products with the specified status</returns>
        [HttpGet("status/{status}")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProductsByStatus(int status)
        {
            try
            {
                var products = await _productService.GetProductsByStatusAsync(status);
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving Products", error = ex.Message });
            }
        }

        /// <summary>
        /// Get Products by category
        /// </summary>
        /// <param name="category">Product category</param>
        /// <returns>List of Products in the specified category</returns>
        [HttpGet("category/{category}")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProductsByCategory(string category)
        {
            try
            {
                var products = await _productService.GetProductsByCategoryAsync(category);
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving Products", error = ex.Message });
            }
        }

        /// <summary>
        /// Get active Products
        /// </summary>
        /// <returns>List of active Products</returns>
        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetActiveProducts()
        {
            try
            {
                var products = await _productService.GetActiveProductsAsync();
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving active Products", error = ex.Message });
            }
        }

        /// <summary>
        /// Search Products with pagination and filtering
        /// </summary>
        /// <param name="searchTerm">Search term</param>
        /// <param name="manufacturerId">Filter by manufacturer ID</param>
        /// <param name="status">Filter by status</param>
        /// <param name="category">Filter by category</param>
        /// <param name="brand">Filter by brand</param>
        /// <param name="page">Page number (default: 1)</param>
        /// <param name="pageSize">Page size (default: 10)</param>
        /// <returns>Paginated list of Products</returns>
        [HttpGet("search")]
        public async Task<ActionResult<object>> SearchProducts(
            [FromQuery] string searchTerm = "",
            [FromQuery] int? manufacturerId = null,
            [FromQuery] int? status = null,
            [FromQuery] string? category = null,
            [FromQuery] string? brand = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var (products, totalCount) = await _productService.SearchProductsAsync(searchTerm, manufacturerId, status, category, brand, page, pageSize);

                return Ok(new
                {
                    data = products,
                    totalCount,
                    page,
                    pageSize,
                    totalPages = (int)Math.Ceiling((double)totalCount / pageSize)
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while searching Products", error = ex.Message });
            }
        }

        /// <summary>
        /// Get Products by multiple manufacturer IDs (deduplicated)
        /// </summary>
        [HttpPost("by-manufacturers")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProductsByManufacturers([FromBody] int[] manufacturerIds)
        {
            try
            {
                var ids = manufacturerIds ?? Array.Empty<int>();
                var products = await _productService.GetProductsByManufacturerIdsAsync(ids);
                var items = products.GroupBy(p => p.Id).Select(g => g.First()).OrderBy(p => p.Name).ToList();
                return Ok(items);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving Products", error = ex.Message });
            }
        }

        /// <summary>
        /// Get distinct brand names for autocomplete
        /// </summary>
        [HttpGet("brands")]
        public async Task<ActionResult<IEnumerable<string>>> GetDistinctBrands()
        {
            try
            {
                var brands = await _productService.GetDistinctBrandsAsync();
                return Ok(brands);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving brands", error = ex.Message });
            }
        }

    }
}
