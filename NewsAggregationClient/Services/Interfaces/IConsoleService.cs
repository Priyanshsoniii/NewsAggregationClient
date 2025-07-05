namespace NewsAggregationClient.Services.Interfaces;

public interface IConsoleService
{
    void WriteLine(string message);
    void WriteLine(string message, ConsoleColor color);
    void Write(string message);
    void Write(string message, ConsoleColor color);
    string ReadLine();
    ConsoleKeyInfo ReadKey();
    void Clear();
    void SetCursorPosition(int left, int top);
    string ReadPassword();
    int ReadInteger(string prompt, int min = int.MinValue, int max = int.MaxValue);
    DateTime ReadDate(string prompt);
    bool ReadYesNo(string prompt);
    void PressAnyKeyToContinue();
    void DisplayHeader(string title);
    void DisplaySeparator();
    void DisplayError(string error);
    void DisplaySuccess(string message);
    void DisplayWarning(string warning);
}