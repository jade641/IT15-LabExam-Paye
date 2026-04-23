namespace TechCorePayroll.Models;

public class PayrollIndexViewModel
{
    public Employee Employee { get; set; } = new Employee();
    public List<Payroll> Payrolls { get; set; } = new();
}
