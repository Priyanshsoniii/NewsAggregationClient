// UI/Validators/InputValidator.cs
using System.Text.RegularExpressions;

namespace NewsAggregationClient.UI.Validators;

public static class InputValidator
{
    private static readonly Regex EmailRegex = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);
    private static readonly Regex PasswordRegex = new(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$", RegexOptions.Compiled);
    private static readonly Regex UsernameRegex = new(@"^[a-zA-Z0-9_]{3,20}$", RegexOptions.Compiled);

    public static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        return EmailRegex.IsMatch(email);
    }

    public static bool IsValidPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            return false;

        return PasswordRegex.IsMatch(password);
    }

    public static bool IsValidUsername(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            return false;

        return UsernameRegex.IsMatch(username);
    }

    public static string GetEmailValidationMessage()
    {
        return "Email must be in valid format (e.g., user@example.com)";
    }

    public static string GetPasswordValidationMessage()
    {
        return "Password must be at least 8 characters long and contain at least one uppercase letter, one lowercase letter, and one digit";
    }

    public static string GetUsernameValidationMessage()
    {
        return "Username must be 3-20 characters long and contain only letters, numbers, and underscores";
    }

    public static bool IsValidMenuChoice(string input, int minChoice, int maxChoice)
    {
        if (string.IsNullOrWhiteSpace(input))
            return false;

        if (!int.TryParse(input, out int choice))
            return false;

        return choice >= minChoice && choice <= maxChoice;
    }

    public static bool IsValidDate(string dateString)
    {
        return DateTime.TryParse(dateString, out _);
    }

    public static bool IsValidDateRange(DateTime startDate, DateTime endDate)
    {
        return startDate <= endDate && startDate <= DateTime.Now.Date;
    }

    public static bool IsValidArticleId(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return false;

        return int.TryParse(input, out int id) && id > 0;
    }

    public static bool IsValidSearchQuery(string query)
    {
        return !string.IsNullOrWhiteSpace(query) && query.Trim().Length >= 2;
    }

    public static bool IsValidKeyword(string keyword)
    {
        return !string.IsNullOrWhiteSpace(keyword) && keyword.Trim().Length >= 2;
    }

    public static List<string> ValidateKeywords(List<string> keywords)
    {
        var validKeywords = new List<string>();

        foreach (var keyword in keywords)
        {
            var trimmedKeyword = keyword?.Trim();
            if (IsValidKeyword(trimmedKeyword))
            {
                validKeywords.Add(trimmedKeyword!);
            }
        }

        return validKeywords.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
    }
}