public class HttpClientFactory
{
    public static HttpClient CreateClient()
    {
        return new HttpClient
        {
            BaseAddress = new Uri("https://localhost:7000/api")
        };
    }
}