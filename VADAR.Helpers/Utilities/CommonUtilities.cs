// <copyright file="CommonUtilities.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VADAR.Helpers.Const;

namespace VADAR.Helpers.Utilities
{
    /// <summary>
    /// Common utilities class.
    /// </summary>
    public static class CommonUtilities
    {
        /// <summary>
        /// GetCalendarInterval.
        /// </summary>
        /// <param name="fromDate">fromDate.</param>
        /// <param name="toDate">toDate.</param>
        /// <returns>int.</returns>
        public static string GetCalendarInterval(DateTime? fromDate, DateTime? toDate)
        {
            var minutesNumber = (toDate.Value.ToBinary() - fromDate.Value.ToBinary()) / 1000;

            if (minutesNumber <= 600000)
            {
                return "30s";
            }

            if (minutesNumber <= 36000000)
            {
                return "1m";
            }

            if (minutesNumber <= 864000000)
            {
                return "1h";
            }

            return "1d";
        }

        /// <summary>
        /// GetCulture.
        /// </summary>
        /// <param name="request">request.</param>
        /// <returns>culture.</returns>
        public static string GetCulture(HttpRequest request)
        {
            try
            {
                var lang = request != null && request.Cookies[CookieRequestCultureProvider.DefaultCookieName] != null && request.Cookies[CookieRequestCultureProvider.DefaultCookieName].Any() ? request.Cookies[CookieRequestCultureProvider.DefaultCookieName] : Constants.LanguageCodeConstants.Vietnamese;
                var culture = lang.Split('|').Any() && lang.Split('|')[0].Split('=').Count() > 1 ? lang.Split('|')[0].Split('=')[1] : Constants.LanguageCodeConstants.Vietnamese;

                return culture;
            }
            catch (Exception)
            {
                return Constants.LanguageCodeConstants.Vietnamese;
            }
        }

        /// <summary>
        /// AddCookie.
        /// </summary>
        /// <param name="culture">culture.</param>
        /// <param name="request">request.</param>
        /// <param name="response">response.</param>
        public static void UpdateCookie(string culture, HttpRequest request, HttpResponse response)
        {
            var langValue = culture == Constants.LanguageCodeConstants.Vietnamese ? "vi" : "en";
            var cookieKeys = request.Cookies.Where(c => c.Key.ToLower().Trim() == "lang").Select(c => c.Key);
            var enumerable = cookieKeys as string[] ?? cookieKeys.ToArray();
            if (enumerable.Any())
            {
                foreach (var el in enumerable)
                {
                    response.Cookies.Delete(el, new CookieOptions { Path = "/" });
                }
            }

            response.Cookies.Append("lang", langValue, new CookieOptions { Path = "/" });
        }

        /// <summary>
        /// Get Class Name For Severity.
        /// </summary>
        /// <param name="severity">severity.</param>
        /// <returns>Class Name.</returns>
        public static string GetClassNameForSeverity(string severity)
        {
            if (string.IsNullOrWhiteSpace(severity))
            {
                return "text-main";
            }

            return severity.ToLower() switch
            {
                Constants.SeverityNameConstants.Info => "text-VADARe",
                Constants.SeverityNameConstants.Low => "text-main",
                Constants.SeverityNameConstants.Medium => "text-orange",
                Constants.SeverityNameConstants.High => "text-danger",
                _ => "text-main"
            };
        }

        /// <summary>
        /// Format json string.
        /// </summary>
        /// <param name="json">json.</param>
        /// <returns>json string formated.</returns>
        public static string FormatJsonString(string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                return string.Empty;
            }

            if (json.StartsWith("["))
            {
                // Hack to get around issue with the older Newtonsoft library
                // not handling a JSON array that contains no outer element.
                json = "{\"list\":" + json + "}";
                var formattedText = JObject.Parse(json).ToString(Formatting.Indented);
                formattedText = formattedText.Substring(13, formattedText.Length - 14).Replace("\n  ", "\n");
                return formattedText;
            }

            return JObject.Parse(json).ToString(Formatting.Indented);
        }

        /// <summary>
        /// Gets a random invoice number to be used with a sample request that requires an invoice number.
        /// </summary>
        /// <returns>A random invoice number in the range of 0 to 999999.</returns>
        public static string GetRandomInvoiceNumber()
        {
            return new Random().Next(999999).ToString();
        }

        /// <summary>
        /// Get all days.
        /// </summary>
        /// <param name="startDate">Start date time.</param>
        /// <param name="endDate">End date time.</param>
        /// <returns>All days.</returns>
        public static List<string> GetAllDays(DateTime startDate, DateTime endDate)
        {
            return Enumerable.Range(0, 1 + endDate.Subtract(startDate).Days)
                .Select(offset => startDate.AddDays(offset).ToString("M/d/yyyy")).ToList();
        }

        /// <summary>
        /// Get all months.
        /// </summary>
        /// <param name="startDate">Start date time.</param>
        /// <param name="endDate">End date time.</param>
        /// <returns>All months.</returns>
        public static List<string> GetAllMonths(DateTime startDate, DateTime endDate)
        {
            var result = Enumerable.Range(0, ((endDate.Year - startDate.Year) * 12) + endDate.Month - startDate.Month + 1)
                .Select(m => new DateTime(startDate.Year, startDate.Month, 1).AddMonths(m));

            return result.Select(el => $"{el.Month}/{el.Year}").ToList();
        }

        /// <summary>
        /// Get all quarter.
        /// </summary>
        /// <param name="startDate">Start date time.</param>
        /// <param name="endDate">End date time.</param>
        /// <returns>All quarter name.</returns>
        public static List<string> GetAllQuarter(DateTime startDate, DateTime endDate)
        {
            var subResult = Enumerable.Range(0, ((endDate.Year - startDate.Year) * 12) + endDate.Month - startDate.Month + 1)
             .Select(m => "Q" + GetQuarterName(new DateTime(startDate.Year, startDate.Month, 1).AddMonths(m)).ToString() + "/" + new DateTime(startDate.Year, startDate.Month, 1)
             .AddMonths(m).Year.ToString()).Distinct();

            return subResult.Select(el => $"{el}").ToList();
        }

        /// <summary>
        /// Get all weeks.
        /// </summary>
        /// <param name="startDate">Start date time.</param>
        /// <param name="endDate">End date time.</param>
        /// <returns>All Weeks.</returns>
        public static List<string> GetAllWeeks(DateTime startDate, DateTime endDate)
        {
            var weekDayNames = new[] { "Monday" }; // new[] { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };
            var days = weekDayNames
                .Select(s => (DayOfWeek)Enum.Parse(typeof(DayOfWeek), s))
                .ToArray();

            // For week
            var result = Enumerable.Range(0, (endDate - startDate).Days + 1)
                .Select(d => startDate.AddDays(d))
                .Where(dt => days.Contains(dt.DayOfWeek));

            return (from el in result let yearName = el.AddDays(7).Year.ToString() select $"W{GetIso8601WeekOfYear(el)}/{yearName}").ToList();
        }

        /// <summary>
        /// Get all years.
        /// </summary>
        /// <param name="startDate">Start date time.</param>
        /// <param name="endDate">End date time.</param>
        /// <returns>All years.</returns>
        public static List<string> GetAllYears(DateTime startDate, DateTime endDate)
        {
            var result = Enumerable.Range(0, endDate.Year - startDate.Year + 1)
         .Select(y => new DateTime(startDate.Year, startDate.Month, 1).AddYears(y));

            return result.Select(el => $"{el.Year}").ToList();
        }

        /// <summary>
        /// Get Iso 8601 week of year.
        /// This presumes that weeks start with Monday.
        /// Week 1 is the 1st week of the year with a Thursday in it.
        /// </summary>
        /// <param name="time">date time.</param>
        /// <returns>Iso 8601 week of year.</returns>
        public static int GetIso8601WeekOfYear(DateTime time)
        {
            // Seriously cheat.  If its Monday, Tuesday or Wednesday, then it'll
            // be the same week# as whatever Thursday, Friday or Saturday are,
            // and we always get those right
            var day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(time);
            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
            {
                time = time.AddDays(3);
            }

            // Return the week of our adjusted day
            return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }

        /// <summary>
        /// Get quater name.
        /// </summary>
        /// <param name="myDate">date time.</param>
        /// <returns>quater name.</returns>
        public static int GetQuarterName(DateTime myDate)
        {
            return (int)Math.Ceiling(myDate.Month / 3.0);
        }

        /// <summary>
        /// Get string time ago.
        /// </summary>
        /// <param name="yourDate">Date.</param>
        /// <returns>String time ago.</returns>
        public static string StringTimeAgo(DateTime yourDate)
        {
            const int second = 1;
            const int minute = 60 * second;
            const int hour = 60 * minute;
            const int day = 24 * hour;
            const int month = 30 * day;

            var ts = new TimeSpan(DateTime.UtcNow.Ticks - yourDate.Ticks);
            var delta = Math.Abs(ts.TotalSeconds);

            if (delta < 1 * minute)
            {
                return ts.Seconds == 1 ? "one second ago" : ts.Seconds + " seconds ago";
            }

            if (delta < 2 * minute)
            {
                return "a minute ago";
            }

            if (delta < 45 * minute)
            {
                return ts.Minutes + " minutes ago";
            }

            if (delta < 90 * minute)
            {
                return "an hour ago";
            }

            if (delta < 24 * hour)
            {
                return ts.Hours + " hours ago";
            }

            if (delta < 48 * hour)
            {
                return "yesterday";
            }

            if (delta < 30 * day)
            {
                return ts.Days + " days ago";
            }

            if (delta < 12 * month)
            {
                var months = Convert.ToInt32(Math.Floor((double)ts.Days / 30));
                return months <= 1 ? "one month ago" : months + " months ago";
            }

            var years = Convert.ToInt32(Math.Floor((double)ts.Days / 365));
            return years <= 1 ? "one year ago" : years + " years ago";
        }
    }
}
