namespace NewsAggregation.Client.UI.Interfaces;

public interface IMenuHandler
{
    Task HandleMenuAsync(UserDto user);
}