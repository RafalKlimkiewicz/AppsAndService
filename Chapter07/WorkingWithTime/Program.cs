using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using IHost host = Host.CreateDefaultBuilder(args)
  .ConfigureServices(services =>
  {
      services.AddLocalization(options =>
      {
          options.ResourcesPath = "Resources";
      });

      services.AddTransient<PacktResources>();
  })
  .Build();

ConfigureConsole("pl-PL");

//SectionTitle("Specifyng date and time values");

//WriteLine($"MinValue: {DateTime.MinValue}");
//WriteLine($"MaxValue: {DateTime.MaxValue}");
//WriteLine($"UnixEpoch: {DateTime.UnixEpoch}");
//WriteLine($"Now: {DateTime.Now}");
//WriteLine($"Today: {DateTime.Today}");
//WriteLine($"Today:d: {DateTime.Today:d}");
//WriteLine($"Today:D: {DateTime.Today:D}");
//WriteLine($"Today:o: {DateTime.Today:o}");
//WriteLine($"Today:R: {DateTime.Today:R}");

//SectionTitle("Date only");

//WriteLine($"Day : {Thread.CurrentThread.CurrentCulture.NativeName}  : {DateTime.Now:dddd}");
//WriteLine($"Day : {Thread.CurrentThread.CurrentCulture.NativeName}  : {DateTimeFormatInfo.CurrentInfo.GetDayName(DateTime.Now.DayOfWeek)}");
//WriteLine($"DateOnly : {new DateOnly(2030, 11, 12)}");

//SectionTitle("Date only");

//var dtfi = DateTimeFormatInfo.CurrentInfo;
////WriteLine($"{}");
//WriteLine($"{dtfi.DateSeparator}");
//WriteLine($"{dtfi.TimeSeparator}");

//WriteLine($"{dtfi.LongDatePattern}");
//WriteLine($"{dtfi.ShortDatePattern}");

//WriteLine($"{dtfi.LongTimePattern}");
//WriteLine($"{dtfi.ShortTimePattern}");

//Write("Days Names:");
//for (int i = 0; i < dtfi.DayNames.Length - 1; i++)
//{
//    Write($"     {dtfi.GetDayName((DayOfWeek) i)}");
//}

//WriteLine();
//Write("Days Names:");

//for (int i = 1; i < dtfi.MonthNames.Length; i++)
//{
//    Write($"     {dtfi.GetMonthName(i)}");
//}


//////////////////////////////////////////////////////////////////////////////////
//////////////////// DATE TIME AND TIME ZONES ////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////////
#region DatTime And TimeZones
OutputTimeZones();

OutputDateTime(DateTime.Now, "DateTime.Now");
OutputDateTime(DateTime.UtcNow, "DateTime.UtcNow");

OutputTimeZone(TimeZoneInfo.Local, "TimeZoneInfo.Local");
OutputTimeZone(TimeZoneInfo.Utc, "TimeZoneInfo.Utc");

Write("Enter a time zone or press Enter for US East Coast: ");
string zoneId = ReadLine()!;

if (string.IsNullOrEmpty(zoneId))
{
    zoneId = "Eastern Standard Time";
}

try
{
    var otherZone = TimeZoneInfo.FindSystemTimeZoneById(zoneId);

    OutputTimeZone(otherZone, $"TimeZoneInfo.FindSystemTimeZoneById(\"{zoneId}\")");

    SectionTitle($"What's the time in {zoneId}?");

    Write("Enter a local time or press Enter for now: ");

    string? timeText = ReadLine();
    DateTime localTime;

    if ((string.IsNullOrEmpty(timeText)) || (!DateTime.TryParse(timeText, out localTime)))
    {
        localTime = DateTime.Now;
    }

    DateTime otherZoneTime = TimeZoneInfo.ConvertTime(dateTime: localTime, sourceTimeZone: TimeZoneInfo.Local, destinationTimeZone: otherZone);

    WriteLine($"{localTime} {GetCurrentZoneName(TimeZoneInfo.Local, localTime)} is {otherZoneTime} {GetCurrentZoneName(otherZone, otherZoneTime)}.");
}
catch (TimeZoneNotFoundException)
{
    WriteLine($"The {zoneId} zone cannot be found on the local system.");
}
catch (InvalidTimeZoneException)
{
    WriteLine($"The {zoneId} zone contains invalid or missing data.");
}
catch (System.Security.SecurityException)
{
    WriteLine("The application does not have permission to read time zone information.");
}
catch (OutOfMemoryException)
{
    WriteLine($"Not enough memory is available to load information on the {zoneId} zone.");
}

#endregion

//////////////////////////////////////////////////////////////////////////////////
//////////////////// DATE TIME AND TIME ZONES ////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////////



OutputEncoding = System.Text.Encoding.UTF8;

//OutputCultures("Current culture");


WriteLine("Example ISO culture codes:");

string[] cultureCodes = {
  "da-DK", "en-GB", "en-US", "fa-IR",
  "fr-CA", "fr-FR", "he-IL", "pl-PL", "sl-SI" };

foreach (string code in cultureCodes)
{
    CultureInfo culture = CultureInfo.GetCultureInfo(code);
    WriteLine($"  {culture.Name}: {culture.EnglishName} / {culture.NativeName}");
}


WriteLine();

Write("Enter an ISO culture code: ");
string? cultureCode = ReadLine();

if (string.IsNullOrWhiteSpace(cultureCode))
{
    cultureCode = "en-US";
}

CultureInfo ci;

try
{
    ci = CultureInfo.GetCultureInfo(cultureCode);
}
catch (CultureNotFoundException)
{
    WriteLine($"Culture code not found: {cultureCode}");
    WriteLine("Exiting the app.");
    return;
}

// change the current cultures on the thread
CultureInfo.CurrentCulture = ci;
CultureInfo.CurrentUICulture = ci;

//OutputCultures("After changing the current culture");

PacktResources resources = host.Services.GetRequiredService<PacktResources>();

Write(resources.GetEnterYourNamePrompt());

string? name = ReadLine();
if (string.IsNullOrWhiteSpace(name))
{
    name = "Bob";
}

Write(resources.GetEnterYourDobPrompt());
string? dobText = ReadLine();

if (string.IsNullOrWhiteSpace(dobText))
{
    // If they do not enter a DOB then use
    // sensible defaults for their culture.
    dobText = ci.Name switch
    {
        "en-US" or "fr-CA" => "1/27/1990",
        "da-DK" or "fr-FR" or "pl-PL" => "27/1/1990",
        "fa-IR" => "1990/1/27",
        _ => "1/27/1990"
    };
}

Write(resources.GetEnterYourSalaryPrompt());
string? salaryText = ReadLine();

if (string.IsNullOrWhiteSpace(salaryText))
{
    salaryText = "34500";
}

DateTime dob = DateTime.Parse(dobText);
int minutes = (int) DateTime.Today.Subtract(dob).TotalMinutes;
decimal salary = decimal.Parse(salaryText);

// WriteLine($"{name} was born on a {dob:dddd}. {name} is {
//   minutes:N0} minutes old. {name} earns {salary:C}.");

WriteLine(resources.GetPersonDetails(name, dob, minutes, salary));
