using Microsoft.EntityFrameworkCore;
using ChatApplication.Server.Data;
using ChatApplication.Server.Models;
using ChatApplication.Server.Hubs;




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
