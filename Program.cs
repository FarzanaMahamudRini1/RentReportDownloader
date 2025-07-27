using System;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;

class Apitester
{
    static async Task Main(string[] args)
    {
        // Fetch the API token using TokenFetcher
        TokenFetcher tokenFetcher = new TokenFetcher();
        string apiToken = await tokenFetcher.GetApiToken();

        if (string.IsNullOrEmpty(apiToken))
        {
            Console.WriteLine("Failed to retrieve API token.");
            return;
        }

        // Load propids
        string propidFilePath = @"C:.\data\propids.txt";
        if (!File.Exists(propidFilePath))
        {
            Console.WriteLine("Propid file not found.");
            return;
        }

        string[] propids = File.ReadAllText(propidFilePath).Split(',');

        // April 2025 configuration
        string startDate = "04/01/2025";
        string endDate = "04/30/2025";
        string themonth = "04";
        string endday = "30";
        string theYear = "2025";

        // Initialize HttpClient
        using (HttpClient client = new HttpClient())
        {
            client.Timeout = TimeSpan.FromMinutes(15);
            client.BaseAddress = new Uri("https://della.api.rentmanager.com/");

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("X-Api-Token", apiToken); // generalized
            client.DefaultRequestHeaders.Add("X-Location-ID", Environment.GetEnvironmentVariable("API_LOCATIONID"));


            foreach (string propid in propids)
            {
                string trimmedPropid = propid.Trim();
                if (string.IsNullOrEmpty(trimmedPropid)) continue;

                string url = $"/Reports/1/RunReport?parameters=StartDate,{startDate};EndDate,{endDate};PropOwnerIDs,{trimmedPropid}&GetOptions=ReturnCSVStream";
                HttpResponseMessage response = await client.GetAsync(url);

                try
                {
                    response.EnsureSuccessStatusCode();
                }
                catch (HttpRequestException ex)
                {
                    if (response.StatusCode == HttpStatusCode.NotFound)
                    {
                        Console.WriteLine($"Report not found for {trimmedPropid} in March 2025");
                        continue;
                    }
                    else
                    {
                        Console.WriteLine($"Error for {trimmedPropid} (April 2025): {ex.Message}");
                        continue;
                    }
                }

                string reportPath = "reports, trimmedPropid";
                Directory.CreateDirectory(reportPath);

                string reportFile = Path.Combine(reportPath, $"Report_{themonth}_{endday}_{theYear}.csv");

                using (Stream reportStream = await response.Content.ReadAsStreamAsync())
                {
                    if (reportStream.GetType().Name == "MemoryStream")
                    {
                        if (File.Exists(reportFile))
                        {
                            File.Delete(reportFile);
                        }

                        using (Stream fileStream = File.Create(reportFile))
                        {
                            await reportStream.CopyToAsync(fileStream);
                        }
                    }
                }

                Console.WriteLine($"✔ Saved: {reportFile}");
            }
        }
    }
}
