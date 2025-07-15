using System;

public sealed class TimeUtils
{

    /// <summary>
    ///获取当前时间截(int, 精确到s)
    /// </summary>
    /// <returns></returns>
    public static int GetCurrTimestampS()
    {
        return (int)(GetCurrTimestampMs() * 0.001);
    }

    /// <summary>
    ///获取当前时间截(long, 精确到ms)
    /// </summary>
    /// <returns></returns>
    public static long GetCurrTimestampMs()
    {
        TimeSpan ts = DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1);
        return (long)ts.TotalMilliseconds;     //精确到毫秒
    }
    /// <summary>
    /// 获取时间戳(秒)
    /// </summary>
    /// <returns></returns>
    public static int GetTimestampS(int year,int month,int day,int hour,int min,int sec)
    {
        return (int)(GetTimestampMs(year, month, day, hour, min, sec) * 0.001);
    }
    /// <summary>
    /// 获取时间戳(毫秒)
    /// </summary>
    /// <returns></returns>
    public static long GetTimestampMs(int year, int month, int day, int hour, int min, int sec)
    {
        DateTime time = new DateTime(year, month, day, hour, min, sec);
        TimeSpan ts = time.ToUniversalTime() - new DateTime(1970, 1, 1);
        return (long)ts.TotalMilliseconds;     //精确到毫秒
    }
    public static int GetTimestampS(DateTime time)
    {
        return (int)(GetTimestampMs(time) * 0.001);
    }

    public static long GetTimestampMs(DateTime time)
    {
        TimeSpan ts = time.ToUniversalTime() - new DateTime(1970, 1, 1);
        return (long)ts.TotalMilliseconds;     //精确到毫秒
    }
    /// <summary>
    /// 时间戳转换为本地时间对象
    /// </summary>
    /// <returns></returns>      
    public static DateTime GetUnixDateTime(long unix)
    {
        //long unix = 1500863191;
        DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
        DateTime newTime = dtStart.AddSeconds(unix);
        return newTime;

    }
    /// <summary>
    /// 获取当天某点时间(默认0点)
    /// </summary>
    /// <returns></returns>
    public static DateTime GetTodayTime(int hour = 0,int min = 0,int sec = 0)
    {
        DateTime today = DateTime.Today;
        today.AddHours(hour);
        today.AddMinutes(min);
        today.AddSeconds(sec);
        return today;
    }
    /// <summary>
    /// 获取指定日期0点时间戳
    /// </summary>
    /// <param name="dateTime"></param>
    /// <returns></returns>
    public static int GetDayStartTimestampS(DateTime dateTime)
    {
        return GetTimestampS(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0);
    }
    /// <summary>
    /// 获取第x天时间(0点)
    /// </summary>
    /// <param name="next"></param>
    /// <returns></returns>
    public static DateTime GetNextDayTime(int next = 1)
    {
        DateTime now = GetTodayTime();
        return now.AddDays(next);
    }

    /// <summary>
    ///  获取当天某点时间戳(默认0点)
    /// </summary>
    /// <returns></returns>
    public static int GetTodayTimestamp(int hour = 0,int min = 0,int sec = 0)
    {
        return GetTimestampS(GetTodayTime(hour, min, sec));
    }

    /// <summary>
    /// 获取第x天时间(0点)
    /// </summary>
    /// <param name="next"></param>
    /// <returns></returns>
    public static int GetNextDayTimestamp(int next = 1)
    {
        return GetTimestampS(GetNextDayTime(next));
    }

    public static int GetNextDayTimestamp(long todaystamp,int next)
    {
        DateTime time = GetUnixDateTime(todaystamp);
        //time.AddDays(next);
        DateTime nextDay = time.AddDays(next);
        return GetDayStartTimestampS(nextDay);
    }
}

