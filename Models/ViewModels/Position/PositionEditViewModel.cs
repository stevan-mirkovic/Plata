using Plata.Models.Interfaces;
using Plata.Models.Entities;


namespace Plata.Models.ViewModels
{
    public class PositionEditViewModel(Position position) : IViewModel
    {
        public Position EditedPosition { get; set; } = position;
        public AuthenticationViewModel? Credentials { get; set; }
    }
}