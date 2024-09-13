using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ReadAPIClient
{
    public class APIClient
    { 

        public static async Task GETAsync()
        {
            // Use HttpClient in a 'using' block to ensure it is disposed of correctly
            using (HttpClient _httpClient = new HttpClient())
            {
                try
                {
                    // Handle SSL certificate validation bypass for development (if needed)
                    _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    // Making a GET request
                    try
                    {
                        HttpResponseMessage _httpResponseMessage = await _httpClient.GetAsync("http://localhost:5165/api/Data/GetCSISData?Cycleid=420");
                        _httpResponseMessage.EnsureSuccessStatusCode();
                        var jsonResponse = await _httpResponseMessage.Content.ReadAsStringAsync();

                        // Log the response to the console
                        Console.WriteLine($"Response: {jsonResponse}\n");

                    }
                    catch (Exception ex )
                    {

                        await Console.Out.WriteLineAsync("ex");
                    }
                   // HttpResponseMessage _httpResponseMessage = await _httpClient.GetAsync("https://10.2.10.19:7068/api/Data/GetCSISData?Cycleid=420");

                   
                }
                catch (HttpRequestException httpEx)
                {
                    // Log HttpRequestException if the request fails
                    Console.WriteLine($"Request error: {httpEx.Message}");
                }
                catch (Exception ex)
                {
                    // Log any other exceptions
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }
                finally
                {
                    Console.ReadKey(); // Pause the console to view the output
                }
            }
        }

        //public static async Task GETAsync(HttpClient _httpClient)
        //{


        //    HttpResponseMessage _httpResponseMessage = await _httpClient.GetAsync("https://10.2.10.19:7068/api/Data/GetCSISData?Cycleid=723");

        //    _httpResponseMessage.EnsureSuccessStatusCode();

        //    var jasonResponse = await _httpResponseMessage.Content.ReadAsStringAsync();
        //    Console.WriteLine($"{jasonResponse}\n");

        //    Console.ReadKey();
        //}

        
        

    }
}
