using System.Text.Json.Serialization;
using api.Extensions;
using api.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options => { options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()); });

builder.Services.AddApplicationService(builder.Configuration);
builder.Services.AddIdentityService(builder.Configuration);
builder.Services.AddRepositoryServices();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("admin", policy =>
        policy.RequireRole("admin"));
});

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

// app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseCors(); // this line is added

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();