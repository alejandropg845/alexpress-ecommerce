using API.DTOs.CartDTO;
using API.DTOs.CartProductsDTO;
using API.Entities;
using API.Interfaces.Factories;
using API.Interfaces.Repositories;
using API.Interfaces.Repositories.Products;
using API.Interfaces.Services;
using API.Responses.Cart;

namespace API.Services.App
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IProductWriteRepository _productRepository;
        private readonly ICouponStrategyFactory _couponStrategyFactory;
        public CartService(ICartRepository cr, IProductWriteRepository productRepository, ICouponStrategyFactory f)
        {
            _cartRepository = cr;
            _productRepository = productRepository;
            _couponStrategyFactory = f;
        }
        public async Task<ToCartDto?> GetCartAsync(string userId)
        => await _cartRepository.GetCartAsync(userId);

        public async Task<AddToCartResponse> AddToCartAsync(string userId, AddToCartDto dto)
        {
            var response = new AddToCartResponse();

            // Verificar que el product a agregar existe
            var product = await _productRepository.GetProductAsync(dto.ProductId);

            response.ProductExists = product is not null;
            if (!response.ProductExists) return response;

            // Verificar que el cart existe
            Cart? cart = await _cartRepository.GetCartIfExistsAsync(userId);

            if (cart is null)
            {
                cart = new Cart { AppUserId = userId };

                _cartRepository.AddCart(cart);
            }

            if (product!.AppUserId == userId) // <== Usuario agrega productos propios
            {
                response.OwnProduct = true;
                return response;
            }

            var productInCart = await _cartRepository.GetProductInCartIfExistsAsync(cart!.Id, dto.ProductId);

            if (productInCart == null) // El producto no está en el carrito
            {
                var newCartProduct = new CartProduct
                {
                    CartId = cart.Id,
                    ProductId = product.Id,
                    CustomizedDiscount = dto.CustomizedDiscount,
                    Image = product.Images.First(),
                    ShippingPrice = product.ShippingPrice,
                    Title = product.Title,
                    CouponName = dto.CouponName,
                    Price = product.Price
                };

                var r = ValidateAddingCartProductAsync(newCartProduct, dto.Quantity, product);

                response.NoMoreStock = r.NoMoreStock;
                response.TwoOrMoreUnitsRequired = r.TwoOrMoreUnitsRequired;
                response.IsProductRemoved = r.IsProductRemoved;

                if (r.OwnProduct || r.NoMoreStock || r.TwoOrMoreUnitsRequired)
                    return response;

                newCartProduct.Quantity = dto.Quantity <= 0 ? 1 : dto.Quantity; // <== Agregar Quantity a newProduct

                /* Agregamos 0 en wantedQuantity ya que el producto no existía en el carrito
                 * por lo que no se está agregando cantidad sino la misma quantity del product */
                CalculateCartProductPrice(newCartProduct, dto.Quantity, dto.CouponName, dto.CustomizedDiscount);

                cart.CartProducts.Add(newCartProduct);

                ModifyCartSummary(cart, newCartProduct.NewPrice);

                response.Summary = cart.Summary;

                response.CartProduct = newCartProduct;

            }
            else // <== El producto está en el carrito
            {
                if (dto.Quantity == -1 && productInCart.Quantity == 1) // <== Se quita en su totalidad el producto
                {
                    cart.CartProducts.Remove(productInCart);
                    response.IsProductRemoved = true;
                }

                var r = ValidateAddingCartProductAsync(productInCart, dto.Quantity, product);

                response.OwnProduct = r.OwnProduct;
                response.NoMoreStock = r.NoMoreStock;
                response.TwoOrMoreUnitsRequired = r.TwoOrMoreUnitsRequired;

                if (r.OwnProduct || r.NoMoreStock || r.TwoOrMoreUnitsRequired)
                    return response;

                if (!response.IsProductRemoved)
                {
                    int totalQuantity = productInCart.Quantity + dto.Quantity;

                    decimal oldPrice = productInCart.NewPrice;

                    CalculateCartProductPrice(productInCart, totalQuantity, dto.CouponName, dto.CustomizedDiscount);

                    productInCart.Quantity += dto.Quantity; // <== marcamos la nueva cantidad a Quantity

                    decimal amountToAdd = productInCart.NewPrice - oldPrice; 

                    ModifyCartSummary(cart, amountToAdd);

                    /* Reseteamos el coupon por si el usuario añadió quantity en una interfaz donde
                     * el cupon no podia ser seleccionado, en este caso, se eliminaría el coupon. */
                    ResetCoupon(productInCart, dto);
                }
                else ModifyCartSummary(cart, -productInCart.NewPrice);

                response.Summary = cart.Summary;
                response.CartProduct = productInCart;
            }

            await _cartRepository.SaveContextChangesAsync();

            return response;
        }

        private static void ResetCoupon(CartProduct productInCart, AddToCartDto dto)
        {

            if (dto.CouponName is not null && dto.CustomizedDiscount > 0)

                throw new Exception("Both Coupon and discount cannot have values");
            
            productInCart.CouponName = dto.CouponName;
               
            productInCart.CustomizedDiscount = dto.CustomizedDiscount;

        }

        private static AddToCartResponse ValidateAddingCartProductAsync(CartProduct productInCart, int wantedQuantity, Product product)
        {
            var response = new AddToCartResponse();

            /* Se agrega más cantidad que la stock */

            if (product.Stock < productInCart.Quantity + wantedQuantity)
            {
                response.NoMoreStock = true;
                return response;
            }

            /* El usuario agrega la cantidad de CartProduct menor a la que requiere el coupon */

            if (productInCart.CouponName == "is50OffOneProduct" && wantedQuantity > 0)
            
                if (productInCart.Quantity + wantedQuantity < 2)
                {
                    response.TwoOrMoreUnitsRequired = true;
                    return response;
                }

            /* El usuario quita cantidad a un CartProduct que tiene un cupón que quiere al menos dos unidades */
            if (productInCart.CouponName == "is50OffOneProduct" && wantedQuantity < 0)

                if (productInCart.Quantity == 2)
                {
                    response.TwoOrMoreUnitsRequired = true;
                    return response;
                }

            return response;
        }

        private void ModifyCartSummary(Cart cart, decimal price)
        {
            cart.Summary += price;

            if (cart.Id is 0) // <== Si Cart.Id es 0, quiere decir que el cart es nuevo (ef lo evalúa como Added)

                _cartRepository.AddCart(cart);
        }

        private void CalculateCartProductPrice(CartProduct cartProduct, int wantedQuantity, string? couponName, int discount)
        {
            
            if (discount == 0)
            {
                var strategy = _couponStrategyFactory.GetStrategy(couponName);

                cartProduct.NewPrice = strategy.CalculatePrice(
                    cartProduct.Price,
                    wantedQuantity,
                    cartProduct.ShippingPrice
                );
                return;
            }

            /* Hay discount */
            decimal dis = cartProduct.Price * wantedQuantity * discount / 100;

            cartProduct.NewPrice = (cartProduct.Price * wantedQuantity) - dis + cartProduct.ShippingPrice;

            
                
            
        }

        public async Task<RemoveCartProductResponse> RemoveCartProductAsync(int productId, string userId)
        {
            var response = new RemoveCartProductResponse();

            Cart? cart = await _cartRepository.GetCartIfExistsAsync(userId);

            var cartProduct = await _cartRepository.GetProductInCartIfExistsAsync(cart!.Id, productId);

            response.CartProductExists = cartProduct is not null;
            if (!response.CartProductExists) return response;


            cart!.CartProducts.Remove(cartProduct!);

            ModifyCartSummary(cart, - cartProduct!.NewPrice);

            await _cartRepository.SaveContextChangesAsync();

            response.Summary = cart.Summary;

            return response;
        }
        
    }
}
