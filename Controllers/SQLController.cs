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

        [HttpGet("Custom Query")]
        public IActionResult CustomQuery(string query)
        {
            try
            {
                Stopwatch stopwatch = new();
                stopwatch.Start();
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();

                    string customQuery = $@"{query}";

                    using (var cmd = new NpgsqlCommand(customQuery, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
                stopwatch.Stop();
                return Ok(stopwatch.ElapsedMilliseconds);
            }catch(Exception ex)
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
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();

                    string getNamesakeAverageSalaryQuery = @"SELECT 
                                                                name,
                                                                COUNT(*) AS TotalEmployees,
                                                                AVG(salary) AS AverageSalary
                                                            FROM 
                                                                Employees
                                                            GROUP BY 
                                                                name;";

                    using (var cmd = new NpgsqlCommand(getNamesakeAverageSalaryQuery, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
                stopwatch.Stop();
                return Ok(stopwatch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("Get All Employees")]
        public IActionResult GetAllEmployees()
        {
            try
            {
                Stopwatch stopwatch = new();
                stopwatch.Start();
                List<Employee> employees = new List<Employee>();
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();

                    string getEmployeeTableQuery = @"
                            SELECT * FROM Employees";

                    using(var cmd = new NpgsqlCommand(getEmployeeTableQuery, conn))
                    {
                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {

                            while (reader.Read())
                            {
                                Employee employee = new Employee
                                {
                                    id = reader.GetInt32(reader.GetOrdinal("id")),
                                    name = reader.GetString(reader.GetOrdinal("name")),
                                    positionid = reader.GetInt32(reader.GetOrdinal("positionid")),
                                    salary = reader.GetDecimal(reader.GetOrdinal("salary")),
                                    email = reader.GetString(reader.GetOrdinal("email"))
                                };
                                employees.Add(employee);
                            }
                        }
                    }
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
        public IActionResult GetEmployeeById(int id)
        {
            try
            {
                Stopwatch stopwatch = new();
                stopwatch.Start();
                Employee employee;
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();

                    var getEmployeeByIdQuery = $@"SELECT * FROM employees WHERE employees.id = {id};";
                    using (var cmd = new NpgsqlCommand(getEmployeeByIdQuery, conn)) 
                    { 
                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                employee = new()
                                {
                                    id = reader.GetInt32(reader.GetOrdinal("id")),
                                    name = reader.GetString(reader.GetOrdinal("name")),
                                    positionid = reader.GetInt32(reader.GetOrdinal("positionid")),
                                    salary = reader.GetDecimal(reader.GetOrdinal("salary")),
                                    email = reader.GetString(reader.GetOrdinal("email"))
                                };
                            }
                        }
                    }
                }
                stopwatch.Stop();
                return Ok(stopwatch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
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
                        CREATE TABLE Positions (
                            Id serial PRIMARY KEY,
                            Name VARCHAR(255) NOT NULL
                        );";

                    using (var cmd = new NpgsqlCommand(createPositionTableQuery, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }

                    string createEmployeeTableQuery = @"
                            CREATE TABLE Employees (
                                Id serial PRIMARY KEY,
                                Name VARCHAR(255) NOT NULL,
                                PositionId INT,
                                Salary DECIMAL(10, 2),
                                Email VARCHAR(255),
                                CONSTRAINT fk_position
                                    FOREIGN KEY(PositionId)
                                        REFERENCES positions(Id)
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

                        string insertEmployeeIntoTableQuery = $@"INSERT INTO Employees (Name,PositionId,Salary,Email)
                                                               VALUES ('{obj.name}','{obj.positionid}','{obj.salary}','{obj.email}');";

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
                        string insertIntoPositionTableQuery = $@"INSERT INTO positions (Id,Name)
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
            obj.name = NameFaker.MaleName();
            obj.email = InternetFaker.Email();
            obj.positionid = random.Next(1, 7);
            obj.salary = random.Next(1, 6) * random.Next(20000, 40000);
            return obj;
        }
    }
}
