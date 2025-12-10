using API.DTOs.CartProductsDTO;
using API.DTOs.OrderDTO;
using API.Entities;
using API.Interfaces.Services;
using API.Payloads.Order;
using API.Responses.Order;
using API.UnitsOfWork.Interfaces;
using Microsoft.Extensions.Primitives;
using Stripe;
using Stripe.Checkout;
using System.Text.Json;

namespace API.Services.App
{
    public class OrderService : IOrderService
    {
        private readonly IOrderUnitOfWork _uow;
        private readonly string _secret_key;
        private readonly string _webHookSecret;
        private readonly string _successUrl;
        private readonly string _errorUrl;
        private readonly ILogger<OrderService> _logger;
        public OrderService(IOrderUnitOfWork uow, IConfiguration config, ILogger<OrderService> logger)
        {
            _uow = uow;
            _secret_key = config["Stripe:Secret_key"]!;
            _webHookSecret = config["Stripe:Web_hook_secret"]!;
            _successUrl = config["Stripe:SuccessUrl"]!;
            _errorUrl = config["Stripe:ErrorUrl"]!;
            _logger = logger;
        }
        public async Task<IReadOnlyList<OrderDto>> GetOrdersAsync(string userId)
        => await _uow.OrderRepository.GetOrdersAsync(userId);
        
        public async Task HandleStripeWebHook(string json, StringValues stripeSignature)
        {
            var stripeEvent = EventUtility.ConstructEvent(json, stripeSignature, _webHookSecret);

            if (stripeEvent.Type == "checkout.session.completed")
            {
                var session = stripeEvent.Data.Object as Session;

                if (session is null) return;

                string userId = session.Metadata["userId"];

                int addressId = Convert.ToInt32(session.Metadata["addressId"]);

                string email = session.Metadata["email"];

                string username = session.Metadata["username"];


                var stripeSessionId = session.Id;


                bool isOrderCreated = await _uow.OrderRepository.IsOrderAlreadyCreatedAsync(stripeSessionId);

                if (isOrderCreated)
                {
                    _logger.LogWarning("YA EXISTE ORDER CON ID {id}", stripeSessionId);
                    return;
                }

                _logger.LogWarning("NO EXISTE ORDER CON ID {id}", stripeSessionId);


                var response = await CreateOrderAsync(
                    addressId, 
                    userId, 
                    email, 
                    stripeSessionId,
                    username
                );

            }

        }

        public async Task<CreateOrderResponse> CreateOrderAsync(int addressId, string userId, string email, string stripeSessionId, string username)
        {
            var response = new CreateOrderResponse();

            response.UserExists = await _uow.UsersRepository.UserExistsAsync(userId);


            if (!response.UserExists) return response;

            var orderedCart = await _uow.CartRepository.GetOrderedCartDtoAsync(userId);

            response.CartIsNull = orderedCart is null;


            if (orderedCart == null) return response;

            response.CartIsNull = orderedCart.Summary is 0 || !orderedCart.CartProducts.Any();


            if (response.CartIsNull) return response;

            using var transaction = await _uow.BeginTransactionAsync();

            try
            {

                var order = new Order
                {
                    AddressId = addressId,
                    AppUserId = userId,
                    CreatedOn = DateTimeOffset.UtcNow,
                    Rating = 0,
                    Summary = orderedCart.Summary,
                    StripeSessionId = stripeSessionId ?? "testing value",
                    OrderedProducts = orderedCart.CartProducts!.Select(cp => new OrderedProduct
                    {
                        Image = cp.Image,
                        Price = cp.Price,
                        ProductId = cp.ProductId,
                        Quantity = cp.Quantity,
                        Title = cp.Title,
                    }).ToList()
                };

                /* Guardar Order */
                _uow.OrderRepository.SaveOrder(order);

                /* Verificar stock y disponibilidad de cada producto */
                var isStock = await _uow.ProductReadRepository.IsStockAvailableAsync(orderedCart.CartProducts);

                response.IsStockAvailable = isStock.IsStock;
                response.ErrorMessage = isStock.ErrorMessage;
                
                if (!isStock.IsStock)
                {
                    await _uow.RollbackTransactionAsync(transaction);
                    return response;
                }


                /* Actualizar Sold y Quantity de cada Product en el carrito */
                await _uow.ProductWriteRepository.UpdateOrderedProductsAsync(orderedCart.CartProducts!);

                /* Vaciar carrito */
                await _uow.CartRepository.EmptyUserCartAsync(orderedCart.Id); // <== Vaciar carrito 

                /* Agregar el OutboxMessage */
                _uow.OutboxMessageRepository.SaveOutboxMessage(
                    CreatePayload(
                        orderedCart.Summary, 
                        email,
                        username,
                        orderedCart.CartProducts
                    )
                );

                /* Commit */
                await _uow.SaveAllChangesAsync();

                await _uow.CommitTransactionAsync(transaction);

  

                return response;

            
            } catch (Exception e)
            {
                await _uow.RollbackTransactionAsync(transaction);
                response.ErrorMessage = e.Message;
                return response;
            }


        }

        private OutboxMessage CreatePayload(decimal summary, string email, string username, List<CartProductDto> orderedProducts)
        {

            var orderMail = new OrderMail
            {
                Email = email,
                Summary = summary,
                Username = username,
                OrderedProducts = [.. orderedProducts.Select(op => new OrderedProductDto
                {
                    ShippingPrice = op.ShippingPrice,
                    Image = op.Image,
                    Price = op.Price,
                    Quantity = op.Quantity,
                    Title = op.Title,
                    ProductId = op.ProductId,
                    CouponName = op.CouponName is null && op.CustomizedDiscount is 0 
                    ? null 
                    : op.CouponName ?? op.CustomizedDiscount.ToString() 
                })]
            };


            var outbox = new OutboxMessage { Payload = JsonSerializer.Serialize(orderMail), Type = "orderMail" };

            return outbox;

        }

        public async Task<SummarizeOrderResponse> SummarizeOrderAsync(string username, string userId, int addressId, string email)
        {
            var response = new SummarizeOrderResponse();

            var cart = await _uow.CartRepository.GetCartAsync(userId);

            response.IsCart = cart is not null && cart.CartProducts.Any();
            if (!response.IsCart) return response;

            response.UserExists = await _uow.UsersRepository.UserExistsAsync(userId);

            if (!response.UserExists) return response;

            response.AddressExists = await _uow.AddressRepository.AddressExistsAsync(addressId, userId);

            if (!response.AddressExists) return response;

            StripeConfiguration.ApiKey = _secret_key;

            string successUrl = _successUrl;
            string cancelUrl = _errorUrl;

            var lineItems = new List<SessionLineItemOptions>();

            foreach(var product in cart!.CartProducts)
            

                lineItems.Add(
                    new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            Currency = "usd",
                            UnitAmount = (long) product.NewPrice * 100,
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = product.Title,
                                Description = $"The quantity for this product is {product.Quantity}",
                                Images = new() { product.Image }
                            },
                            
                        },
                        Quantity = 1
                    }
                );

            
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new() { "card" },
                LineItems = lineItems,
                Mode = "payment",
                SuccessUrl = successUrl,
                CancelUrl = cancelUrl,
                ClientReferenceId = userId,
                Metadata = new() { 
                    { "userId", userId },
                    { "addressId", addressId.ToString() }, 
                    { "email", email },
                    { "username", username }
                }
            };

            var service = new SessionService();
            var session = await service.CreateAsync(options);

            response.SessionUrl = session.Url;

            return response;
        }

        public async Task<ReviewOrderResponse> ReviewOrderAsync(CreateReviewDto dto, string username, string userId)
        {
            var response = new ReviewOrderResponse();

            var order = await _uow.OrderRepository.GetOrderInfoForReviewAsync(dto.OrderId, userId);
            response.OrderExists = order is not null;

            if (order is null) return response;

            using var transaction = await _uow.BeginTransactionAsync();

            try
            {
                await _uow.OrderRepository.SetOrderRatingAsync(dto.Rating, dto.OrderId);

                List<ReviewItem> reviewItems = new();

                foreach (var productId in order.ProductsIds)
                {
                    var reviewItem = new ReviewItem
                    {
                        Author = username,
                        ProductId = productId,
                        Comment = dto.Comment,
                        CreatedAt = DateTime.UtcNow,
                        Rating = dto.Rating
                    };

                    reviewItems.Add(reviewItem);
                }

                _uow.ReviewRepository.AddReviewsAsync(reviewItems);

                await _uow.ProductWriteRepository.SetProductsReviewAsync(order.ProductsIds, dto.Rating);

                await _uow.SaveAllChangesAsync();

                await _uow.CommitTransactionAsync(transaction);

                return response;
            } catch
            {
                await _uow.RollbackTransactionAsync(transaction);
                throw;
            }

        } 
    }
}
