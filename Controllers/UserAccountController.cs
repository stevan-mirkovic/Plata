using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Plata.Data;
using Plata.Enums;
using Plata.Models.Entities;
using Plata.Models.ViewModels;
using Plata.Services;

namespace Plata.Controllers
{
    public partial class UserAccountController(LocalDbContext dbContext, AuthManager authManager, ExchangeRateApi exchangeRate) 
        : BaseController(authManager, dbContext, exchangeRate)
    {
        [HttpPost]
        public IActionResult LogIn(AuthenticationViewModel credentials)
        {
            var matchedUserAccount = GetUserAccountForLogIn(credentials.Identifier);

            if (matchedUserAccount == null)
            {
                TempData["AppStatus"] = AppStatus.WrongIdentifierFail;
                return RedirectToAction("Index", "Home");
            }

            if (!authManager.IsPasswordValid(matchedUserAccount, credentials.Password))
            {
                TempData["AppStatus"] = AppStatus.WrongPasswordFail;
                return RedirectToAction("Index", "Home");
            }

            authManager.SignInUser(matchedUserAccount);

            TempData["AppStatus"] = AppStatus.LogInSuccess;
            return RedirectToAction("Index", "Company");
        }

        [Authorize]
        public IActionResult LogOut()
        {
            authManager.LogOutUser();

            TempData["AppStatus"] = AppStatus.LogOutSuccess;
            return RedirectToAction("Index", "Home");
        }
        
        [HttpPost]
        public IActionResult Create(UserAccount newUserAccount)
        {
            if (!authManager.IsUsernameAvailable(newUserAccount.Username))
            {
                TempData["AppStatus"] = AppStatus.NotAvailableUsernameFail;
                return RedirectToAction("Index", "Home");
            }

            if (!authManager.IsEmailAvailable(newUserAccount.Company.EmailAddress))
            {
                TempData["AppStatus"] = AppStatus.NotAvailableEmailFail;
                return RedirectToAction("Index", "Home");
            }

            newUserAccount.Password = authManager.HashPassword(newUserAccount, newUserAccount.Password);

            if (newUserAccount.Company.Address != null)
            {
                if (newUserAccount.Company.Address.IsEmpty()) newUserAccount.Company.Address = null;

                else if (newUserAccount.Company.Address.IsIncomplete())
                {
                    TempData["AppStatus"] = AppStatus.IncompleteAddressFail;
                    return RedirectToAction("Index", "Home");
                }
            }

            newUserAccount.Company.PayPolicies.Add(new PayPolicy());
            AddEntity(newUserAccount);
            authManager.SignInUser(newUserAccount);

            TempData["AppStatus"] = AppStatus.SignUpSuccess;
            return RedirectToAction("Index", "Company");
        }

        [Authorize]
        public async Task<IActionResult> Edit()
        {
            var authenticatedUser = GetEntityById<UserAccount>(_authenticatedUserId);
            if (authenticatedUser == null) return Unauthorized();

            await SetDefaultViewBag();
            return View(new UserAccountEditViewModel(authenticatedUser));
        }

        [Authorize]
        [HttpPost]
        public IActionResult Edit(UserAccount editedUserAccount, AuthenticationViewModel credentials)
        {
            if (credentials.ConfirmationPassword == null)
            {
                TempData["AppStatus"] = AppStatus.NoConfirmationPasswordFail;
                return RedirectToAction();
            }

            var authenticatedUser = GetEntityById<UserAccount>(_authenticatedUserId);
            if (authenticatedUser == null) return Unauthorized();
            
            if (!authManager.IsPasswordValid(authenticatedUser, credentials.ConfirmationPassword))
            {
                TempData["AppStatus"] = AppStatus.WrongPasswordFail;
                return RedirectToAction();
            }

            if (credentials.NewPassword != null)
            {
                editedUserAccount.Password = authManager.HashPassword(authenticatedUser, credentials.NewPassword);
            }

            EditEntity(authenticatedUser, editedUserAccount);

            TempData["AppStatus"] = AppStatus.EditAccountSuccess;
            return RedirectToAction();
        }

        [Authorize]
        public IActionResult Delete(AuthenticationViewModel credentials)
        {
            if (credentials.ConfirmationPassword == null)
            {
                TempData["AppStatus"] = AppStatus.NoConfirmationPasswordFail;
                return RedirectToAction("Edit");
            }

            var authenticatedUser = GetUserAccountForDelete();
            if (authenticatedUser == null) return Unauthorized();

            if (!authManager.IsPasswordValid(authenticatedUser, credentials.ConfirmationPassword))
            {
                TempData["AppStatus"] = AppStatus.WrongPasswordFail;
                return RedirectToAction("Edit");
            }

            DeleteUserAccount(authenticatedUser);
            authManager.LogOutUser();

            TempData["AppStatus"] = AppStatus.DeleteAccountSuccess;
            return RedirectToAction("Index", "Home");
        }
    }
}