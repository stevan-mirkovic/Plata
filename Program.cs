using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Plata.Data;
using Plata.Services;
using Plata.Services.Settings;

namespace Plata
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

            builder.Services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[] { "sr-Latn-RS", "en-US", "fr-FR" };
                options.SetDefaultCulture(supportedCultures[0])
                    .AddSupportedCultures(supportedCultures)
                    .AddSupportedUICultures(supportedCultures);
            });

            builder.Services.AddControllersWithViews()
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
                .AddDataAnnotationsLocalization();

            builder.Services.AddDbContext<LocalDbContext>(options => options
            .UseSqlServer(builder.Configuration.GetConnectionString("LocalDbConnectionString")));

            builder.Services.Configure<ExchangeRateApiSettings>(builder.Configuration.GetSection("ExchangeRateApi"));
            builder.Services.AddHttpClient<ExchangeRateApi>();

            builder.Services.AddAuthentication("UserAuthenticationScheme")
                .AddCookie("UserAuthenticationScheme", options =>
                {
                    options.Cookie.Name = "UserCookie";
                    options.LoginPath = "/";
                    options.LogoutPath = "/Company";
                });

            builder.Services.AddHttpContextAccessor();

            builder.Services.AddTransient<AuthManager>();
            builder.Services.AddSingleton<FilesGenerator>();

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseRequestLocalization();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();
            app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
            app.Run();
        }
    }
}