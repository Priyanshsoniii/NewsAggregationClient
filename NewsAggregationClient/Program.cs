using NewsAggregationClient.Services;

public class Program {
    static void Main(string[] args)
    {
        var httpClient = HttpClientFactory.CreateClient();
        IAuthService? authService = new AuthService(httpClient) as IAuthService;
        var menu = new MenuService(authService);

        menu.ShowMainMenuAsync().GetAwaiter().GetResult();
    }

}

