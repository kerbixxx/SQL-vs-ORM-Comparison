using Microsoft.AspNetCore.Mvc;
using Npgsql;
using SQL.Entities;
using Faker;
using System.Diagnostics;

namespace SQL.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SQLController : ControllerBase
    {
        private readonly string connectionString = "Host=localhost;Port=5432;Username=postgres;Password=5051;Database=sqltesting";
        public SQLController() { }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok();
        }

        [HttpPost("Create Tables")]
        public IActionResult CreateTable()
        {
            try
            {
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();

                    string createPositionTableQuery = @"
                        CREATE TABLE positions (
                            position_id serial PRIMARY KEY,
                            name VARCHAR(255) NOT NULL
                        );";

                    using (var cmd = new NpgsqlCommand(createPositionTableQuery, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }

                    string createEmployeeTableQuery = @"
                            CREATE TABLE employees (
                                employee_id serial PRIMARY KEY,
                                name VARCHAR(255) NOT NULL,
                                position_id INT,
                                salary DECIMAL(10, 2),
                                email VARCHAR(255),
                                CONSTRAINT fk_position
                                    FOREIGN KEY(position_id)
                                        REFERENCES positions(position_id)
                            );";

                    using (var cmd = new NpgsqlCommand(createEmployeeTableQuery, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
                return Ok();
            } catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("Fill Employees with data")]
        public IActionResult FillTablesWithData(int? value)
        {
            try
            {
                Stopwatch stopwatch = new();
                stopwatch.Start();
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();

                    for(int i = 0; i < value; i++)
                    {
                        var obj = GenerateEmployee();

                        string insertEmployeeIntoTableQuery = $@"INSERT INTO employees (name,position_id,salary,email)
                                                               VALUES ('{obj.Name}','{obj.PositionId}','{obj.Salary}','{obj.Email}');";

                        using (var cmd = new NpgsqlCommand(insertEmployeeIntoTableQuery, conn))
                        {
                            cmd.ExecuteNonQuery();
                        }
                    }
                    stopwatch.Stop();
                    return Ok(stopwatch.ElapsedMilliseconds);
                }
            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("Fill Positions Table")]
        public IActionResult FillPositionsTable()
        {
            try
            {
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    List<Position> objList = new()
                    {
                        new Position(){Id = 1, Name = "Младший разработчик"},
                        new Position(){Id = 2, Name = "Средний разработчик"},
                        new Position(){Id = 3, Name = "Старший разработчик"},
                        new Position(){Id = 4, Name = "Рекрутер"},
                        new Position(){Id = 5, Name = "Проект Менеджер"},
                        new Position(){Id = 6, Name = "Тестировщик"}
                    };

                    conn.Open();

                    foreach (var obj in objList)
                    {
                        string insertIntoPositionTableQuery = $@"INSERT INTO positions (position_id,name)
                                                                 VALUES ('{obj.Id}','{obj.Name}');";

                        using (var cmd = new NpgsqlCommand(insertIntoPositionTableQuery, conn))
                        {
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [HttpDelete]
        public IActionResult DeleteTables()
        {
            try
            {
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();

                    string DeleteTablesQuery = "DROP SCHEMA public CASCADE;" +
                        "CREATE SCHEMA public;";

                    using (var cmd = new NpgsqlCommand(DeleteTablesQuery, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }

                    return Ok();
                }
            } catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [NonAction]
        public Employee GenerateEmployee()
        {
            Random random = new Random();
            Employee obj = new Employee();
            obj.Name = NameFaker.MaleName();
            obj.Email = InternetFaker.Email();
            obj.PositionId = random.Next(1, 7);
            obj.Salary = random.Next(1, 6) * random.Next(20000, 40000);
            return obj;
        }
    }
}
