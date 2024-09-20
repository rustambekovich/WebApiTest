using System;
using System.Net.Http;
using System.Threading.Tasks;

class Program
{
    private static async Task SendPostRequestAsync(HttpClient client, string apiUrl, int requestNumber)
    {
        try
        {
            var formData = new MultipartFormDataContent();

            formData.Add(new StringContent($"John Doe {requestNumber}"), "Name");
            formData.Add(new StringContent($"johndoe@example.com"), "Email");
            formData.Add(new StringContent("1990-01-01"), "Birthday");

            // Send the POST request
            HttpResponseMessage response = await client.PostAsync(apiUrl, formData);

            // Ensure successful response
            response.EnsureSuccessStatusCode();

            string responseData = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Request {requestNumber} succeeded. Response: {responseData}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Request {requestNumber} failed. Error: {ex.Message}");
        }
    }

    private static async Task SendConcurrentRequestsAsync(string url, int numberOfRequests)
    {
        using (HttpClient httpClient = new HttpClient())
        {
            Task[] tasks = new Task[numberOfRequests];

            for (int i = 0; i < numberOfRequests; i++)
            {
                // Start a new request
                tasks[i] = SendPostRequestAsync(httpClient, url, i + 1);
            }

            // Wait for all requests to complete
            await Task.WhenAll(tasks);
        }
    }

    static async Task Main(string[] args)
    {
        string url = "http://45.130.148.192:6666/api/Register";  // Replace with your actual URL
        int numberOfRequests = 1000;

        Console.WriteLine($"Sending {numberOfRequests} requests to {url}...");

        // Send all requests concurrently
        await SendConcurrentRequestsAsync(url, numberOfRequests);

        Console.WriteLine("All requests completed.");
    }
}
