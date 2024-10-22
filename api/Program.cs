using System.Text;
using api.Extensions;
using api.Middleware;
using api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;


var builder = WebApplication.CreateBuilder(args);


// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddApplicationService(builder.Configuration);
builder.Services.AddIdentityService(builder.Configuration);
builder.Services.AddRepositoryServices();


var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

// app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseCors(); // this line is added

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
