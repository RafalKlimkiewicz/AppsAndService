using System.Collections.ObjectModel;
using System.Globalization;

partial class Program
{
    private static void ConfigureConsole(string culture = "en-US", bool overrideComputerCulture = true)
    {
        OutputEncoding = System.Text.Encoding.UTF8;

        Thread t = Thread.CurrentThread;

        if (overrideComputerCulture)
        {
            t.CurrentCulture = System.Globalization.CultureInfo.GetCultureInfo(culture);
            t.CurrentUICulture = t.CurrentCulture;
        }

        var ci = t.CurrentCulture;

        WriteLine($"Current culture: {ci.DisplayName}");
        WriteLine($"Short date pattern: {ci.DateTimeFormat.ShortDatePattern}");
        WriteLine($"Long date pattern: {ci.DateTimeFormat.LongDatePattern}");
        WriteLine();
    }

    private static void SectionTitle(string title)
    {
        ConsoleColor previousColor = ForegroundColor;
        ForegroundColor = ConsoleColor.DarkYellow;
        WriteLine($"*** {title}");
        ForegroundColor = previousColor;
    }

    private static void OutputTimeZones()
    {
        var zones = TimeZoneInfo.GetSystemTimeZones();

        WriteLine($"*** {zones.Count} time zones:");

        foreach (var zone in zones)
        {
            WriteLine($"{zone.Id}");
        }
    }

    private static void OutputDateTime(DateTime dateTime, string title)
    {
        SectionTitle(title);
        WriteLine($"Value: {dateTime}");
        WriteLine($"Kind: {dateTime.Kind}");
        WriteLine($"IsDaylightSavingTime: {dateTime.IsDaylightSavingTime()}");
        WriteLine($"ToLocalTime(): {dateTime.ToLocalTime()}");
        WriteLine($"ToUniversalTime(): {dateTime.ToUniversalTime()}");
    }

    private static void OutputTimeZone(TimeZoneInfo zone, string title)
    {
        SectionTitle(title);
        WriteLine($"Id: {zone.Id}");
        WriteLine($"IsDaylightSavingTime(DateTime.Now): {zone.IsDaylightSavingTime(DateTime.Now)}");
        WriteLine($"StandardName: {zone.StandardName}");
        WriteLine($"DaylightName: {zone.DaylightName}");
        WriteLine($"BaseUtcOffset: {zone.BaseUtcOffset}");
    }

    private static string GetCurrentZoneName(TimeZoneInfo zone, DateTime when)
    {
        // time zone names change if Daylight Saving time is active
        // e.g. GMT Standard Time becomes GMT Summer Time
        return zone.IsDaylightSavingTime(when) ?
          zone.DaylightName : zone.StandardName;
    }

    private static void OutputCultures(string title)
    {
        ConsoleColor previousColor = ForegroundColor;
        ForegroundColor = ConsoleColor.DarkYellow;

        WriteLine($"*** {title}");

        // Get the cultures from the current thread.
        CultureInfo globalization = CultureInfo.CurrentCulture;
        CultureInfo localization = CultureInfo.CurrentUICulture;

        WriteLine($"The current globalization culture is {globalization.Name}: {globalization.DisplayName}");

        WriteLine($"The current localization culture is {localization.Name}: {localization.DisplayName}");

        WriteLine($"Days of the week: {string.Join(", ", globalization.DateTimeFormat.DayNames)}");

        WriteLine($"Months of the year: {string.Join(", ", globalization.DateTimeFormat.MonthNames
          // Some have 13 months; most 12, and the last is empty.
          .TakeWhile(month => !string.IsNullOrEmpty(month)))}");

        WriteLine($"1st day of this year: {new DateTime(year: DateTime.Today.Year, month: 1, day: 1)
          .ToString("D", globalization)}");

        WriteLine($"Number group separator: {globalization.NumberFormat.NumberGroupSeparator}");

        WriteLine($"Number decimal separator: {globalization.NumberFormat.NumberDecimalSeparator}");

        RegionInfo region = new(globalization.LCID);

        WriteLine($"Currency symbol: {region.CurrencySymbol}");

        WriteLine($"Currency name: {region.CurrencyNativeName} ({region.CurrencyEnglishName})");

        WriteLine($"IsMetric: {region.IsMetric}");

        WriteLine();

        ForegroundColor = previousColor;
    }
}
