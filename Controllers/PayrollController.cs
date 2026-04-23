using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechCorePayroll.Data;
using TechCorePayroll.Models;

namespace TechCorePayroll.Controllers;

public class PayrollController : Controller
{
    private readonly AppDbContext _context;

    public PayrollController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(int? employeeId)
    {
        if (employeeId == null)
        {
            var employees = await _context.Employees
                .OrderBy(e => e.LastName)
                .ThenBy(e => e.FirstName)
                .ToListAsync();

            return View("SelectEmployee", employees);
        }

        var employee = await _context.Employees.FirstOrDefaultAsync(e => e.EmployeeId == employeeId);
        if (employee == null)
        {
            return NotFound();
        }

        var payrolls = await _context.Payrolls
            .Include(p => p.Employee)
            .Where(p => p.EmployeeId == employeeId)
            .OrderByDescending(p => p.Date)
            .ToListAsync();

        var viewModel = new PayrollIndexViewModel
        {
            Employee = employee,
            Payrolls = payrolls
        };

        return View(viewModel);
    }

    public async Task<IActionResult> Create(int? employeeId)
    {
        if (employeeId == null)
        {
            return NotFound();
        }

        var employee = await _context.Employees.FirstOrDefaultAsync(e => e.EmployeeId == employeeId);
        if (employee == null)
        {
            return NotFound();
        }

        ViewBag.Employee = employee;

        var payroll = new Payroll
        {
            EmployeeId = employee.EmployeeId,
            Date = DateTime.Today
        };

        return View(payroll);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("EmployeeId,Date,DaysWorked,Deduction")] Payroll payroll)
    {
        var employee = await _context.Employees.FirstOrDefaultAsync(e => e.EmployeeId == payroll.EmployeeId);
        if (employee == null)
        {
            return NotFound();
        }

        ApplyPayrollCalculations(payroll, employee);

        if (!ModelState.IsValid)
        {
            ViewBag.Employee = employee;
            return View(payroll);
        }

        _context.Payrolls.Add(payroll);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index), new { employeeId = payroll.EmployeeId });
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var payroll = await _context.Payrolls
            .Include(p => p.Employee)
            .FirstOrDefaultAsync(p => p.PayrollId == id);

        if (payroll == null)
        {
            return NotFound();
        }

        ViewBag.Employee = payroll.Employee;
        return View(payroll);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("PayrollId,EmployeeId,Date,DaysWorked,Deduction")] Payroll payroll)
    {
        if (id != payroll.PayrollId)
        {
            return NotFound();
        }

        var existing = await _context.Payrolls
            .Include(p => p.Employee)
            .FirstOrDefaultAsync(p => p.PayrollId == id);

        if (existing == null)
        {
            return NotFound();
        }

        var employee = existing.Employee ?? await _context.Employees
            .FirstOrDefaultAsync(e => e.EmployeeId == existing.EmployeeId);

        if (employee == null)
        {
            return NotFound();
        }

        ApplyPayrollCalculations(payroll, employee);

        if (!ModelState.IsValid)
        {
            payroll.EmployeeId = existing.EmployeeId;
            ViewBag.Employee = employee;
            return View(payroll);
        }

        existing.Date = payroll.Date;
        existing.DaysWorked = payroll.DaysWorked;
        existing.Deduction = payroll.Deduction;
        existing.Employee = employee;
        ApplyPayrollCalculations(existing, employee);

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index), new { employeeId = existing.EmployeeId });
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var payroll = await _context.Payrolls
            .Include(p => p.Employee)
            .FirstOrDefaultAsync(m => m.PayrollId == id);

        if (payroll == null)
        {
            return NotFound();
        }

        return View(payroll);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var payroll = await _context.Payrolls
            .FirstOrDefaultAsync(p => p.PayrollId == id);

        if (payroll == null)
        {
            return RedirectToAction("Index", "Employee");
        }

        var employeeId = payroll.EmployeeId;
        _context.Payrolls.Remove(payroll);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index), new { employeeId });
    }

    private static void ApplyPayrollCalculations(Payroll payroll, Employee employee)
    {
        payroll.GrossPay = payroll.DaysWorked * employee.DailyRate;
        payroll.NetPay = payroll.GrossPay - payroll.Deduction;
    }
}
