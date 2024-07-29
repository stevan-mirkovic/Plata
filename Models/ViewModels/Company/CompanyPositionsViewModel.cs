using Plata.Models.Interfaces;
using Plata.Models.Entities;


namespace Plata.Models.ViewModels
{
    public class CompanyPositionsViewModel(Company company) : IViewModel
    {
        public Company Company { get; set; } = company;
        public Position? NewPosition { get; set; }
    }
}