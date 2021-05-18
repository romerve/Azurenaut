using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace SalesFuncApp
{
    public static class Products
    {
        [FunctionName("Products")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            List<ProductItem> productList = new List<ProductItem>();

            var str = Environment.GetEnvironmentVariable("SqlDb");
            using (SqlConnection conn = new SqlConnection(str))
            {
                conn.Open();
                var text = "SELECT TOP (20) * FROM SalesLT.Product ";

                using (SqlCommand cmd = new SqlCommand(text, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ProductItem item = new ProductItem()
                            {
                                ProductID = reader["ProductID"].ToString(),
                                Name = reader["Name"].ToString(),
                                ProductNumber = reader["ProductNumber"].ToString(),
                                Color = reader["Color"].ToString(),
                                StandardCost = reader["StandardCost"].ToString(),
                                ListPrice = reader["ListPrice"].ToString(),
                                Size = reader["Size"].ToString(),
                                Weight = reader["Weight"].ToString()
                            };

                            productList.Add(item);

                        }
                    }
                }
                conn.Close();
            }

            dynamic data = JsonSerializer.Serialize<IList<ProductItem>>(productList);

            return new OkObjectResult(data);
        }
    }

    public class ProductItem
    {
        public string ProductID { get; set; }
        public string Name { get; set; }
        public string ProductNumber { get; set; }
        public string Color { get; set; }
        public string StandardCost { get; set; }
        public string ListPrice { get; set; }
        public string Size { get; set; }
        public string Weight { get; set; }
    }
}
