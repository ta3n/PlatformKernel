namespace SharedKernel.AppShared.Extensions;

public static class TimeSpanExtension
{
    public static string ToHourMinuteString(
        this TimeSpan timeSpan
    )
    {
        return $"{timeSpan.Hours:D2}:{timeSpan.Minutes:D2}";
    }
}
