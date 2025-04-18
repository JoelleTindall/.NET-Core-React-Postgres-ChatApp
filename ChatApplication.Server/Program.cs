using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ChatApplication.Server.Data;
using ChatApplication.Server.Models;
//using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ChatApplication.Server.Hubs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;



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

//builder.Services.AddAuthorization();
//builder.Services.AddIdentityApiEndpoints<ApplicationUser>()
//.AddEntityFrameworkStores<ChatAppContext>();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
        options.LogoutPath = "/logout";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax; 
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Use Secure in production
        options.ExpireTimeSpan = TimeSpan.FromHours(1);
        options.SlidingExpiration = true;
    });

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();


var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();
//app.MapIdentityApi<ApplicationUser>();

//app.MapPost("/logout", async (SignInManager<ApplicationUser> signInManager) =>
//{

//await signInManager.SignOutAsync();
//return Results.Ok();

//}).RequireAuthorization();


// app.MapGet("/pingauth", (ClaimsPrincipal user) =>
// {
//     var email = user.FindFirstValue(ClaimTypes.Email);
//     var id = user.FindFirstValue(ClaimTypes.NameIdentifier);
//     return Results.Json(new { Email = email, Id = id });
// }).RequireAuthorization();
var cookiePolicyOptions = new CookiePolicyOptions
{
    MinimumSameSitePolicy = SameSiteMode.Strict,
};
app.UseCookiePolicy(cookiePolicyOptions);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowReactApp");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<ChatHub>("/chathub");

app.MapFallbackToFile("/index.html");

app.Run();
