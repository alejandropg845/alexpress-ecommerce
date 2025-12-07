using API.DbContexts;
using API.Entities;
using API.Factories.Cart;
using API.Interfaces.Factories;
using API.Interfaces.Repositories;
using API.Interfaces.Repositories.Products;
using API.Interfaces.Services;
using API.Interfaces.Services.User;
using API.Payloads.Auth;
using API.Repositories;
using API.Services.App;
using API.Services.Secondary;
using API.UnitsOfWork.Classes;
using API.UnitsOfWork.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Data.Common;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace API
{
    public static class Extensions
    {
        public static IServiceCollection SetDbContexts(this IServiceCollection services, IConfiguration config)
        {
            var connectionString = config.GetConnectionString("defaultConnection");

            services.AddScoped<DbConnection>(provider =>
            {
                var connection = new SqlConnection(connectionString);
                connection.Open();
                return connection;
            });

            Action<IServiceProvider, DbContextOptionsBuilder> configureDbContext = (provider, options) =>
            {
                var connection = provider.GetRequiredService<DbConnection>();
                options.UseSqlServer(connection);
            };

            services.AddDbContext<AuthDbContext>(configureDbContext);

            services.AddDbContext<AddressDbContext>(configureDbContext);

            services.AddDbContext<CartDbContext>(configureDbContext);

            services.AddDbContext<OrderDbContext>(configureDbContext);

            services.AddDbContext<ProductDbContext>(configureDbContext);

            services.AddDbContext<ReviewDbContext>(configureDbContext);

            services.AddDbContext<WishListDbContext>(configureDbContext);

            services.AddDbContext<OutboxMessageDbContext>(configureDbContext);

            services.AddDbContext<RTokenDbContext>(configureDbContext);

            return services;
        }
        public static IServiceCollection SetIdentityConfiguration(this IServiceCollection services)
        {
            services.AddIdentity<AppUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 5;
                options.Password.RequireNonAlphanumeric = false;
            }).AddEntityFrameworkStores<AuthDbContext>()
            .AddDefaultTokenProviders();

            return services;
        }
        public static IServiceCollection SetCors(this IServiceCollection services)
        {
            services.AddCors(serviceProvider =>
            {
                serviceProvider.AddPolicy("angularCORS", policy =>
                {
                    policy.SetIsOriginAllowed(origin =>
                    {
                        return origin.Contains("localhost");
                    }).AllowAnyHeader().AllowAnyMethod().AllowCredentials();
                });
            });

            return services;
        }

        public static IServiceCollection SetScopes(this IServiceCollection services)
        {
            /* Servicios */
            services.AddScoped<IAddressService, AddressService>();
            services.AddScoped<IAdminService, AdminService>();
            services.AddScoped<ICartService, CartService>();
            services.AddScoped<IMailService, MailService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IUserWriteService, UserService>();
            services.AddScoped<IUserReadService, UserService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IWishlistService, WishlistService>();
            services.AddScoped<IRTokenService, RTokenService>();

            services.AddScoped<IOrderUnitOfWork, OrderUnitOfWork>();
            services.AddScoped<IAuthUnitOfWork, AuthUnitOfWork>();

            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IOutboxMessageService, OutboxMessageService>();
            services.AddSingleton<ICouponStrategyFactory, CouponStrategyFactory>();

            /* Repositories */

            services.AddScoped<IAddressRepository, AddressRepository>();
            services.AddScoped<IAdminRepository, AdminRepository>();
            services.AddScoped<ICartRepository, CartRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IProductWriteRepository, ProductRepository>();
            services.AddScoped<IProductReadRepository, ProductRepository>();
            services.AddScoped<IReviewRepository, ReviewRepository>();
            services.AddScoped<IUsersRepository, UserRepository>();
            services.AddScoped<IProductWriteRepository, ProductRepository>();
            services.AddScoped<IWishListRepository, WishListRepository>();
            services.AddScoped<IOutboxMessageRepository, OutBoxMessageRepository>();
            services.AddScoped<IRTokenRepository, RTokenRepository>();

            return services;
        }

        public static IServiceCollection SetAuthentication(this IServiceCollection services, IConfiguration config)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = config["JWT:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = config["JWT:Audience"],
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JWT:SigningKey"]!)),
                };
            });

            return services;
        }

        public static OutboxMessage CreateTokenOutboxMessage(string token, string email, string type)
        {

            var emailToken = new EmailToken
            {
                Email = email,
                Token = token
            };

            string payload = JsonSerializer.Serialize(emailToken);

            var outboxMessage = new OutboxMessage
            {
                Payload = payload,
                Type = type
            };

            return outboxMessage;

        }

        public static RToken CreateRefreshToken(string appUserId)
        {

            byte[] randomCharacters = new byte[32];
            using var rng = RandomNumberGenerator.Create();

            rng.GetBytes(randomCharacters);

            string rToken = Convert.ToBase64String(randomCharacters);


            var newRToken = new RToken
            {
                ExpirationTime = DateTimeOffset.UtcNow.AddMonths(1),
                RefreshToken = rToken,
                AppUserId = appUserId
            };

            return newRToken;

        }

    }
}
