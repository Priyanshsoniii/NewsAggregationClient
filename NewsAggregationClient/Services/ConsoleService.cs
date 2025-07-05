// Services/ConsoleService.cs
using NewsAggregationClient.Services.Interfaces;

namespace NewsAggregationClient.Services;

public class ConsoleService : IConsoleService
{
    public void WriteLine(string message)
    {
        Console.WriteLine(message);
    }

    public void WriteLine(string message, ConsoleColor color)
    {
        var originalColor = Console.ForegroundColor;
        Console.ForegroundColor = color;
        Console.WriteLine(message);
        Console.ForegroundColor = originalColor;
    }

    public void Write(string message)
    {
        Console.Write(message);
    }

    public void Write(string message, ConsoleColor color)
    {
        var originalColor = Console.ForegroundColor;
        Console.ForegroundColor = color;
        Console.Write(message);
        Console.ForegroundColor = originalColor;
    }

    public string ReadLine()
    {
        return Console.ReadLine() ?? string.Empty;
    }

    public ConsoleKeyInfo ReadKey()
    {
        return Console.ReadKey();
    }

    public void Clear()
    {
        Console.Clear();
    }

    public void SetCursorPosition(int left, int top)
    {
        Console.SetCursorPosition(left, top);
    }

    public string ReadPassword()
    {
        var password = string.Empty;
        ConsoleKeyInfo key;

        do
        {
            key = Console.ReadKey(true);

            if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
            {
                password += key.KeyChar;
                Console.Write("*");
            }
            else if (key.Key == ConsoleKey.Backspace && password.Length > 0)
            {
                password = password[0..^1];
                Console.Write("\b \b");
            }
        }
        while (key.Key != ConsoleKey.Enter);

        Console.WriteLine();
        return password;
    }

    public int ReadInteger(string prompt, int min = int.MinValue, int max = int.MaxValue)
    {
        int result;
        while (true)
        {
            Write(prompt);
            var input = ReadLine();

            if (int.TryParse(input, out result) && result >= min && result <= max)
            {
                return result;
            }

            DisplayError($"Please enter a valid number between {min} and {max}.");
        }
    }

    public DateTime ReadDate(string prompt)
    {
        DateTime result;
        while (true)
        {
            Write($"{prompt} (yyyy-mm-dd): ");
            var input = ReadLine();

            if (DateTime.TryParse(input, out result))
            {
                return result;
            }

            DisplayError("Please enter a valid date in yyyy-mm-dd format.");
        }
    }

    public bool ReadYesNo(string prompt)
    {
        while (true)
        {
            Write($"{prompt} (y/n): ");
            var input = ReadLine().ToLower();

            if (input == "y" || input == "yes")
                return true;
            if (input == "n" || input == "no")
                return false;

            DisplayError("Please enter 'y' for yes or 'n' for no.");
        }
    }

    public void PressAnyKeyToContinue()
    {
        WriteLine("\nPress any key to continue...", ConsoleColor.Gray);
        ReadKey();
    }

    public void DisplayHeader(string title)
    {
        Clear();
        var headerLength = Math.Max(title.Length + 4, 50);
        var separator = new string('=', headerLength);

        WriteLine(separator, ConsoleColor.Cyan);
        WriteLine($"  {title.ToUpper()}", ConsoleColor.Cyan);
        WriteLine(separator, ConsoleColor.Cyan);
        WriteLine("");
    }

    public void DisplaySeparator()
    {
        WriteLine(new string('-', 50), ConsoleColor.Gray);
    }

    public void DisplayError(string error)
    {
        WriteLine($"ERROR: {error}", ConsoleColor.Red);
    }

    public void DisplaySuccess(string message)
    {
        WriteLine($"SUCCESS: {message}", ConsoleColor.Green);
    }

    public void DisplayWarning(string warning)
    {
        WriteLine($"WARNING: {warning}", ConsoleColor.Yellow);
    }
}