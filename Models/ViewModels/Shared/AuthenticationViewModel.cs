using Plata.Models.Interfaces;

namespace Plata.Models.ViewModels
{
    public class AuthenticationViewModel : IViewModel
    {
        public string? Identifier { get; set; }
        public string? Password { get; set; }
        public string? ConfirmationPassword { get; set; }
        public string? NewPassword { get; set; }
    }
}
