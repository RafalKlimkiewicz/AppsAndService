using NodaTime;

SectionTitle("Converting Noda Time types");

var now = SystemClock.Instance.GetCurrentInstant();

WriteLine($"Now (Instant): {now}");
WriteLine();

var nowInUts = now.InUtc();

WriteLine($"Now (DateTimeZone): {nowInUts.Zone}");
WriteLine($"Now (ZonedDateTime): {nowInUts}");
WriteLine($"Now (DST): {nowInUts.IsDaylightSavingTime()}");
WriteLine();

DateTimeZone zonePT = DateTimeZoneProviders.Tzdb["US/Pacific"];
ZonedDateTime newInPT = now.InZone(zonePT);

WriteLine($"Now (DateTimeZone): {nowInUts.Zone}");
WriteLine($"Now (ZonedDateTime): {nowInUts}");
WriteLine($"Now (DST): {nowInUts.IsDaylightSavingTime()}");
WriteLine();

DateTimeZone zoneUK = DateTimeZoneProviders.Tzdb["Europe/London"];
ZonedDateTime nowInUK = now.InZone(zoneUK);

WriteLine($"Now (DateTimeZone): {nowInUK.Zone}");
WriteLine($"Now (ZonedDateTime): {nowInUK}");
WriteLine($"Now (DST): {nowInUK.IsDaylightSavingTime()}");
WriteLine();


var nowInLocal = nowInUts.LocalDateTime;

WriteLine($"Now (LocalDateTime): {nowInLocal}");
WriteLine($"Now (LocalDate): {nowInLocal.Date}");
WriteLine($"Now (LocalTime): {nowInLocal.TimeOfDay}");
WriteLine();


SectionTitle("Working with periods");

// The modern .NET era began with the release of .NET Core 1.0
// on June 27, 2016 at 10am Pacific Time, or 5pm UTC.
LocalDateTime start = new(year: 2016, month: 6, day: 27, hour: 17, minute: 0, second: 0);
LocalDateTime end = LocalDateTime.FromDateTime(DateTime.UtcNow);

WriteLine("Modern .NET era");
WriteLine($"Start: {start}");
WriteLine($"End: {end}");
WriteLine();

var period = Period.Between(start, end);

WriteLine($"Period: {period}");
WriteLine($"Years: {period.Years}");
WriteLine($"Months: {period.Months}");
WriteLine($"Weeks: {period.Weeks}");
WriteLine($"Days: {period.Days}");
WriteLine($"Hours: {period.Hours}");
WriteLine();

var p1 = Period.FromWeeks(2);
var p2 = Period.FromDays(14);

WriteLine($"p1 (period of two weeks): {p1}");
WriteLine($"p1 (period of two weeks): {p1.Normalize()}");
WriteLine($"p2 (period of 14 days): {p2}");
WriteLine($"p2 (period of 14 days): {p2.Normalize()}");
WriteLine($"p1 == p2: {p1 == p2}");
WriteLine($"p1.Normalize() == p2: {p1.Normalize() == p2}");

WriteLine();
WriteLine(TimeZoneInfo.Local);
WriteLine();
WriteLine();
