namespace Plata.Models.OwnedEntities
{
    public class ContributionsPolicy
    {
        public ContributionsPolicy() { }

        public ContributionsPolicy(decimal pension, decimal health, decimal unemployment)
        {
            PensionContributionRate = pension;
            HealthInsuranceRate = health;
            UnemploymentInsuranceRate = unemployment;
        }

        public decimal PensionContributionRate { get; set; }
        public decimal HealthInsuranceRate { get; set; }
        public decimal UnemploymentInsuranceRate { get; set; }
    }
}
