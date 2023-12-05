using ChatHub.Configurations;
using ChatHub.Migrations;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllersWithViews();

builder.Services.UseWebConfigurations();

var app = builder.Build();

app.Services.CreateScope().ServiceProvider.GetService<IDatabaseMigration>()?.RunMigrations().GetAwaiter().GetResult();


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();