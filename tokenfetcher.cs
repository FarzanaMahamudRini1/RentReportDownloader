using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;

public class TokenFetcher
{
    public async Task<string> GetApiToken()
    {
        string apiToken = null;
        using (HttpClient client = new HttpClient())
        {
            client.BaseAddress = new Uri("https://della.api.rentmanager.com/");

            // Set up the request headers
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            UserAuthorizationModel uam = new UserAuthorizationModel
            {
                Username = Environment.GetEnvironmentVariable("API_USERNAME"),
                Password = Environment.GetEnvironmentVariable("API_PASSWORD"),
                LocationID = int.Parse(Environment.GetEnvironmentVariable("API_LOCATIONID"))

            };

            try
            {
                // Make the POST request to get the token
                HttpResponseMessage response = await client.PostAsJsonAsync("/Authentication/AuthorizeUser", uam);

                if (response.IsSuccessStatusCode)
                {
                    // Extract and return the token
                    apiToken = await response.Content.ReadAsStringAsync();
                    apiToken = apiToken.Trim('"');
                }
                else
                {
                    Console.WriteLine($"Failed to retrieve token: {response.StatusCode}");
                }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Request error: {e.Message}");
            }
        }

        return apiToken;
    }
}

public class UserAuthorizationModel
{
    public string Username { get; set; }
    public string Password { get; set; }
    public int LocationID { get; set; }
}