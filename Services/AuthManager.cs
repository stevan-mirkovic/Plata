using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Plata.Data;
using Plata.Models.Entities;
using System.Security.Claims;

namespace Plata.Services
{
    public class AuthManager(IHttpContextAccessor httpAccessor, LocalDbContext dbContext)
    {
        private readonly HttpContext _httpContext = httpAccessor.HttpContext ?? throw new InvalidOperationException();

        public void SignInUser(UserAccount userAccount)
        {
            var idClaim = new Claim("Id", userAccount.Id.ToString());
            var claims = new List<Claim>() { idClaim };
            var identity = new ClaimsIdentity(claims, "UserAuthenticationScheme");
            var principal = new ClaimsPrincipal(identity);

            _httpContext.SignInAsync(principal);
        }

        public void LogOutUser() => _httpContext.SignOutAsync();

        public int GetAuthenticatedUserId()
        {
            var principal = _httpContext.AuthenticateAsync().Result.Principal;
            var idClaimValue = principal?.FindFirstValue("Id");
            return Convert.ToInt32(idClaimValue);
        }

        public string HashPassword(UserAccount userAccount, string password)
        {
            var hasher = new PasswordHasher<UserAccount>();
            return hasher.HashPassword(userAccount, password);
        }

        public bool IsUserAuthenticated() => _httpContext.User.Identity.IsAuthenticated;

        public bool IsUsernameAvailable(string username) => !dbContext.UserAccounts.Any(u => u.Username == username);

        public bool IsEmailAvailable(string emailAddress) => !dbContext.UserAccounts.Any(u => u.Company.EmailAddress == emailAddress);

        public bool IsPasswordValid(UserAccount userAccount, string password)
        {
            var hasher = new PasswordHasher<UserAccount>();
            var passwordVerification = hasher.VerifyHashedPassword(userAccount, userAccount.Password, password);
            return passwordVerification == PasswordVerificationResult.Success;
        }
    }
}