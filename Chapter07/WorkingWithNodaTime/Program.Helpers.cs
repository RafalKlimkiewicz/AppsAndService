partial class Program
{
    private static void SectionTitle(string title)
    {
        ConsoleColor prevColor = ForegroundColor;

        ForegroundColor = ConsoleColor.DarkYellow;
        WriteLine($"*** {title}");
        ForegroundColor = prevColor;
    }
}
