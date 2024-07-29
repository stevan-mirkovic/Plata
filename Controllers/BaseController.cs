using Microsoft.AspNetCore.Mvc;
using Plata.Data;
using Plata.Models.Interfaces;
using Plata.Services;
using System.Globalization;

namespace Plata.Controllers
{
    public class BaseController(AuthManager authManager, LocalDbContext dbContext, ExchangeRateApi exchangeRate) : Controller
    {
        protected readonly int _authenticatedUserId = authManager.GetAuthenticatedUserId();

        protected T? GetEntityById<T>(int id) where T : class, IEntity
        {
            return dbContext.Set<T>().Find(id);
        }

        protected void AddEntity(IEntity entity)
        {
            dbContext.Add(entity);
            dbContext.SaveChanges();
        }

        protected void EditEntity(IEntity entity, IEntity editedEntity)
        {
            entity.Edit(editedEntity);
            dbContext.SaveChanges();
        }

        protected void DeleteEntity(IEntity entity)
        {
            dbContext.Remove(entity);
            dbContext.SaveChanges();
        }

        protected async Task SetDefaultViewBag()
        {
            ViewBag.AppStatus = TempData["AppStatus"];
            ViewBag.CurrentCurrency = Request.Cookies["CurrentCurrency"] ?? "RSD";
            ViewBag.ConversionRate = await exchangeRate.GetConversionRate(ViewBag.CurrentCurrency);
            ViewBag.CurrencySymbol = exchangeRate.GetCurrencySymbol(ViewBag.CurrentCurrency);
            ViewBag.CurrentUrl = Url.Action();
            ViewBag.CurrentCulture = CultureInfo.CurrentCulture.Name;
        }
    }
}
