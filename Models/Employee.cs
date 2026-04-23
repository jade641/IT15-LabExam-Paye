using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TechCorePayroll.Models;

[Table("Employees")]
public class Employee
{
    public int EmployeeId { get; set; }

    [Required]
    [StringLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Position { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Department { get; set; } = string.Empty;

    [Range(0, 1000000)]
    [DataType(DataType.Currency)]
    public decimal DailyRate { get; set; }

    public ICollection<Payroll> Payrolls { get; set; } = new List<Payroll>();

    [NotMapped]
    public string FullName => $"{FirstName} {LastName}";
}
