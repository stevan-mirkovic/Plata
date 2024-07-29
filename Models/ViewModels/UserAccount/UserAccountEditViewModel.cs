using Plata.Models.Entities;
using Plata.Models.Interfaces;

namespace Plata.Models.ViewModels
{
    public class UserAccountEditViewModel(UserAccount appUser) : IViewModel
    {
        public UserAccount EditedUserAccount { get; set; } = appUser;
        public AuthenticationViewModel? Credentials { get; set; }
    }
}