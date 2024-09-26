using Crud_Practice.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;

public class EmployeeController : Controller
{
    private readonly IConfiguration configuration;

    public EmployeeController(IConfiguration configuration)
    {
        this.configuration = configuration;
    }

    #region EmployeeList
    public IActionResult EmployeeList()
    {
        string connectionString = this.configuration.GetConnectionString("ConnectionString");
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "PR_Employee_SelectAll";
            SqlDataReader reader = command.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);
            return View(table);
        }
    }
    #endregion

    #region EmployeeAddEdit

    public IActionResult EmployeeAddEdit(int eid)
    {
        EmployeeModel employeeModel = new EmployeeModel
        {
            SelectedProjects = new List<int>() // Initialize SelectedProjects
        };

        // Load the department dropdown
        LoadDepartmentDropDown();

        if (eid > 0)
        {
            string connectionString = this.configuration.GetConnectionString("ConnectionString");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "PR_Employee_SelectByPK";
                command.Parameters.AddWithValue("@Eid", eid);
                SqlDataReader reader = command.ExecuteReader();
                DataTable table = new DataTable();
                table.Load(reader);

                foreach (DataRow row in table.Rows)
                {
                    employeeModel.Eid = Convert.ToInt32(row["Eid"]);
                    employeeModel.EName = row["EName"].ToString();
                    employeeModel.ECode = row["ECode"].ToString();
                    employeeModel.DepartmentID = Convert.ToInt32(row["DepartmentID"]);
                    employeeModel.Gender = row["Gender"].ToString();
                    employeeModel.Salary = Convert.ToDecimal(row["Salary"]);

                    // Convert comma-separated string to List<int>
                    string projectsString = row["Projects"].ToString();
                    if (!string.IsNullOrWhiteSpace(projectsString))
                    {
                        var projectStrings = projectsString.Split(',');
                        foreach (var project in projectStrings)
                        {
                            if (int.TryParse(project, out int projectId))
                            {
                                employeeModel.SelectedProjects.Add(projectId);
                            }
                            else
                            {
                                // Handle invalid project ID if needed
                            }
                        }
                    }
                }
            }
        }

        return View("EmployeeAddEdit", employeeModel);
    }
    #endregion

    #region EmployeeSave

    [HttpPost]
    public IActionResult EmployeeSave(EmployeeModel employeeModel, List<int> selectedProjects)
    {
        LoadDepartmentDropDown();

        if (ModelState.IsValid)
        {
            string connectionString = this.configuration.GetConnectionString("ConnectionString");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;

                if (employeeModel.Eid == 0)
                {
                    command.CommandText = "PR_Employee_Insert";
                }
                else
                {
                    command.CommandText = "PR_Employee_Update";
                    command.Parameters.Add("@Eid", SqlDbType.Int).Value = employeeModel.Eid;
                }

                command.Parameters.Add("@EName", SqlDbType.VarChar).Value = employeeModel.EName;
                command.Parameters.Add("@ECode", SqlDbType.VarChar).Value = employeeModel.ECode;
                command.Parameters.Add("@DepartmentID", SqlDbType.Int).Value = employeeModel.DepartmentID;
                command.Parameters.Add("@Gender", SqlDbType.VarChar).Value = employeeModel.Gender;
                command.Parameters.Add("@Salary", SqlDbType.Decimal).Value = employeeModel.Salary;

                // Convert selected projects list to comma-separated string
                string projects = string.Join(",", selectedProjects);
                command.Parameters.Add("@Projects", SqlDbType.VarChar).Value = projects;

                command.ExecuteNonQuery();
            }

            return RedirectToAction("EmployeeList");
        }

        return View("EmployeeAddEdit", employeeModel);
    }
    #endregion

    #region LoadDepartmentDropDown

    private void LoadDepartmentDropDown()
    {
        string connectionString = this.configuration.GetConnectionString("ConnectionString");
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "PR_Department_DropDown";
            SqlDataReader reader = command.ExecuteReader();
            DataTable dataTable = new DataTable();
            dataTable.Load(reader);

            List<DepartmentDropDownModel> departmentList = new List<DepartmentDropDownModel>();
            foreach (DataRow data in dataTable.Rows)
            {
                DepartmentDropDownModel departmentDropDownModel = new DepartmentDropDownModel
                {
                    DepartmentID = Convert.ToInt32(data["DepartmentID"]),
                    DepartmentName = data["DepartmentName"].ToString()
                };
                departmentList.Add(departmentDropDownModel);
            }

            ViewBag.DepartmentList = departmentList;
        }
    }

    #endregion

    #region Delete

    [HttpPost]
    public IActionResult Delete(int eid)
    {
        string connectionString = configuration.GetConnectionString("ConnectionString");
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            SqlCommand command = new SqlCommand("PR_Employee_Delete", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.AddWithValue("@Eid", eid);
            connection.Open();
            command.ExecuteNonQuery();
        }
        return RedirectToAction("EmployeeList");
    }


    #endregion

    #region MultiDelete

    [HttpPost]
    public IActionResult DeleteMultiple(List<int> selectedEids)
    {
        if (selectedEids != null && selectedEids.Any())
        {
            string employeeIds = string.Join(",", selectedEids); // Convert list to comma-separated string
            string connectionString = configuration.GetConnectionString("ConnectionString");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "PR_Employee_DeleteMultiple";
                command.Parameters.AddWithValue("@EmployeeIDs", employeeIds);
                command.ExecuteNonQuery();
            }
        }

        return RedirectToAction("EmployeeList");
    }

    #endregion




}
