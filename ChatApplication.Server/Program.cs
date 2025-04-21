using Microsoft.EntityFrameworkCore;
using ChatApplication.Server.Data;
using ChatApplication.Server.Models;
using ChatApplication.Server.Hubs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;




var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSignalR().AddHubOptions<ChatHub>(options =>
{
    options.KeepAliveInterval = TimeSpan.FromMinutes(1); // Configure as per your need
    options.ClientTimeoutInterval = TimeSpan.FromMinutes(2); // Set a timeout interval
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        policy =>
        {
            policy.WithOrigins("http://localhost:3000") // Your React app's origin
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials(); // Required if using cookies or authorization
        });
});
// Add services to the container.
builder.Services.AddDbContext<ChatAppContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("ChatAppContext"));
});

//JWT Services
builder.Services.AddAuthentication(auth =>
{
    auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

builder.Services.AddAuthorization();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

var cookiePolicyOptions = new CookiePolicyOptions
{
    MinimumSameSitePolicy = SameSiteMode.Strict,
};
app.UseCookiePolicy(cookiePolicyOptions);



app.UseHttpsRedirection();
app.UseCors("AllowReactApp");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<ChatHub>("/chathub");

app.MapFallbackToFile("/index.html");

app.Run();
