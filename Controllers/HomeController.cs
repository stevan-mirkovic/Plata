using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Plata.Data;
using Plata.Models.ViewModels;
using Plata.Services;
using System.Diagnostics;

namespace Plata.Controllers
{
    public class HomeController(AuthManager authManager, LocalDbContext dbContext, ExchangeRateApi exchangeRate) 
        : BaseController(authManager, dbContext, exchangeRate)
    {
        public async Task<IActionResult> Index()
        {
            if (authManager.IsUserAuthenticated()) return RedirectToAction("Index", "Company");
            if (string.IsNullOrEmpty(Request.Cookies["CurrentCurrency"])) Response.Cookies.Append("CurrentCurrency", "RSD");

            await SetDefaultViewBag();
            return View();
        }

        public IActionResult SetLanguage(string culture, string redirectUrl)
        {
            var cookieName = CookieRequestCultureProvider.DefaultCookieName;
            var cookieValue = CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture));
            Response.Cookies.Append(cookieName, cookieValue);

            return LocalRedirect(redirectUrl);
        }

        public IActionResult SetCurrency(string currency, string redirectUrl)
        {
            Response.Cookies.Append("CurrentCurrency", currency);
            return LocalRedirect(redirectUrl);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}