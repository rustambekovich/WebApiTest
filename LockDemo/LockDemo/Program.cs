using System.Diagnostics;

class Program
{
    private static async Task SendPostRequestAsync(HttpClient client, string apiUrl, int requestNumber)
    {
        var time = new Stopwatch();
        time.Start();
        try
        {
            var formData = new MultipartFormDataContent();

            formData.Add(new StringContent($"John Doe {requestNumber}"), "Name");
            formData.Add(new StringContent($"alik@example.com"), "Email");
            formData.Add(new StringContent("1990-01-01"), "Birthday");

            // Send the POST request
            HttpResponseMessage response = await client.PostAsync(apiUrl, formData);

            // Ensure successful response
            response.EnsureSuccessStatusCode();

            string responseData = await response.Content.ReadAsStringAsync();
            time.Stop();
            Console.WriteLine($"Request {requestNumber} succeeded. Response: {responseData} : {time.ElapsedMilliseconds} ms");
        }
        catch (Exception ex)
        {
            time.Stop();
            Console.WriteLine($"Request {requestNumber} failed. Error: {ex.Message} : {time.ElapsedMilliseconds} ms");
        }
    }

    private static async Task SendConcurrentRequestsAsync(string url, int numberOfRequests)
    {
        var time = new Stopwatch();
        time.Start();

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

        time.Stop();

        await Console.Out.WriteLineAsync($"Umumiy vaqt: {time.ElapsedMilliseconds} ms\n Ortacha vaqt {time.ElapsedMilliseconds / numberOfRequests} ms");
    }

    static async Task Main(string[] args)
    {
        string url = "https://localhost:7296/api/Register";  // Replace with your actual URL
        int numberOfRequests = 10000;

        Console.WriteLine($"Sending {numberOfRequests} requests to {url}...");

        // Send all requests concurrently
        await SendConcurrentRequestsAsync(url, numberOfRequests);

        Console.WriteLine("All requests completed.");
    }
}
