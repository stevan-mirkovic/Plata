using Plata.Models.Entities;
using Plata.Models.Interfaces;

namespace Plata.Models.ViewModels
{
    public class HomeIndexViewModel : IViewModel
    {
        public AuthenticationViewModel? Credentials { get; set; }
        public UserAccount? NewUserAccount { get; set; }
    }
}