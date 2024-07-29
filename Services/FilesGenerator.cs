using ClosedXML.Excel;
using CsvHelper.Configuration;
using CsvHelper;
using Plata.Models.Entities;
using System.Globalization;
using Plata.Utilities;
using iText.Layout;
using iText.Kernel.Pdf;
using iText.Layout.Element;
using iText.Kernel.Geom;
using iText.Kernel.Font;
using iText.Layout.Properties;


namespace Plata.Services
{
    public class FilesGenerator
    {
        public byte[] ExportEmployeesToXlsx(IEnumerable<Employee> employees)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.AddWorksheet("Employees");
            var currentCulture = CultureInfo.CurrentCulture.Name;
            var currentRow = 1;

            worksheet.Cell(currentRow, 1).Value = currentCulture == "en-US" ? "First name" : currentCulture == "sr-Latn-RS" ? "Ime" : currentCulture == "fr-FR" ? "Prénom" : "";
            worksheet.Cell(currentRow, 2).Value = currentCulture == "en-US" ? "Last name" : currentCulture == "sr-Latn-RS" ? "Prezime" : currentCulture == "fr-FR" ? "Nom" : "";
            worksheet.Cell(currentRow, 3).Value = currentCulture == "en-US" ? "Birthday" : currentCulture == "sr-Latn-RS" ? "Rodjendan" : currentCulture == "fr-FR" ? "Anniversaire" : "";
            worksheet.Cell(currentRow, 4).Value = currentCulture == "en-US" ? "Hire date" : currentCulture == "sr-Latn-RS" ? "Datum zaposlenja" : currentCulture == "fr-FR" ? "Date d'embauche" : "";
            worksheet.Cell(currentRow, 5).Value = currentCulture == "en-US" ? "Phone number" : currentCulture == "sr-Latn-RS" ? "Broj telefona" : currentCulture == "fr-FR" ? "Numéro de téléphone" : "";
            worksheet.Cell(currentRow, 6).Value = currentCulture == "en-US" ? "Email address" : currentCulture == "sr-Latn-RS" ? "Email adresa" : currentCulture == "fr-FR" ? "Adresse email" : "";
            worksheet.Cell(currentRow, 7).Value = currentCulture == "en-US" ? "Address" : currentCulture == "sr-Latn-RS" ? "Adresa" : currentCulture == "fr-FR" ? "Adresse" : ""; 
            worksheet.Cell(currentRow, 8).Value = currentCulture == "en-US" ? "Last net salary" : currentCulture == "sr-Latn-RS" ? "Zadnja neto plata" : currentCulture == "fr-FR" ? "Dernier salaire net" : "";

            foreach (var employee in employees)
            {
                currentRow++;
                worksheet.Cell(currentRow, 1).Value = employee.FirstName;
                worksheet.Cell(currentRow, 2).Value = employee.LastName;
                worksheet.Cell(currentRow, 3).Value = employee.Birthday.ToString();
                worksheet.Cell(currentRow, 4).Value = employee.HireDate.ToString();
                worksheet.Cell(currentRow, 5).Value = employee.PhoneNumber;
                worksheet.Cell(currentRow, 6).Value = employee.EmailAddress;
                worksheet.Cell(currentRow, 7).Value = employee.Address?.ToString();
                worksheet.Cell(currentRow, 8).Value = employee.GetLastPayslip()?.NetSalary;
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        
        public byte[] ExportEmployeesToCsv(IEnumerable<Employee> employees)
        {
            using var memoryStream = new MemoryStream();
            using var streamWriter = new StreamWriter(memoryStream);
            using var csvWriter = new CsvWriter(streamWriter, new CsvConfiguration(CultureInfo.InvariantCulture));

            csvWriter.Context.RegisterClassMap<EmployeeCsvMap>();
            csvWriter.WriteRecords(employees);
            streamWriter.Flush();
            return memoryStream.ToArray();
        }
        
        private class EmployeeCsvMap : ClassMap<Employee>
        {
            public EmployeeCsvMap()
            {
                var currentCulture = CultureInfo.CurrentCulture.Name;

                Map(employee => employee.FirstName)
                    .Name(currentCulture == "en-US" ? "First name" : currentCulture == "sr-Latn-RS" ? "Ime" : currentCulture == "fr-FR" ? "Prénom" : "");

                Map(employee => employee.LastName)
                    .Name(currentCulture == "en-US" ? "Last name" : currentCulture == "sr-Latn-RS" ? "Prezime" : currentCulture == "fr-FR" ? "Nom" : "");

                Map(employee => employee.Birthday)
                    .Name(currentCulture == "en-US" ? "Birthday" : currentCulture == "sr-Latn-RS" ? "Rodjendan" : currentCulture == "fr-FR" ? "Anniversaire" : "")
                    .ToString();

                Map(employee => employee.HireDate)
                    .Name(currentCulture == "en-US" ? "Hire date" : currentCulture == "sr-Latn-RS" ? "Datum zaposlenja" : currentCulture == "fr-FR" ? "Date d'embauche" : "")
                    .ToString();
                    
                Map(employee => employee.PhoneNumber)
                    .Name(currentCulture == "en-US" ? "Phone number" : currentCulture == "sr-Latn-RS" ? "Broj telefona" : currentCulture == "fr-FR" ? "Numéro de téléphone" : "");

                Map(employee => employee.EmailAddress)
                    .Name(currentCulture == "en-US" ? "Email address" : currentCulture == "sr-Latn-RS" ? "Email adresa" : currentCulture == "fr-FR" ? "Adresse email" : "");

                Map(employee => employee.Address)
                    .Name(currentCulture == "en-US" ? "Address" : currentCulture == "sr-Latn-RS" ? "Adresa" : currentCulture == "fr-FR" ? "Adresse" : "")
                    ?.ToString();

                Map(employee => employee.EmploymentContracts)
                    .Name(currentCulture == "en-US" ? "Last net salary" : currentCulture == "sr-Latn-RS" ? "Zadnja neto plata" : currentCulture == "fr-FR" ? "Dernier salaire net" : "")
                    .Convert(mapContext => mapContext.Value.GetLastPayslip() == null ? "" : Formatter.FormatDecimalForInput(mapContext.Value.GetLastPayslip().NetSalary));
            }
        }

        public byte[] ExportPayslipToPdf(Payslip payslip)
        {
            using var memoryStream = new MemoryStream();
            using var writer = new PdfWriter(memoryStream);
            using var pdf = new PdfDocument(writer);
            var document = new Document(pdf, PageSize.A4, false);
            document.SetMargins(30, 25, 25, 25);

            var currentCulture = CultureInfo.CurrentCulture.Name;

            document.Add(GetCompanyInfoPdfElement(payslip, currentCulture));
            document.Add(GetPayslipTitlePdfElement(payslip, currentCulture));
            document.Add(GetEmployeeInfoPdfElement(payslip, currentCulture));
            document.Add(new Paragraph("\n"));
            document.Add(GetBasesTitlePdfElement(currentCulture));
            document.Add(new Paragraph("_________________________________________________________________________________"));
            document.Add(GetSalaryBasesPdfElement(payslip, currentCulture));
            document.Add(new Paragraph("_________________________________________________________________________________"));
            document.Add(new Paragraph("\n"));
            document.Add(GetDetailsTitlePdfElement(currentCulture));
            document.Add(new Paragraph("_________________________________________________________________________________"));
            document.Add(GetGrossSalaryPdfElement(payslip, currentCulture));
            document.Add(new Paragraph("_ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _"));
            document.Add(GetCompanyCostPdfElement(payslip, currentCulture));
            document.Add(new Paragraph("_ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _"));
            document.Add(GetNetSalaryPdfElement(payslip, currentCulture));
            document.Add(new Paragraph("_________________________________________________________________________________"));
            document.Close();

            return memoryStream.ToArray();
        }

        private Paragraph GetCompanyInfoPdfElement(Payslip payslip, string culture)
        {
            var company = payslip.PayPolicy.Company;

            var enCompanyInfoContent = $"Name: {company.Name}\nSector: {company.Sector}\nFoundation date: {company.FoundationDate}\nAddress: {company.Address}";
            var srCompanyInfoContent = $"Naziv: {company.Name}\nSektor: {company.Sector}\nDatum osnivanja: {company.FoundationDate}\nAdresa: {company.Address}";
            var frCompanyInfoContent = $"Nom: {company.Name}\nSecteur: {company.Sector}\nDate de fondation: {company.FoundationDate}\nAdresse: {company.Address}";

            var companyInfoContent = culture switch
            {
                "en-US" => enCompanyInfoContent,
                "sr-Latn-RS" => srCompanyInfoContent,
                "fr-FR" => frCompanyInfoContent,
                _ => string.Empty
            };

            return new Paragraph(companyInfoContent).SetFontSize(10);
        }

        private Paragraph GetPayslipTitlePdfElement(Payslip payslip, string culture)
        {
            var titleContent = culture switch
            {
                "en-US" => $"PAYSLIP ({payslip.TimePeriod})",
                "sr-Latn-RS" => $"PLATNI LISTIC ({payslip.TimePeriod})",
                "fr-FR" => $"FICHE DE PAIE ({payslip.TimePeriod})",
                _ => string.Empty
            };

            return new Paragraph(titleContent)
                .SetFontSize(18)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetMarginTop(25)
                .SetMarginBottom(25);
        }

        private Paragraph GetEmployeeInfoPdfElement(Payslip payslip, string culture)
        {
            var employee = payslip.EmploymentContract.Employee;
            var position = payslip.EmploymentContract.Position;

            var enEmployeeInfoContent = $"First name: {employee.FirstName}\nLast name: {employee.LastName}\nAddress: {employee.Address}\nPosition: {position.Name}\nHire date: {employee.HireDate}";
            var srEmployeeInfoContent = $"Ime: {employee.FirstName}\nPrezime: {employee.LastName}\nAdresa: {employee.Address}\nPozicija: {position.Name}\nDatum zaposlenja: {employee.HireDate}";
            var frEmployeeInfoContent = $"Prénom: {employee.FirstName}\nNom: {employee.LastName}\nAdresse: {employee.Address}\nPoste: {position.Name}\nDate d'embauche: {employee.HireDate}";

            var employeeInfoContent = culture switch
            {
                "en-US" => enEmployeeInfoContent,
                "sr-Latn-RS" => srEmployeeInfoContent,
                "fr-FR" => frEmployeeInfoContent,
                _ => string.Empty,
            };

            return new Paragraph(employeeInfoContent)
                .SetFontSize(10)
                .SetMarginBottom(10);
        }

        private Paragraph GetBasesTitlePdfElement(string culture)
        {
            var basesTitleContent = culture switch
            {
                "en-US" => "Salary bases",
                "sr-Latn-RS" => "Osnovice za obracun",
                "fr-FR" => "Bases salariales",
                _ => string.Empty
            };

            return new Paragraph(basesTitleContent)
                .SetFontSize(14)
                .SetTextAlignment(TextAlignment.CENTER);
        }

        private Paragraph GetSalaryBasesPdfElement(Payslip payslip, string culture)
        {
            var position = payslip.EmploymentContract.Position;

            var enSalaryBasesContent = $"Base gross salary: {position.BaseGrossSalary} - Salary coefficient: {payslip.EmploymentContract.SalaryCoefficient} - Taxable bonuses: {payslip.TaxableBonuses} - Tax-free bonuses: {payslip.TaxFreeBonuses}";
            var srSalaryBasesContent = $"Osnovna bruto plata: {position.BaseGrossSalary} - Koeficijent plate: {payslip.EmploymentContract.SalaryCoefficient} - Oporezivi dodaci: {payslip.TaxableBonuses} - Neoporezivi dodaci: {payslip.TaxFreeBonuses}";
            var frSalaryBasesContent = $"Salaire brut de base: {position.BaseGrossSalary} - Coefficient de salaire: {payslip.EmploymentContract.SalaryCoefficient} - Primes imposables: {payslip.TaxableBonuses} - Primes non-imposables: {payslip.TaxFreeBonuses}";

            var salaryBasesContent = culture switch
            {
                "en-US" => enSalaryBasesContent,
                "sr-Latn-RS" => srSalaryBasesContent,
                "fr-FR" => frSalaryBasesContent,
                _ => string.Empty
            };

            return new Paragraph(salaryBasesContent)
                .SetFontSize(10)
                .SetMarginTop(10);
        }

        private Paragraph GetDetailsTitlePdfElement(string culture)
        {
            var detailsTitleContent = culture switch
            {
                "en-US" => "Salary details",
                "sr-Latn-RS" => "Detalji o plati",
                "fr-FR" => "Détails de salaire",
                _ => string.Empty
            };

            return new Paragraph(detailsTitleContent)
                .SetFontSize(14)
                .SetTextAlignment(TextAlignment.CENTER);
        }

        private Paragraph GetGrossSalaryPdfElement(Payslip payslip, string culture)
        {
            var position = payslip.EmploymentContract.Position;

            var enGrossSalaryContent = $"Gross salary: {position.BaseGrossSalary} * {payslip.EmploymentContract.SalaryCoefficient} + {payslip.TaxableBonuses} + {payslip.TaxFreeBonuses} = {payslip.GrossSalary}";
            var srGrossSalaryContent = $"Bruto plata: {position.BaseGrossSalary} * {payslip.EmploymentContract.SalaryCoefficient} + {payslip.TaxableBonuses} + {payslip.TaxFreeBonuses} = {payslip.GrossSalary}";
            var frGrossSalaryContent = $"Salaire brut: {position.BaseGrossSalary} * {payslip.EmploymentContract.SalaryCoefficient} + {payslip.TaxableBonuses} + {payslip.TaxFreeBonuses} = {payslip.GrossSalary}";

            var grossSalaryContent = culture switch
            {
                "en-US" => enGrossSalaryContent,
                "sr-Latn-RS" => srGrossSalaryContent,
                "fr-FR" => frGrossSalaryContent,
                _ => string.Empty
            };

            return new Paragraph(grossSalaryContent)
                .SetFontSize(10)
                .SetMarginTop(10);
        }

        private Paragraph GetCompanyCostPdfElement(Payslip payslip, string culture)
        {
            var contributionRates = payslip.PayPolicy.CompanyContributions;
            var pensionContribution = (payslip.GrossSalary - payslip.TaxFreeBonuses) * contributionRates.PensionContributionRate;
            var healthContribution = (payslip.GrossSalary - payslip.TaxFreeBonuses) * contributionRates.HealthInsuranceRate;
            var unemploymentContribution = (payslip.GrossSalary - payslip.TaxFreeBonuses) * contributionRates.UnemploymentInsuranceRate;
            var companyCost = payslip.GrossSalary + pensionContribution + healthContribution + unemploymentContribution;

            var enCompanyCostContent = $"Pension contribution: ({payslip.GrossSalary} - {payslip.TaxFreeBonuses}) * {contributionRates.PensionContributionRate} = {pensionContribution} \nHealth contribution: ({payslip.GrossSalary} - {payslip.TaxFreeBonuses}) * {contributionRates.HealthInsuranceRate} = {healthContribution} \nUnemployment contribution: ({payslip.GrossSalary} - {payslip.TaxFreeBonuses}) * {contributionRates.UnemploymentInsuranceRate} = {unemploymentContribution}\n\nCompany cost: {payslip.GrossSalary} + {pensionContribution} + {healthContribution} + {unemploymentContribution} = {companyCost}";

            var srCompanyCostContent = $"Doprinos za penzijsko: ({payslip.GrossSalary} - {payslip.TaxFreeBonuses}) * {contributionRates.PensionContributionRate} = {pensionContribution} \nDoprinos za zdravstveno: ({payslip.GrossSalary} - {payslip.TaxFreeBonuses}) * {contributionRates.HealthInsuranceRate} = {healthContribution} \nDoprinos za nezaposlenost: ({payslip.GrossSalary} - {payslip.TaxFreeBonuses}) * {contributionRates.UnemploymentInsuranceRate} = {unemploymentContribution}\n\nTroškovi poslodavca: {payslip.GrossSalary} + {pensionContribution} + {healthContribution} + {unemploymentContribution} = {companyCost}";

            var frCompanyCostContent = $"Cotisation retraite: ({payslip.GrossSalary} - {payslip.TaxFreeBonuses}) * {contributionRates.PensionContributionRate} = {pensionContribution} \nCotisation santé: ({payslip.GrossSalary} - {payslip.TaxFreeBonuses}) * {contributionRates.HealthInsuranceRate} = {healthContribution} \nCotisation chômage: ({payslip.GrossSalary} - {payslip.TaxFreeBonuses}) * {contributionRates.UnemploymentInsuranceRate} = {unemploymentContribution}\n\nCharges patronales: {payslip.GrossSalary} + {pensionContribution} + {healthContribution} + {unemploymentContribution} = {companyCost}";

            var companyCostContent = culture switch
            {
                "en-US" => enCompanyCostContent,
                "sr-Latn-RS" => srCompanyCostContent,
                "fr-FR" => frCompanyCostContent,
                _ => string.Empty
            };

            return new Paragraph(companyCostContent)
                .SetFontSize(10)
                .SetMarginTop(10);
        }

        private Paragraph GetNetSalaryPdfElement(Payslip payslip, string culture)
        {
            var payPolicy = payslip.PayPolicy;
            var contributionRates = payPolicy.EmployeeContributions;
            var tax = (payslip.GrossSalary - payPolicy.TaxFreeSalaryPortion - payslip.TaxFreeBonuses) * payPolicy.TaxRate;
            var pensionContribution = (payslip.GrossSalary - payslip.TaxFreeBonuses) * contributionRates.PensionContributionRate;
            var healthContribution = (payslip.GrossSalary - payslip.TaxFreeBonuses) * contributionRates.HealthInsuranceRate;
            var unemploymentContribution = (payslip.GrossSalary - payslip.TaxFreeBonuses) * contributionRates.UnemploymentInsuranceRate;
            var netSalary = payslip.GrossSalary - tax - pensionContribution - healthContribution - unemploymentContribution;

            var enNetSalaryContent = $"Tax: ({payslip.GrossSalary} - {payPolicy.TaxFreeSalaryPortion} - {payslip.TaxFreeBonuses}) * {payPolicy.TaxRate} = {tax} \nPension contribution: ({payslip.GrossSalary} - {payslip.TaxFreeBonuses}) * {contributionRates.PensionContributionRate} = {pensionContribution} \nHealth contribution: ({payslip.GrossSalary} - {payslip.TaxFreeBonuses}) * {contributionRates.HealthInsuranceRate} = {healthContribution} \nUnemployment contribution: ({payslip.GrossSalary} - {payslip.TaxFreeBonuses}) * {contributionRates.UnemploymentInsuranceRate} = {unemploymentContribution}\n\nNet salary: {payslip.GrossSalary} - {tax} - {pensionContribution} - {healthContribution} - {unemploymentContribution} = {netSalary}";

            var srNetSalaryContent = $"Porez: ({payslip.GrossSalary} - {payPolicy.TaxFreeSalaryPortion} - {payslip.TaxFreeBonuses}) * {payPolicy.TaxRate} = {tax} \nDoprinos za penzijsko: ({payslip.GrossSalary} - {payslip.TaxFreeBonuses}) * {contributionRates.PensionContributionRate} = {pensionContribution} \nDoprinos za zdravstveno: ({payslip.GrossSalary} - {payslip.TaxFreeBonuses}) * {contributionRates.HealthInsuranceRate} = {healthContribution} \nDoprinos za nezaposlenost: ({payslip.GrossSalary} - {payslip.TaxFreeBonuses}) * {contributionRates.UnemploymentInsuranceRate} = {unemploymentContribution}\n\nNeto plata: {payslip.GrossSalary} - {tax} - {pensionContribution} - {healthContribution} - {unemploymentContribution} = {netSalary}";

            var frNetSalaryContent = $"Impot: ({payslip.GrossSalary} - {payPolicy.TaxFreeSalaryPortion} - {payslip.TaxFreeBonuses}) * {payPolicy.TaxRate} = {tax} \nCotisation retraite: ({payslip.GrossSalary} - {payslip.TaxFreeBonuses}) * {contributionRates.PensionContributionRate} = {pensionContribution} \nCotisation santé: ({payslip.GrossSalary} - {payslip.TaxFreeBonuses}) * {contributionRates.HealthInsuranceRate} = {healthContribution} \nCotisation chômage: ({payslip.GrossSalary} - {payslip.TaxFreeBonuses}) * {contributionRates.UnemploymentInsuranceRate} = {unemploymentContribution}\n\nSalaire net: {payslip.GrossSalary} - {tax} - {pensionContribution} - {healthContribution} - {unemploymentContribution} = {netSalary}";

            var netSalaryContent = culture switch
            {
                "en-US" => enNetSalaryContent,
                "sr-Latn-RS" => srNetSalaryContent,
                "fr-FR" => frNetSalaryContent,
                _ => string.Empty
            };

            return new Paragraph(netSalaryContent)
                .SetFontSize(10)
                .SetMarginTop(10);
        }
    }
}
