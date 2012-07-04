namespace System
{
	internal class TimeZoneInfo
	{
	   internal static TimeZoneInfo Local = null;

      internal static DateTime ConvertTime(DateTime dateTime, TimeZoneInfo destinationTimeZone)
      {
         // TODO: fix it for .net 2.0
         return dateTime;
      }
	}
}
