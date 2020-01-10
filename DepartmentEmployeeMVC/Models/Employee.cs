using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace DepartmentEmployeeMVC.Models
{
    public class Employee
    {
        public int Id { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Display(Name = "LastName")]
        public string LastName { get; set; }
        [Display(Name = "DepartmentId")]
        public int DepartmentId { get; set; }
        public Department Department { get; set; }
    }
}
