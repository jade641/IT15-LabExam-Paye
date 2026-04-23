using Microsoft.EntityFrameworkCore;
using TechCorePayroll.Models;

namespace TechCorePayroll.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Payroll> Payrolls => Set<Payroll>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Employee>()
            .HasMany(e => e.Payrolls)
            .WithOne(p => p.Employee)
            .HasForeignKey(p => p.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Employee>()
            .Property(e => e.DailyRate)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Payroll>()
            .Property(p => p.GrossPay)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Payroll>()
            .Property(p => p.Deduction)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Payroll>()
            .Property(p => p.NetPay)
            .HasPrecision(18, 2);
    }
}
