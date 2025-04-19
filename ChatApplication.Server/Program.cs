using Microsoft.EntityFrameworkCore;
using ChatApplication.Server.Models;
using ChatApplication.Server.Hubs;
using ChatApplication.Server.Data;

var builder = WebApplication.CreateBuilder(args);

// --- Services ---

builder.Services.AddDbContext<ChatAppContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("ChatAppContext")));

// builder.Services.AddIdentityApiEndpoints<ApplicationUser>()
//     .AddEntityFrameworkStores<ChatAppContext>();

builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddSignalR(options =>
{
    options.KeepAliveInterval = TimeSpan.FromMinutes(1);
    options.ClientTimeoutInterval = TimeSpan.FromMinutes(2);
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:59996")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --- Build App ---
var app = builder.Build();

// --- Database Auto-Migrate ---
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ChatAppContext>();
    db.Database.Migrate();
}

// --- Middleware ---

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowReactApp");

app.UseAuthentication();
app.UseAuthorization();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapControllers();
// app.MapIdentityApi<ApplicationUser>();

// app.MapPost("/logout", async (SignInManager<ApplicationUser> signInManager) =>
// {
//     await signInManager.SignOutAsync();
//     return Results.Ok();
// }).RequireAuthorization();

app.MapHub<ChatHub>("/chathub");

app.MapFallbackToFile("/index.html");

app.Run();
