using Crud_Practice.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;

namespace Crud_Practice.Controllers
{
    public class ItemsController : Controller
    {
        private readonly IConfiguration _configuration;

        public ItemsController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult ItemList()
        {
            List<ItemModel> itemList = new List<ItemModel>();
            string connectionString = _configuration.GetConnectionString("ConnectionString");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand("PR_Items_SelectAll", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    itemList.Add(new ItemModel
                    {
                        ItemId = Convert.ToInt32(reader["ItemId"]),
                        Name = reader["Name"].ToString(),
                        Price = Convert.ToDecimal(reader["Price"]),
                        Category = reader["Category"].ToString(),
                        IsAvailable = Convert.ToBoolean(reader["IsAvailable"]),
                        Features = reader["Features"].ToString().Split(',').ToList(),
                        Tags = reader["Tags"].ToString(),
                        SelectedRadioOption = reader["Category"].ToString() // Example of radio binding
                    });
                }
            }
            return View(itemList);
        }

        public IActionResult ItemAddEdit(int? id)
        {
            ItemModel model = new ItemModel();
            if (id.HasValue)
            {
                string connectionString = _configuration.GetConnectionString("ConnectionString");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand("PR_Items_SelectById", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    command.Parameters.AddWithValue("@ItemId", id.Value);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        model = new ItemModel
                        {
                            ItemId = Convert.ToInt32(reader["ItemId"]),
                            Name = reader["Name"].ToString(),
                            Price = Convert.ToDecimal(reader["Price"]),
                            Category = reader["Category"].ToString(),
                            IsAvailable = Convert.ToBoolean(reader["IsAvailable"]),
                            Features = reader["Features"].ToString().Split(',').ToList(),
                            Tags = reader["Tags"].ToString(),
                            SelectedRadioOption = reader["Category"].ToString()
                        };
                    }
                }
            }
            return View(model);
        }

        [HttpPost]
        public IActionResult ItemSave(ItemModel model)
        {
            if (ModelState.IsValid)
            {
                string connectionString = _configuration.GetConnectionString("ConnectionString");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(model.ItemId == null ? "PR_Items_Insert" : "PR_Items_Update", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    if (model.ItemId != null)
                    {
                        command.Parameters.AddWithValue("@ItemId", model.ItemId);
                    }
                    command.Parameters.AddWithValue("@Name", model.Name);
                    command.Parameters.AddWithValue("@Price", model.Price);
                    command.Parameters.AddWithValue("@Category", model.SelectedRadioOption);
                    command.Parameters.AddWithValue("@IsAvailable", model.IsAvailable);
                    command.Parameters.AddWithValue("@Features", string.Join(",", model.Features));
                    command.Parameters.AddWithValue("@Tags", model.Tags);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
                return RedirectToAction("ItemList");
            }
            return View("ItemAddEdit", model);
        }

        [HttpPost]
        public IActionResult ItemDelete(int id)
        {
            string connectionString = _configuration.GetConnectionString("ConnectionString");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand("PR_Items_Delete", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@ItemId", id);
                connection.Open();
                command.ExecuteNonQuery();
            }
            return RedirectToAction("ItemList");
        }

        [HttpPost]
        public IActionResult MultiDelete(int[] ids)
        {
            if (ids.Length > 0)
            {
                string connectionString = _configuration.GetConnectionString("ConnectionString");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand("PR_Items_MultiDelete", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    command.Parameters.AddWithValue("@ItemIds", string.Join(",", ids));
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            return RedirectToAction("ItemList");
        }
    }

}
