namespace Crud_Practice.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class EmployeeModel
    {
        public int Eid { get; set; }

        [Required(ErrorMessage = "Employee Name is required.")]
        [StringLength(100, ErrorMessage = "Name can't exceed 100 characters.")]
        public string EName { get; set; }

        [Required(ErrorMessage = "Employee Code is required.")]
        [StringLength(10, ErrorMessage = "Code can't exceed 10 characters.")]
        public string ECode { get; set; }

        [Required(ErrorMessage = "Department is required.")]
        public int DepartmentID { get; set; }

        [Required(ErrorMessage = "Gender is required.")]
        [RegularExpression("^[MF]$", ErrorMessage = "Gender must be 'M' or 'F'.")]
        public string Gender { get; set; }

        [Required(ErrorMessage = "Salary is required.")]
        [Range(1, double.MaxValue, ErrorMessage = "Salary must be a positive number.")]
        public decimal? Salary { get; set; }

        public List<int> SelectedProjects { get; set; } // Changed from string to List<int>
    }

    public class DepartmentDropDownModel
    {
        public int DepartmentID { get; set; }
        public string DepartmentName { get; set; }
    }
}