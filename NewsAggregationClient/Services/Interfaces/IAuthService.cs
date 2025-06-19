
public interface IAuthService
{
    Task RegisterAsync();
    Task<TokenResponseDto?> LoginAsync();
}