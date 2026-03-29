using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ExpenseTracker.Models;
using ExpenseTracker.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Linq;

namespace ExpenseTracker.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(int? month, int? year)
    {
        // Get expenses for the selected month/year or default to current
        var selectedMonth = month ?? DateTime.Now.Month;
        var selectedYear = year ?? DateTime.Now.Year;

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        // Base query for the selected year and month, scoped to the user
        var expensesQuery = _context.Expenses
            .Include(e => e.Category)
            .Where(e => e.Date.Year == selectedYear && e.UserId == userId);
        
        // For trend chart, we need all expenses for the year
        var yearlyExpenses = await expensesQuery.ToListAsync();

        // For category chart and summaries, we need expenses for the selected month
        var monthlyExpenses = yearlyExpenses
            .Where(e => e.Date.Month == selectedMonth)
            .ToList();

        // 1. Doughnut Chart & Analytics (Categories)
        var categoryTotals = monthlyExpenses
            .GroupBy(e => e.Category)
            .Select(g => new 
            { 
                CategoryId = g.Key?.Id ?? 0,
                CategoryName = g.Key?.Name ?? "Uncategorized", 
                CategoryColor = g.Key?.Color ?? "#ccc",
                Budget = g.Key?.MonthlyBudget,
                Total = g.Sum(e => e.Amount),
                DisplayOrder = g.Key?.DisplayOrder ?? 999,
                GroupName = g.Key?.Group ?? "Uncategorized"
            })
            .OrderBy(c => c.DisplayOrder)
            .ThenByDescending(c => c.Total)
            .ToList();

        // Budget Alerts
        ViewBag.BudgetAlerts = categoryTotals
            .Where(c => c.Budget.HasValue && c.Total > c.Budget.Value)
            .Select(c => new { 
                CategoryName = c.CategoryName, 
                TotalFormatted = c.Total.ToString("C"), 
                BudgetFormatted = c.Budget.Value.ToString("C") 
            })
            .ToList();

        // Group Analytics
        ViewBag.GroupAnalytics = categoryTotals
            .GroupBy(c => c.GroupName)
            .Select(g => new
            {
                GroupName = g.Key,
                Total = g.Sum(c => c.Total)
            })
            .OrderByDescending(g => g.Total)
            .ToList();

        ViewBag.Labels = categoryTotals.Select(c => c.CategoryName).ToArray();
        ViewBag.Data = categoryTotals.Select(c => c.Total).ToArray();
        ViewBag.Colors = categoryTotals.Select(c => c.CategoryColor).ToArray();
        ViewBag.CategoryAnalytics = categoryTotals;

        // 2. Summary Cards
        ViewBag.TotalExpenses = monthlyExpenses.Sum(e => e.Amount);
        ViewBag.TransactionsCount = monthlyExpenses.Count;
        ViewBag.TopCategory = categoryTotals.OrderByDescending(c => c.Total).FirstOrDefault()?.CategoryName ?? "N/A";

        // 3. Trend Chart (Monthly totals for the selected year - MULTILINE BY CATEGORY)
        
        var allCategoriesInYear = yearlyExpenses
            .Select(e => e.Category)
            .Where(c => c != null)
            .DistinctBy(c => c.Id)
            .OrderBy(c => c.DisplayOrder)
            .ToList();

        var trendDatasets = new List<object>();

        // We also want a "Total" line potentially, or just category lines. Let's do category lines.
        foreach (var category in allCategoriesInYear)
        {
            var monthlyData = new decimal[12];
            for (int i = 1; i <= 12; i++)
            {
                monthlyData[i - 1] = yearlyExpenses
                    .Where(e => e.Date.Month == i && e.CategoryId == category.Id)
                    .Sum(e => e.Amount);
            }
            
            trendDatasets.Add(new
            {
                label = category.Name,
                data = monthlyData,
                borderColor = category.Color ?? "#ccc",
                backgroundColor = category.Color ?? "#ccc",
                fill = false,
                tension = 0.3
            });
        }

        // Add an Uncategorized line if needed
        var uncategorizedExpenses = yearlyExpenses.Where(e => e.CategoryId == 0 || e.Category == null).ToList();
        if (uncategorizedExpenses.Any())
        {
            var monthlyData = new decimal[12];
            for (int i = 1; i <= 12; i++)
            {
                monthlyData[i - 1] = uncategorizedExpenses
                    .Where(e => e.Date.Month == i)
                    .Sum(e => e.Amount);
            }
            trendDatasets.Add(new
            {
                label = "Uncategorized",
                data = monthlyData,
                borderColor = "#ccc",
                backgroundColor = "#ccc",
                fill = false,
                tension = 0.3
            });
        }
        
        // Add Total Spending line as well
        var totalMonthlyData = new decimal[12];
        for (int i = 1; i <= 12; i++)
        {
            totalMonthlyData[i - 1] = yearlyExpenses
                .Where(e => e.Date.Month == i)
                .Sum(e => e.Amount);
        }
        trendDatasets.Add(new
        {
            label = "Total Spending",
            data = totalMonthlyData,
            borderColor = "#212529", // Dark color for total
            backgroundColor = "#212529",
            borderDash = new int[] { 5, 5 }, // Dashed line to distinguish
            fill = false,
            tension = 0.3,
            borderWidth = 3
        });

        ViewBag.TrendDatasets = trendDatasets;

        // 4. Pass selected filters back to the view
        ViewBag.SelectedMonth = selectedMonth;
        ViewBag.SelectedYear = selectedYear;

        // Ensure we pass a list of years to the view for the dropdown
        var availableYears = await _context.Expenses
            .Select(e => e.Date.Year)
            .Distinct()
            .OrderByDescending(y => y)
            .ToListAsync();
        
        if (!availableYears.Contains(DateTime.Now.Year))
        {
            availableYears.Insert(0, DateTime.Now.Year);
        }
        ViewBag.AvailableYears = availableYears;

        return View();
    }

    [AllowAnonymous]
    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
