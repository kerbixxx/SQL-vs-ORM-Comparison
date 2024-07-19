using Microsoft.AspNetCore.Mvc;
using Npgsql;
using SQL.Entities;
using Faker;
using System.Diagnostics;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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

        [HttpGet("Get Employees By Position")]
        public IActionResult GetEmployeesByPosition(int id)
        {
            try
            {
                Stopwatch stopwatch = new();
                stopwatch.Start();
                List<Employee> employees = new();
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();

                    string getEmployeesByPositionQuery = $@"SELECT * FROM employees WHERE employees.positionid = '{id}'";

                    using (var cmd = new NpgsqlCommand(getEmployeesByPositionQuery, conn))
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
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("Get Employee By Name & Surname")]
        public IActionResult GetEmployeeByName(string name)
        {
            try
            {
                Stopwatch stopwatch = new();
                stopwatch.Start();
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();

                    string getEmployeeByNameQuery = $@"SELECT * FROM employees WHERE employees.name = '{name}';";
                    Employee employee;

                    using (var cmd = new NpgsqlCommand(getEmployeeByNameQuery, conn))
                    {
                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {

                            while (reader.Read())
                            {
                                employee = new Employee
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

        [HttpGet("Full outer join")]
        public IActionResult FullOuterJoin()
        {
            try
            {
                Stopwatch stopwatch = new();
                stopwatch.Start();
                List<Employee> employees = new List<Employee>();
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();

                    string joinQuery = @"SELECT 
                                            E.id AS EmployeeID,
                                            E.name AS EmployeeName,
                                            P.id AS PositionID,
                                            P.name AS PositionName
                                        FROM 
                                            Employees E
                                        FULL OUTER JOIN 
                                            Positions P ON E.positionid = P.id;
";

                    using (var cmd = new NpgsqlCommand(joinQuery, conn))
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
                        new Position(){id = 1, name = "Младший разработчик"},
                        new Position(){id = 2, name = "Средний разработчик"},
                        new Position(){id = 3, name = "Старший разработчик"},
                        new Position(){id = 4, name = "Рекрутер"},
                        new Position(){id = 5, name = "Проект Менеджер"},
                        new Position(){id = 6, name = "Тестировщик"}
                    };

                    conn.Open();

                    foreach (var obj in objList)
                    {
                        string insertIntoPositionTableQuery = $@"INSERT INTO positions (Id,Name)
                                                                 VALUES ('{obj.id}','{obj.name}');";

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
