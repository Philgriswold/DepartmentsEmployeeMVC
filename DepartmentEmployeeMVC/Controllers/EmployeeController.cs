using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using DepartmentEmployeeMVC.Models;
using DepartmentsEmployeesMVC.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;

namespace DepartmentEmployeeMVC.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly IConfiguration _config;

        public EmployeeController(IConfiguration config)
        {
            _config = config;
        }

        public SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }
        // GET: Employee
        public ActionResult Index()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Id, FirstName, LastName, DepartmentId FROM Employee";

                    var reader = cmd.ExecuteReader();

                    var employees = new List<Employee>();

                    while (reader.Read())
                    {
                        employees.Add(new Employee
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId"))
                        });
                    }

                    reader.Close();
                    return View(employees);
                }
            }
        }

        // GET: Employee/Details/5
        public ActionResult Details(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT e.Id, e.FirstName, e.LastName, d.Id AS DepartmentId, d.DeptName
                                        FROM Employee e
                                        JOIN Department d ON e.DepartmentId = d.Id
                                        WHERE e.id = @id";
                    cmd.Parameters.Add(new SqlParameter("@Id", id));

                    var reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        var employee = new Employee
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            Department = new Department
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                                Name = reader.GetString(reader.GetOrdinal("DeptName"))
                            }
                        };

                        reader.Close();
                        return View(employee);
                    }

                    reader.Close();
                    return NotFound();
                }
            }
        }

        // GET: Employee/Create
        public ActionResult Create()
        {
            var departments = GetDepartments().Select(d => new SelectListItem
            {
                Text = d.Name,
                Value = d.Id.ToString()
            }).ToList();

            var viewModel = new EmployeeViewModel
            {
                Employee = new Employee(),
                Departments = departments
            };
            return View(viewModel);
        }

        // POST: Employee/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Employee employee)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"INSERT INTO Employee (FirstName, LastName, DepartmentId) 
                                            VALUES (@firstName, @lastName, @departmentId)";
                        cmd.Parameters.Add(new SqlParameter("@firstName", employee.FirstName));
                        cmd.Parameters.Add(new SqlParameter("@lastName", employee.LastName));
                        cmd.Parameters.Add(new SqlParameter("@departmentId", employee.DepartmentId));

                        cmd.ExecuteNonQuery();
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Employee/Edit/5
        public ActionResult Edit(int id)
        {
            var departments = GetDepartments().Select(d => new SelectListItem
            {
                Text = d.Name,
                Value = d.Id.ToString()
            }).ToList();
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Id, FirstName, LastName, DepartmentId
                                        FROM Employee
                                        WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    var reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        var employee = new Employee
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId"))
                        };

                        reader.Close();

                        var viewModel = new EmployeeViewModel
                        {
                            Employee = employee,
                            Departments = departments
                        };
                        return View(viewModel);
                    }
                    reader.Close();
                    return NotFound();
                }
            }
        }
        




        // POST: Employee/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Employee employee)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"UPDATE Employee 
                                            SET 
                                            FirstName = @firstName,
                                            LastName = @lastName,
                                            DepartmentId = @departmentId
                                            WHERE Id = @id";

                        cmd.Parameters.Add(new SqlParameter("@firstName", employee.FirstName));
                        cmd.Parameters.Add(new SqlParameter("@lastName", employee.LastName));
                        cmd.Parameters.Add(new SqlParameter("@departmentId", employee.DepartmentId));
                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        cmd.ExecuteNonQuery();
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return RedirectToAction(nameof(Edit), new { id = id });
            }
        }
             
        // GET: Employee/Delete/5
        public ActionResult Delete(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT FirstName, LastName, Id FROM Employee WHERE Id = @id";

                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    var reader = cmd.ExecuteReader();
                     
                    if (reader.Read())
                    {
                        var employee = new Employee
                        {
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            Id = reader.GetInt32(reader.GetOrdinal("Id"))
                        };
                        reader.Close();
                        return View(employee);
                    }

                    return NotFound();
                }
            }
        }

        // POST: Employee/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete([FromRoute] int id, Employee employee)
        {
            try
            { 
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"DELETE FROM Employee WHERE Id = @id";

                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        cmd.ExecuteNonQuery();

                        return RedirectToAction(nameof(Index));

                    }
                }

            }
            
            catch
            {
                return View();
            }
        }
        private List<Department> GetDepartments()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Id, DeptName FROM Department";

                    var reader = cmd.ExecuteReader();

                    var departments = new List<Department>();

                    while(reader.Read())
                    {
                        departments.Add(new Department
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("DeptName"))
                        }); 
                    }
                    reader.Close();

                    return departments;
                }
            }
        }

    }
}