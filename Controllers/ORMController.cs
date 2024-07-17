using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SQL.Context;
using System.Diagnostics;

namespace SQL.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ORMController : ControllerBase
    {
        [HttpGet("Get All Employees")]
        public async Task<IActionResult> GetAllEmployees()
        {
            try
            {
                Stopwatch stopwatch = new();
                stopwatch.Start();
                using (var context = new AppDbContext())
                {
                    var employees = await context.employees.ToListAsync();
                    stopwatch.Stop();
                    return Ok(stopwatch.ElapsedMilliseconds);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("Get average salary from Namesake")]
        public IActionResult GetNamesakeAverageSalary()
        {
            try
            {
                Stopwatch stopwatch = new();
                stopwatch.Start();
                using (var context = new AppDbContext())
                {
                    var result = context.employees
                                .GroupBy(e => e.name)
                                .Select(g => new {
                                    Name = g.Key,
                                    TotalEmployees = g.Count(),
                                    AverageSalary = g.Average(e => e.salary)
                                })
                                .ToList();
                }
                stopwatch.Stop();
                return Ok(stopwatch.ElapsedMilliseconds);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("Get Employee By Id")]
        public async Task<IActionResult> GetEmployeeById(int id)
        {
            try
            {
                Stopwatch stopwatch = new();
                stopwatch.Start();
                using (var context = new AppDbContext())
                {
                    var employee = await context.employees.FirstOrDefaultAsync(e => e.id == id);
                }
                stopwatch.Stop();
                return Ok(stopwatch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
