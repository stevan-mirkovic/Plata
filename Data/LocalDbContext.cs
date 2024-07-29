using Microsoft.EntityFrameworkCore;
using Plata.Models.Entities;

namespace Plata.Data
{
    public class LocalDbContext(DbContextOptions<LocalDbContext> options) : DbContext(options)
    {
        public DbSet<UserAccount> UserAccounts { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<EmploymentContract> EmploymentContracts { get; set; }
        public DbSet<PayPolicy> PayPolicies { get; set; }
        public DbSet<Payslip> Payslips { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserAccount>(userAccount =>
            {
                userAccount.Property(u => u.Username).HasMaxLength(100);
                userAccount.Property(u => u.Password).HasMaxLength(100);
                userAccount.HasIndex(u => u.Username).IsUnique();
            });

            modelBuilder.Entity<Company>(company =>
            {
                company.Property(c => c.Name).HasMaxLength(100);
                company.Property(c => c.Sector).HasMaxLength(100);
                company.Property(c => c.Description).HasMaxLength(600);
                company.Property(c => c.EmailAddress).HasMaxLength(100);
                company.Property(c => c.PhoneNumber).HasMaxLength(15);
                company.HasIndex(c => c.EmailAddress).IsUnique();

                company.HasOne(c => c.Address)
                    .WithOne(a => a.Company)
                    .HasForeignKey<Address>(a => a.CompanyId);
            });

            modelBuilder.Entity<Employee>(employee =>
            {
                employee.Property(e => e.FirstName).HasMaxLength(50);
                employee.Property(e => e.LastName).HasMaxLength(50);
                employee.Property(e => e.EmailAddress).HasMaxLength(100);
                employee.Property(e => e.PhoneNumber).HasMaxLength(15);
                employee.HasIndex(e => e.EmailAddress).IsUnique();

                employee.HasOne(e => e.Address)
                    .WithOne(a => a.Employee)
                    .HasForeignKey<Address>(a => a.EmployeeId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Address>(address =>
            {
                address.Property(a => a.Street).HasMaxLength(100);
                address.Property(a => a.Number).HasMaxLength(20);
                address.Property(a => a.City).HasMaxLength(100);
                address.Property(a => a.PostalCode).HasMaxLength(20);
                address.Property(a => a.Country).HasMaxLength(100);

                address.ToTable(t => t.HasCheckConstraint("CK_Address_Company_Or_Employee", "([CompanyId] IS NOT NULL AND [EmployeeId] IS NULL) OR ([CompanyId] IS NULL AND [EmployeeId] IS NOT NULL)"));
            });

            modelBuilder.Entity<Position>(position =>
            {
                position.Property(p => p.Name).HasMaxLength(100);

                position.HasMany(p => p.EmploymentContracts)
                    .WithOne(ec => ec.Position)
                    .HasForeignKey(ec => ec.PositionId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<PayPolicy>(payPolicy =>
            {
                payPolicy.OwnsOne(pp => pp.EmployeeContributions);
                payPolicy.OwnsOne(pp => pp.CompanyContributions);

                payPolicy.HasMany(pp => pp.Payslips)
                    .WithOne(ps => ps.PayPolicy)
                    .HasForeignKey(ps => ps.PayPolicyId)
                    .OnDelete(DeleteBehavior.Restrict);

                payPolicy.HasIndex(pp => pp.CompanyId)
                    .HasFilter("[IsActive] = 1")
                    .IsUnique();
            });
            
            modelBuilder.Entity<EmploymentContract>(employmentContract =>
            {
                employmentContract.HasMany(ec => ec.Payslips)
                    .WithOne(ps => ps.EmploymentContract)
                    .HasForeignKey(ps => ps.ContractId)
                    .OnDelete(DeleteBehavior.Restrict);

                employmentContract.HasIndex(ec => ec.EmployeeId)
                    .HasFilter("[IsActive] = 1")
                    .IsUnique();
            });
                
        }
    }
}