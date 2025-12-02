using Microsoft.EntityFrameworkCore;
using wepapp.Data;

var builder = WebApplication.CreateBuilder(args);

// 1. سرویس‌ها
builder.Services.AddControllersWithViews();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        b => b.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});

// 2. تنظیمات دیتابیس (مخصوص SQLite)
// این خط به برنامه میگه از دیتابیس SQLite استفاده کنه
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// 3. اجرای مایگریشن (ساده شده برای SQLite)
// چون SQLite فایل است، نیاز به صبر کردن و تلاش مجدد ندارد
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        // این دستور اگر فایل دیتابیس نباشد، آن را می‌سازد
        context.Database.Migrate();
        Console.WriteLine("--> SQLite Database created/migrated successfully.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"--> Error applying migrations: {ex.Message}");
    }
}

// 4. میدل‌ورها
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// app.UseHttpsRedirection(); // در داکر/رندر معمولاً هندل می‌شود، اگر ارور SSL داد این را کامنت نگه دار
app.UseStaticFiles();

app.UseRouting();

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllers();

app.Run();
