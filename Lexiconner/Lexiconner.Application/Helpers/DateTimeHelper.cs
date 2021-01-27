using NodaTime;
using NodaTime.TimeZones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lexiconner.Application.Helpers
{
    public static class DateTimeHelper
    {
        /// <summary>
        /// Returns timezone list from TZDB
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<TzdbZoneLocation> GetTzdbZoneLocations()
        {
            return TzdbDateTimeZoneSource.Default
               .ZoneLocations
               .ToList();
        }

        public static DateTimeZone GetTimeZoneById(string timeZoneId)
        {
            // return DateTimeZoneProviders.Tzdb[timeZoneId];

            if (String.IsNullOrEmpty(timeZoneId))
            {
                return null;
            }
            var timeZone = DateTimeZoneProviders.Tzdb.GetZoneOrNull(timeZoneId);
            return timeZone;
        }

        #region Transformations

        /// <summary>
        /// Converts LocalDateTime to DateTimeZone, or to UTC if DateTimeZone is null
        /// </summary>
        /// <param name="localDateTime"></param>
        /// <param name="dateTimeZone"></param>
        /// <returns></returns>
        public static ZonedDateTime LocalDateTimeToZonedDateTimeOrUtc(LocalDateTime localDateTime, DateTimeZone dateTimeZone)
        {
            if (dateTimeZone == null)
            {
                var dateZoned = localDateTime.InUtc();
                return dateZoned;
            }
            else
            {
                var dateZoned = localDateTime.InZoneLeniently(dateTimeZone);
                return dateZoned;
            }
        }

        /// <summary>
        /// Note: the DateTime here must have a "Kind" of Utc.
        /// </summary>
        public static DateTime UtcToLocal(DateTime dateTime, string timeZoneId)
        {
            Instant instant = Instant.FromDateTimeUtc(dateTime);
            IDateTimeZoneProvider timeZoneProvider = DateTimeZoneProviders.Tzdb;
            var timezone = timeZoneProvider[timeZoneId];
            var zonedDateTime = instant.InZone(timezone);
            return zonedDateTime.ToDateTimeUnspecified();
        }

        /// <summary>
        /// Note: the DateTime here should have a "Kind" of Unspecified
        /// </summary>
        public static DateTime LocalToUtc(DateTime dateTime, string timeZoneId)
        {
            LocalDateTime localDateTime = LocalDateTime.FromDateTime(dateTime);
            IDateTimeZoneProvider timeZoneProvider = DateTimeZoneProviders.Tzdb;
            var timezone = timeZoneProvider[timeZoneId];
            var zonedDbDateTime = timezone.AtLeniently(localDateTime);
            return zonedDbDateTime.ToDateTimeUtc();
        }

        /// <summary>
        /// Note: the DateTime here should have a "Kind" of Unspecified
        /// </summary>
        public static DateTimeOffset LocalToUtcOffset(DateTime dateTime, string timeZoneId)
        {
            LocalDateTime localDateTime = LocalDateTime.FromDateTime(dateTime);
            IDateTimeZoneProvider timeZoneProvider = DateTimeZoneProviders.Tzdb;
            var timezone = timeZoneProvider[timeZoneId];
            var zonedDbDateTime = timezone.AtLeniently(localDateTime);
            return zonedDbDateTime.ToDateTimeOffset();
        }

        #endregion
    }
}
