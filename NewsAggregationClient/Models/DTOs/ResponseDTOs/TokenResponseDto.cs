public class TokenResponseDto
{
    public string Token { get; set; }
    public UserDto User { get; set; }
    public string Message { get; set; }
}

public class UserDto
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public string Role { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }
    public List<object> SavedArticles { get; set; }
    public List<object> Notifications { get; set; }
    public List<object> NotificationSettings { get; set; }
}