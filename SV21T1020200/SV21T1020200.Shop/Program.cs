using Microsoft.AspNetCore.Authentication.Cookies;

namespace SV21T1020200.Shop
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddHttpContextAccessor();

            builder.Services.AddControllersWithViews()
                            .AddMvcOptions(option =>
                            {
                                option.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
                            });

            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                            .AddCookie(option =>
                            {
                                option.Cookie.Name = "AuthenticationCookie";        // Tên Cookie
                                option.LoginPath = "/Account/Login";                //URL trang đăng nhập
                                option.AccessDeniedPath = "/Account/AccessDenined"; //URL đến trang cấm truy cập
                                option.ExpireTimeSpan = TimeSpan.FromDays(360);     //Thời gian "sống" của Cookie
                            });
            builder.Services.AddSession(option =>
            {
                option.IdleTimeout = TimeSpan.FromMinutes(60);
                option.Cookie.HttpOnly = true;
                option.Cookie.IsEssential = true;
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            app.UseSession();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            ApplicationContext.Configure
            (
                context: app.Services.GetRequiredService<IHttpContextAccessor>(),
                enviroment: app.Services.GetRequiredService<IWebHostEnvironment>()
            );
            //Khởi tạo cấu hình cho BusinessLayer
            string connectionString = builder.Configuration.GetConnectionString("SV21T1020200");
            SV21T1020200.BussinessLayers.Configuration.Initialize(connectionString);
            app.Run();
        }
    }
}
