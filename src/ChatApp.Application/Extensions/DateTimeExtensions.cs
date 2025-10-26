﻿namespace ChatApp.Application.Extensions;

public static class DateTimeExtensions
{
    public static DateTime FirstDayOfThisMonth(this DateTime date)
    {
        return new DateTime(date.Year, date.Month, 1);
    }

    public static DateTime LastDayOfThisMonth(this DateTime date)
    {
        return new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));
    }

    public static bool IsSameDay(this DateTime date, DateTime other)
    {
        return date.Year == other.Year && date.Month == other.Month && date.Day == other.Day;
    }
}
