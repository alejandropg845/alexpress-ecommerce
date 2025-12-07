using API.DTOs.CouponDTO;
using API.DTOs.ProductDto;
using API.Entities;
using API.Interfaces.Coupon;
using API.Interfaces.Repositories;
using API.Interfaces.Repositories.Products;
using API.Interfaces.Services;
using API.Mappers;
using API.Responses.Product;
using API.Services.Secondary;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.EntityFrameworkCore;

namespace API.Services.App
{
    public class ProductService : IProductService
    {
        private readonly IProductWriteRepository _productWriteRepository;
        private readonly IProductReadRepository _productReadRepository;

        private readonly IUsersRepository _usersRepository;
        private readonly string _cs_key;
        private readonly string _cs_endpoint;
        private readonly string _cloudinary_api_key;
        private readonly string _cloudinary_api_secret;
        public ProductService(IProductWriteRepository repo, IConfiguration config, IUsersRepository usersRepository, IProductReadRepository productReadRepository)
        {
            _productReadRepository = productReadRepository;
            _productWriteRepository = repo;
            _cs_endpoint = config["contentSafety:cs_endpoint"]!;
            _cs_key = config["contentSafety:cs_key"]!;
            _cloudinary_api_key = config["cloudinary:cloudinary_api_key"]!;
            _cloudinary_api_secret = config["cloudinary:cloudinary_api_secret"]!;
            _usersRepository = usersRepository;
        }
        public async Task<List<ToProductThumbnailDto>> GetUserProductsAsync(string userId)
        => await _productReadRepository.GetUserProductsAsync(userId);
        public async Task<List<ToProductThumbnailDto>> GetProductsAsync(string? title, int categoryId, string? userId, decimal price)
        {
            var productsDto = await _productReadRepository.GetProductsAsync(title, categoryId, userId, price);

            var usersIds = productsDto.Select(p => p.AppUserId).Distinct();

            

            var usernames = await _usersRepository.GetUsernamesForProductsAsync(usersIds);

            foreach (var product in productsDto)
                product.Username = usernames.GetValueOrDefault(product.AppUserId)!;

            return productsDto;
        }

        public async Task<ToProductDto?> GetProductAsync(int id)
        {
            var product = await _productReadRepository.GetProductDtoAsync(id);

            if (product is not null)
            {
                string? username = await _usersRepository.GetUsernameForProductAsync(product.AppUserId);

                product.Username = username!;

                return product;
            }

            return null;

        }

        public async Task<ToProductDto?> GetProductDtoToUpdateAsync(int id, string appUserId)
        => await _productReadRepository.GetProductDtoToUpdateAsync(id, appUserId);
        public async Task<CreateProductResponse> CreateProductAsync(CreateProductDto dto, string userId, string username)
        {
            var response = new CreateProductResponse();

            response.UserExists = await _usersRepository.UserExistsAsync(userId);

            if (!response.UserExists) return response;


            (bool isExplicitImage,
            bool isExplicitDescription,
            bool isExplicitTitle) = await dto.Images.VerifyContentAsync(
                _cs_endpoint,
                _cs_key,
                dto.Description,
                dto.Title
            );

            response.IsExplicitImage = isExplicitImage;
            response.IsExplicitDescription = isExplicitDescription;
            response.IsExplicitTitle = isExplicitTitle;

            if (isExplicitImage || isExplicitDescription || isExplicitTitle) return response;

            dto.Coupon = AssessCoupon(dto.Coupon);

            /* TODO en el frontend convertir valores que comiencen con 0 automáticamente */

            var newProduct = dto.ToProduct(userId, username);

            var addedProduct = await _productWriteRepository.AddProductAsync(newProduct);

            var productDto = await GetProductAsync(addedProduct.Id);

            response.ProductDto = productDto!;

            return response;
        }
        private static T AssessCoupon<T>(T dto) where T : ICouponDto
        {
            /* Si coupon no es null, ponemos discount en 0 */
            if (dto.CouponName is not null) dto.Discount = 0;

            /* Si discount contiene un valor, coupon debe ser false por completo */
            if (dto.Discount > 0) dto.CouponName = null;

            if (!string.IsNullOrEmpty(dto.CouponName) && dto.CouponName != "is50OffOneProduct" && dto.CouponName != "isFreeShipping" && dto.CouponName != "is50Discount")

                throw new Exception("Invalid coupon name");

            return dto;
            
        }
        public async Task<UpdateProductResponse> UpdateProductAsync(int id, UpdateProductDto dto, string userId)
        {
            var response = new UpdateProductResponse();

            response.UserExists = await _usersRepository.UserExistsAsync(userId);


            if (!response.UserExists) return response;

            var product = await _productWriteRepository.GetProductAsync(id);

            response.ProductExists = product is not null;

            if (product is null) return response;


            (bool isExplicitImage,
            bool isExplicitDescription,
            bool isExplicitTitle) = await dto.Images.VerifyContentAsync(
                _cs_endpoint,
                _cs_key,
                dto.Description,
                dto.Title
            );

            response.IsExplicitContent = isExplicitImage || isExplicitDescription || isExplicitTitle;

            if (response.IsExplicitContent) return response;

            dto.Coupon = AssessCoupon(dto.Coupon);

            product.Images = dto.Images;
            product.Title = dto.Title;
            product.Description = dto.Description;
            product.Price = dto.Price;
            product.ShippingPrice = dto.ShippingPrice;
            product.CategoryId = dto.CategoryId;
            product.ConditionId = dto.ConditionId;
            product.Stock = dto.Stock;
            product.Coupon.Discount = dto.Coupon.Discount;
            product.Coupon.CouponName = dto.Coupon.CouponName;

            await _productWriteRepository.SaveProductChangesAsync();

            var productDto = await GetProductAsync(product.Id);

            response.ProductDto = productDto!;

            return response;
        }

        public async Task<DeleteProductResponse> DeleteProductAsync(int id, string userId)
        {
            var response = new DeleteProductResponse();

            var userExists = await _usersRepository.UserExistsAsync(userId);

            response.IsUserDisabled = !userExists;
            if (!userExists) return response;

            var productExists = await _productReadRepository.ProductExistsAsync(id);

            response.ProductExists = productExists;
            if (!response.ProductExists) return response;

            await _productWriteRepository.SetProductAsDeletedAsync(id);

            return response;

        }

        public async Task DeleteFromCloudinaryAsync(string publicId)
        {
            var account = new Account
            {
                ApiKey = _cloudinary_api_key,
                ApiSecret = _cloudinary_api_secret,
                Cloud = "dyihpj2hw"
            };

            var cloudinary = new Cloudinary(account);

            var deletionParams = new DeletionParams(publicId);

            await cloudinary.DestroyAsync(deletionParams);
        }
    }

}