using HtmlAgilityPack;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Text;

using UploadService.Model;

namespace sqltest {
    class Program {
        static void Main(string[] args) {
            string API = "http://xamxontacts.azurewebsites.net/tables/ingredients?ZUMO-API-VERSION=2.0.0";
            //SendDataToServerAsync(API);
            try {

                SqlConnectionStringBuilder strngbuilder = new SqlConnectionStringBuilder {
                    DataSource = "xamserver.database.windows.net",
                    UserID = "edu123",
                    Password = "Password123",
                    InitialCatalog = "XamContactDb"
                };

                var cmdText = @"insert into dbo.Ingredients (name, id, version, createdAt, updatedAt, deleted) values (@name, @id, @createdAt, @updatedAt, @updatedAt @deleted)";

                foreach (var item in MakeIngridientsList()) {

                    using (SqlConnection conn = new SqlConnection(strngbuilder.ConnectionString)) {

                        var command = new SqlCommand(cmdText, conn);
                        command.Parameters.AddWithValue("@name", item.name);
                        command.Parameters.AddWithValue("@id", item.id);
                        command.Parameters.AddWithValue("@version", item.version);
                        command.Parameters.AddWithValue("@createdAt", item.createdAt);
                        command.Parameters.AddWithValue("@updatedAt", item.updatedAt);
                        command.Parameters.AddWithValue("@deleted", item.deleted);
                        conn.Open();
                        command.ExecuteNonQuery();
                        Console.WriteLine("Ok");
                    }
                }
            } catch (SqlException e) {
                Console.WriteLine(e.ToString());
            }
            Console.ReadLine();
        }


        private static List<Ingredient> MakeIngridientsList() {
            List<Ingredient> ingredients = new();
            var web = new HtmlWeb();
            for (char alphabet = 'a'; alphabet <= 'z'; alphabet++) {

                var doc = web.Load($"https://www.bbc.co.uk/food/ingredients/a-z/{alphabet}");
                var nodes = doc.DocumentNode.SelectNodes("//a[@class = 'pagination__link gel-pica-bold']/@href");
                var pagesNum = nodes == null ? 1 : nodes.Count();

                for (int i = 1; i <= pagesNum; i++) {
                    doc = web.Load($"https://www.bbc.co.uk/food/ingredients/a-z/{alphabet}/{i}");

                    // No material starts with x, so when alphabet is ‘x’, the page will automatically redirect to the homepage.
                    // Determine whether the current doc is the home page through this method.
                    if (doc.DocumentNode.SelectNodes("//*[@class = 'az-keyboard__list']") == null)
                        break;

                    var ingridients = doc.DocumentNode.SelectNodes("//*[@class = 'gel-layout__item gel-1/2 gel-1/3@m gel-1/4@xl']")
                    .ToList();

                    foreach (var item in ingridients) {
                        var text = item.InnerText.Replace("ingredient", string.Empty);
                        Ingredient ingredient = new Ingredient() {
                            id = new Guid().ToString(),
                            name = text,
                            deleted = false,
                            version = "1",
                            createdAt = DateTime.Now,
                            updatedAt = DateTime.Now
                        };
                        ingredients.Add(ingredient);
                    }
                }
            }
            ingredients.Add(new Ingredient() {
                id = new Guid().ToString(),
                name = "00 Flour",
                deleted = false,
                version = "1",
                createdAt = DateTime.Now,
                updatedAt = DateTime.Now

            });
            return ingredients;
        }

        // This way did not work, but I think is the best way to do it
        private static async void SendDataToServerAsync(string API) {

            using HttpClient client = new();
            string json = JsonConvert.SerializeObject(MakeIngridientsList(), Formatting.Indented);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage result = await client.PostAsync(API, content);
            if (result.IsSuccessStatusCode) {
                Console.WriteLine("Success");
            }
        }
    }
}