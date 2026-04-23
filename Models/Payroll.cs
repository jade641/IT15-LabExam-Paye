using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TechCorePayroll.Models;

[Table("Payrolls")]
public class Payroll
{
    public int PayrollId { get; set; }

    [Required]
    public int EmployeeId { get; set; }

    public Employee? Employee { get; set; }

    [DataType(DataType.Date)]
    public DateTime Date { get; set; } = DateTime.Today;

    [Range(0, int.MaxValue, ErrorMessage = "Days worked must not be negative.")]
    public int DaysWorked { get; set; }

    [DataType(DataType.Currency)]
    public decimal GrossPay { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Deduction must not be negative.")]
    [DataType(DataType.Currency)]
    public decimal Deduction { get; set; }

    [DataType(DataType.Currency)]
    public decimal NetPay { get; set; }
}
