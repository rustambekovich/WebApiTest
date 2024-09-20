using System.Diagnostics;

class Program
{
    public static long allTime { get; set; } = 0;  // Total time for all requests
    private static async Task SendPostRequestAsync(HttpClient client, string apiUrl, int requestNumber)
    {
        var time = new Stopwatch();  // Stopwatch to measure time for this request
        time.Start();
        try
        {
            var formData = new MultipartFormDataContent();

            formData.Add(new StringContent($"Assalomu alaykum {requestNumber}"), "Name");
            formData.Add(new StringContent($"2"), "Email");
            formData.Add(new StringContent("Assalomu alaykum"), "Birthday");

            HttpResponseMessage response = await client.PostAsync(apiUrl, formData);

            // Ensure successful response
            response.EnsureSuccessStatusCode();

            string responseData = await response.Content.ReadAsStringAsync();
            time.Stop();  
            long elapsedMs = time.ElapsedMilliseconds;  
            allTime += elapsedMs; 

            Console.WriteLine($"Request {requestNumber} succeeded. Time taken: {elapsedMs} ms. Response: {responseData}");
        }
        catch (Exception ex)
        {
            time.Stop(); 
            long elapsedMs = time.ElapsedMilliseconds;
            allTime += elapsedMs;

            Console.WriteLine($"Request {requestNumber} failed. Time taken: {elapsedMs} ms. Error: {ex.Message}");
        }
    }

    private static async Task SendConcurrentRequestsAsync(string url, int numberOfRequests)
    {
        using (HttpClient httpClient = new HttpClient())
        {
            Task[] tasks = new Task[numberOfRequests]; 

            for (int i = 0; i < numberOfRequests; i++)
            {
                tasks[i] = SendPostRequestAsync(httpClient, url, i + 1);
            }

            await Task.WhenAll(tasks);
        }

        Console.WriteLine($"\nTotal time for all {numberOfRequests} requests: {allTime} ms");
        Console.WriteLine($"Average time per request: {allTime / numberOfRequests} ms");
    }

    static async Task Main(string[] args)
    {
        string url = "http://45.130.148.192:6666/api/Register";  
        int numberOfRequests = 10000; 

        Console.WriteLine($"Sending {numberOfRequests} requests to {url}...");

        await SendConcurrentRequestsAsync(url, numberOfRequests);

        Console.WriteLine("All requests completed.");
    }
}
