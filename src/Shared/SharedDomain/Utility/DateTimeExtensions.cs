namespace SharedDomain;

public static class DateTimeExtensions
{
    public static DateTime DkNow => TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time"));
}
