using Microsoft.EntityFrameworkCore;
using OnlineVotingSystem.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();


// ===============================
// RUN DATABASE MIGRATIONS ON STARTUP
// ===============================
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();  // creates DB + tables if missing
    // OPTIONAL: for migration support use db.Database.Migrate();
}
// ===============================


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Polls/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Polls}/{action=Index}/{id?}");

app.Run();
