namespace api.Controllers.Player;

public class Target
{
    static async Task Main(string[] args)
    {
        string targetIp = "http://<target-ip>"; // Replace <target-ip> with the target IP or URL
        int requestCount = 0; // Counter to keep track of requests
        int maxRequests = 2000; // Total number of requests to send

        using (HttpClient client = new HttpClient())
        {
            while (requestCount < maxRequests)
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(targetIp);
                    Console.WriteLine($"Request {requestCount + 1}: {response.StatusCode}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Request {requestCount + 1}: Failed - {ex.Message}");
                }

                requestCount++;
            }
        }

        Console.WriteLine("Completed sending 2000 requests.");
    }
}