using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ClientApp
{
    class Program
    {
        private const string baseUrl = "http://localhost:7500";
        static void Main(string[] args)
        {

            GetAllData();
            Console.ReadKey();

        }

        static async void GetAllData()
        {
            Console.WriteLine("Getting Products..");
            await GetProducts();
           await  GetUsers();
           
        }
        static async Task GetProducts()
        {
            //We will make a GET request to a really cool website...

            var url = $"{baseUrl}/products";
            //The 'using' will help to prevent memory leaks.
            //Create a new instance of HttpClient
            using (HttpClient client = new HttpClient())

            //Setting up the response...         

            using (HttpResponseMessage res = await client.GetAsync(url))
            using (HttpContent content = res.Content)
            {
                string data = await content.ReadAsStringAsync();
                if (data != null)
                {
                    Console.WriteLine(data);
                }
            }
        }

        static async Task GetUsers()
        {
            //We will make a GET request to a really cool website...

            var url = $"{baseUrl}/users";
          
            Console.WriteLine("Getting Users without token..");
            using (HttpClient client = new HttpClient())
            {
              
                //Setting up the response...         

                using (HttpResponseMessage res = await client.GetAsync(url))
                using (HttpContent content = res.Content)
                {
                    string data = await content.ReadAsStringAsync();

                    Console.WriteLine($"StatusCode: {res.StatusCode} data:{data}");

                }
            }

            Console.WriteLine("Getting Users with token..");
            var token = await GetToken();
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                //Setting up the response...         

                using (HttpResponseMessage res = await client.GetAsync(url))
                using (HttpContent content = res.Content)
                {
                    string data = await content.ReadAsStringAsync();
                  
                    Console.WriteLine($"StatusCode: {res.StatusCode} data:{data}");

                }
            }
        }

        static async Task<string> GetToken()
        {
            var user = new { UserName = "admin", Password = "admin123" };
            var content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");

            var url = $"{baseUrl}/api/auth";
          
            using (HttpClient client = new HttpClient())

            //Setting up the response...         

            using (HttpResponseMessage res = await client.PostAsync(url, content))
            using (HttpContent resContent = res.Content)
            {
                string data = await resContent.ReadAsStringAsync();
                if (data != null)
                {
                    var resJson = JsonConvert.DeserializeObject(data) as dynamic;
                    return resJson.token;
                }
                else return null;
            }
        }
    }
}
