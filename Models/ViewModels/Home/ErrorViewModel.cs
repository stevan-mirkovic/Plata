using Plata.Models.Interfaces;

namespace Plata.Models.ViewModels
{
    public class ErrorViewModel : IViewModel
    {
        public string? RequestId { get; set; }
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}