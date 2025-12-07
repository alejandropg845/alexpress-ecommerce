using API;
using API.BackgroundServices;
using API.Middlewares;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

var config = builder.Configuration;

builder.Services.AddControllers().AddJsonOptions(x =>
                x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

builder.Services.SetIdentityConfiguration();
builder.Services.SetAuthentication(config);

builder.Services.SetCors();


builder.Services.SetDbContexts(config);

builder.Services.SetScopes();

builder.Services.AddHostedService<OutboxMessagesWorker>();

var app = builder.Build();


app.UseMiddleware<ExceptionHandler>();
app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("angularCORS");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();


