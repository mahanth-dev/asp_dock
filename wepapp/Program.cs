using wepapp.Data;
using Microsoft.EntityFrameworkCore;

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

// 2. تنظیمات دیتابیس
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

Console.WriteLine($"--> Connection String: {connectionString}");

// *** اصلاح مهم: حذف AutoDetect و جایگزینی با نسخه ثابت ***
// این کار باعث می‌شود برنامه در لحظه استارت کرش نکند
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 0))));

var app = builder.Build();

// 3. اجرای مایگریشن با سیستم تلاش مجدد (Retry Mechanism)
ApplyMigrations(app);

// 4. میدل‌ورها
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();
app.UseRouting();
app.UseCors("AllowAll");
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllers();

app.Run();

// --- تابع کمکی برای تلاش مجدد اتصال به دیتابیس ---
void ApplyMigrations(WebApplication app)
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var context = services.GetRequiredService<AppDbContext>();
        
        // تلاش 10 بار برای اتصال (افزایش تعداد تلاش‌ها)
        for (int i = 0; i < 10; i++)
        {
            try
            {
                Console.WriteLine($"--> Attempting DB Connection (Try {i + 1}/10)...");
                // تست اتصال ساده قبل از مایگریشن
                if (context.Database.CanConnect())
                {
                    Console.WriteLine("--> Connection Established! Applying Migrations...");
                    context.Database.Migrate();
                    Console.WriteLine("--> Database Migration Applied Successfully!");
                    return;
                }
                else
                {
                    throw new Exception("CanConnect returned false");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> DB Not Ready Yet: {ex.Message}");
                Console.WriteLine("--> Waiting 3 seconds...");
                System.Threading.Thread.Sleep(3000); // 3 ثانیه صبر
            }
        }
        
        Console.WriteLine("--> FATAL ERROR: Could not connect to Database after 10 attempts.");
    }
}
