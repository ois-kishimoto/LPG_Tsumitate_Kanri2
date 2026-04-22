using Microsoft.EntityFrameworkCore;
using LPG_Tsumitate_Kanri2.Data;
using LPG_Tsumitate_Kanri2.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<ContributionCalculator>();
builder.Services.AddScoped<LedgerService>();

var app = builder.Build();

// 開発環境でシードデータを投入
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var calc = scope.ServiceProvider.GetRequiredService<ContributionCalculator>();
    await DataSeeder.SeedAsync(db, calc);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
