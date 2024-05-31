class Program
{
    static async Task Main(string[] args)
    {
        // Example URLs to fetch data from
        List<string> urls = new List<string>
        {
            "https://jsonplaceholder.typicode.com/posts/1",
            "https://jsonplaceholder.typicode.com/posts/2",
            "https://jsonplaceholder.typicode.com/posts/3"
        };

        // Fetch and process data in parallel
        await ProcessDataInParallelAsync(urls);

        // Cancellation example
        CancellationTokenSource cts = new CancellationTokenSource();
        Task fetchTask = FetchDataWithCancellationAsync("https://jsonplaceholder.typicode.com/posts/1", cts.Token);

        // Cancel the task after 500ms
        cts.CancelAfter(500);

        try
        {
            await fetchTask;
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Task was canceled.");
        }

        // Consume async stream
        await foreach (var number in GenerateNumbersAsync())
        {
            Console.WriteLine(number);
        }
    }

    // Fetch data asynchronously from a given URL
    public static async Task<string> FetchDataAsync(string url)
    {
        using (HttpClient client = new HttpClient())
        {
            try
            {
                string data = await client.GetStringAsync(url);
                return data;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Request error: {e.Message}");
                return string.Empty;
            }
        }
    }

    // Fetch data with cancellation support
    public static async Task<string> FetchDataWithCancellationAsync(string url, CancellationToken cancellationToken)
    {
        using (HttpClient client = new HttpClient())
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync(url, cancellationToken);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Request error: {e.Message}");
                return string.Empty;
            }
        }
    }

    // Process data in parallel
    public static async Task ProcessDataInParallelAsync(List<string> urls)
    {
        var tasks = new List<Task<string>>();

        foreach (var url in urls)
        {
            tasks.Add(FetchDataAsync(url));
        }

        string[] results = await Task.WhenAll(tasks);

        foreach (var result in results)
        {
            Console.WriteLine(result);
        }
    }

    // Generate numbers asynchronously using async streams
    public static async IAsyncEnumerable<int> GenerateNumbersAsync()
    {
        for (int i = 0; i < 10; i++)
        {
            await Task.Delay(1000); // Simulate asynchronous work
            yield return i;
        }
    }
}
