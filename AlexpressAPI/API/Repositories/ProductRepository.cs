using API.DbContexts;
using API.DTOs.CartProductsDTO;
using API.DTOs.CouponDTO;
using API.DTOs.ProductDto;
using API.DTOs.ReviewDTO;
using API.Entities;
using API.Interfaces.Repositories.Products;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories
{
    public class ProductRepository : IProductWriteRepository, IProductReadRepository
    {
        private readonly ProductDbContext _context;

        public ProductRepository(ProductDbContext context)
        {
            _context = context;
        }
        public async Task<List<ToProductThumbnailDto>> GetUserProductsAsync(string userId)
        {
            return await _context.Products
                .Where(p => p.AppUserId == userId && !p.IsDeleted)
                .Select(p => new ToProductThumbnailDto
                {
                    AppUserId = p.AppUserId,
                    Username = p.Username,
                    Title = p.Title,
                    Price = p.Price,
                    Category = p.Category.Name,
                    Condition = p.Condition.Name,
                    ShippingPrice = p.ShippingPrice,
                    Image = p.Images[0],
                    Stock = p.Stock,
                    Sold = p.Sold,
                    Accumulated = p.Accumulated,
                    Votes = p.Votes,
                    Id = p.Id
                }).ToListAsync();

        }
        public async Task<int> KeepAliveAsync()
        {
            int productId = await _context.Products
                .Where(p => p.Id == 1).Select(p => p.Id).FirstAsync();

            return productId;
        }
        public async Task<List<ToProductThumbnailDto>> GetProductsAsync(string? title, int categoryId, string? userId, decimal price)
        {
            IQueryable<Product> productsQuery = _context.Products
                .Where(p => !p.IsDeleted);

            
            if (!string.IsNullOrWhiteSpace(title) && title != "null") // <== Filtrar por titulo en el buscador

                productsQuery = productsQuery.Where(p => p.Title.Contains(title));


            if (categoryId != 0) // <== Filtrar por categoría
            
                productsQuery = productsQuery.Where(p => p.CategoryId == categoryId);

            if (price != 0) // <== filtrar por precio menor a 4

                productsQuery = productsQuery.Where(p => p.Price < price);

            var productsDto = await ApplySelect(productsQuery);

            return productsDto;
        }

        private async static Task<List<ToProductThumbnailDto>> ApplySelect(IQueryable<Product> queryable)
        {
            return await queryable.Select(p => new ToProductThumbnailDto
            {
                AppUserId = p.AppUserId,
                Username = p.Username,
                Title = p.Title,
                Price = p.Price,
                Category = p.Category.Name,
                Condition = p.Condition.Name,
                ShippingPrice = p.ShippingPrice,
                Image = p.Images[0],
                Stock = p.Stock,
                Sold = p.Sold,
                Accumulated = p.Accumulated,
                Votes = p.Votes,
                Id = p.Id
            }).ToListAsync();

        }
        public async Task<Product> AddProductAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return product;
        }
        public async Task SaveProductChangesAsync() => await _context.SaveChangesAsync();
        public async Task<Product?> GetProductAsync(int id)
        => await _context.Products
            .Include(p => p.Coupon)
            .FirstOrDefaultAsync(p => p.Id == id);
        public async Task<ToProductDto?> GetProductDtoAsync(int id)
        {
            ToProductDto? product = await _context.Products
                .Where(p => p.Id == id && !p.IsDeleted)
                .Select(p => new ToProductDto
                {
                    Id = p.Id,
                    Accumulated = p.Accumulated,
                    AppUserId = p.AppUserId,
                    Category = p.Category.Name,
                    CategoryId = p.CategoryId,
                    ConditionId = p.ConditionId,
                    Condition = p.Condition.Name,
                    IsDeleted = p.IsDeleted,
                    Coupon = new CouponDto
                    {
                        Discount = p.Coupon.Discount,
                        CouponName = p.Coupon.CouponName
                    },
                    Description = p.Description,
                    Images = p.Images,
                    Price = p.Price,
                    Reviews = p.Reviews.Select(r => new ReviewItemDto
                    {
                        Author = r.Author,
                        Comment = r.Comment,
                        Rating = r.Rating,
                        CreatedAt = r.CreatedAt
                    }).ToList(),
                    Votes = p.Votes,
                    ShippingPrice = p.ShippingPrice,
                    Sold = p.Sold,
                    Stock = p.Stock,
                    Title = p.Title,
                    Username = p.Username,
                }).FirstOrDefaultAsync();

            return product;
        }

        public async Task<ToProductDto?> GetProductDtoToUpdateAsync(int id, string appUserId)
        {
            ToProductDto? product = await _context.Products
                .Where(p => p.Id == id && !p.IsDeleted && p.AppUserId == appUserId)
                .Select(p => new ToProductDto
                {
                    Id = p.Id,
                    Images = p.Images,
                    Title = p.Title,
                    Description = p.Description,
                    Price = p.Price,
                    CategoryId = p.CategoryId,
                    ConditionId = p.ConditionId,
                    ShippingPrice = p.ShippingPrice,
                    Stock = p.Stock,
                    Coupon = new CouponDto
                    {
                        Discount = p.Coupon.Discount,
                        CouponName = p.Coupon.CouponName
                    },
                }).FirstOrDefaultAsync();

            return product;
        }
        public void UpdateProductAsync(Product product)
        => _context.Products.Update(product);
        
        public async Task<bool> ProductExistsAsync(int id)
        => await _context.Products.Where(p => p.Id == id).AnyAsync();
        public async Task SetProductAsDeletedAsync(int id)
        {
            await _context.Products.Where(p => p.Id == id)
                .ExecuteUpdateAsync(p => p.SetProperty(p => p.IsDeleted, true));
        }

        public async Task UpdateOrderedProductsAsync(List<CartProductDto> orderedProducts)
        {
            var ids = orderedProducts.Select(op => op.ProductId).ToList();

            var products = await _context.Products
                .Where(p => ids.Contains(p.Id))
                .ToListAsync();

            foreach (var product in products)
            {
                product.Stock -= orderedProducts.Find(op => op.ProductId == product.Id)!.Quantity;
                product.Sold += orderedProducts.Find(op => op.ProductId == product.Id)!.Quantity;
            }
        }

        public async Task<(bool IsStock, string? ErrorMessage)> IsStockAvailableAsync(List<CartProductDto> orderedProducts)
        {
            List<int> ids = orderedProducts.Select(p => p.ProductId).ToList();

            var liteProducts = await _context.Products
                .Where(p => ids.Contains(p.Id))
                .Select(p => new { p.Stock, p.IsDeleted, p.Id, p.RowVersion })
                .ToListAsync();

            foreach (var orderedProduct in orderedProducts)
            {
                var product = liteProducts.Find(p => p.Id == orderedProduct.ProductId)!;

                if (orderedProduct.Quantity > product.Stock)

                    return new (false, $"The product \"{orderedProduct.Title.Remove(15)}...\" " +
                        $"has {product.Stock} stock(s) available. You selected {orderedProduct.Quantity}");

                if (product.IsDeleted)
                    return new(false, $"The product \"{orderedProduct.Title.Remove(10)}...\" is not available");
            }

            return new (true, null);
           
        }

        public async Task SetProductsReviewAsync(List<int> productsIds, int rating)
        {
            var products = await _context.Products.Where(p => productsIds.Contains(p.Id)).ToListAsync();

            foreach(var product in products)
            {
                product.Votes++;
                product.Accumulated += rating;
            }

        }
    }
}
