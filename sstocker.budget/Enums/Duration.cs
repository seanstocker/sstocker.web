using System;

namespace sstocker.budget.Enums
{
    public enum Duration { Monthly, Weekly, Daily };

    public static class DuractionHelper
    {
        public static DateTime GetStartDate(this Duration duration)
        {
            return DateTime.UtcNow.AddHours(-6).AddDays(-duration.GetDurationDuration()).Date;
        }

        public static DateTime GetEndDate(this Duration duration)
        {
            return duration.GetStartDate().AddDays(duration.GetDurationDuration());
        }

        public static long GetDurationDuration(this Duration duration)
        {
            switch (duration)
            {
                case Duration.Monthly:
                    return 30;
                case Duration.Weekly:
                    return 7;
                case Duration.Daily:
                    return 0;
                default:
                    throw new Exception($"{duration} is not a valid duration.");
            }
        }
    }
}
