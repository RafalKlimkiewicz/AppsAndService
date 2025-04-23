namespace Northwind.Background.Hangfire;

public static class JobHandlers
{
    public static void WriteMessage(string? message)
    {
        var prev = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"[Hangfire] {DateTime.Now:O}: {message}");
        Console.ForegroundColor = prev;
    }
}
