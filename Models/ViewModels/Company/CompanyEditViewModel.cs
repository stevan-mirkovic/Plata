using Plata.Models.Interfaces;
using Plata.Models.Entities;

namespace Plata.Models.ViewModels
{
    public class CompanyEditViewModel(Company company) : IViewModel
    {
        public Company EditedCompany { get; set; } = company;
        public PayPolicy? EditedCompanyPayPolicy { get; set; } = company.GetActivePayPolicy();
        public AuthenticationViewModel? Credentials { get; set; }
    }
}
