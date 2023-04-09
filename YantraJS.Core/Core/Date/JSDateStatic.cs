using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using YantraJS.Core.Clr;
using YantraJS.Utils;

namespace YantraJS.Core
{
    internal static class JSDateStatic
    {


        internal static JSDate AsJSDate(this JSValue v,
                [CallerMemberName] string helper = null)
        {
            if (!(v is JSDate date))
                throw JSContext.Current.NewTypeError($"Date.prototype.{helper} called on non date");
            return date;
        }

        /// <summary>
        /// Converts a .NET date into a javascript date.
        /// </summary>
        /// <param name="dateTime"> The .NET date. </param>
        /// <returns> The number of milliseconds since January 1, 1970, 00:00:00 UTC </returns>
        internal static double ToJSDate(this DateTimeOffset dateTime)
        {
            if (dateTime == JSDate.InvalidDate)
                return double.NaN;
            // The spec requires that the time value is an integer.
            // We could round to nearest, but then date.toUTCString() would be different from Date(date.getTime()).toUTCString().
            //switch(dateTime.Kind)
            //{
            //    case DateTimeKind.Local:
            //        dateTime = dateTime.ToUniversalTime();
            //        break;
            //    case DateTimeKind.Unspecified:
            //        dateTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
            //        break;
            //}
            //var diff = dateTime.Ticks - epoch;
            return dateTime.ToUniversalTime().ToUnixTimeMilliseconds();
            //return Math.Floor((double)(diff / 10000));
        }

    }


    partial class JSDate { 
        public static long epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks;

        [JSExport("UTC")]
        internal static JSValue UTC(in Arguments a)
        {
            var (year, month, day, hour, minute, second, millisecond) = a.Get7Int();
            var val =  ToDateTime(year, month, day, hour, minute, second, millisecond, TimeSpan.Zero).ToJSDate();
            return new JSNumber(val);

        }

        [JSExport("now")]
        internal static JSValue Now(in Arguments a)
        {
            var result = DateTimeOffset.Now.ToJSDate();
            return new JSNumber(result);
        }

        /// <summary>
        /// Jint - private JsValue Parse(JsValue thisObj, JsValue[] arguments), but we changed 
        ///  DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal to DateTimeStyles.AssumeLocal
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        [JSExport("parse")]
        internal static JSValue Parse(in Arguments a)
        {
            var text = a.Get1().ToString();
            double val;
            //if (DateTime.TryParse(text, out var result)) {
            //    val = ToJSDate(result);
            //    return new JSNumber(val);
            //}
            //result = DateParser.Parse(text);
            //val = ToJSDate(result);
            //return new JSNumber(val);

            val = DateParser.Parse(text).ToJSDate();
            return new JSNumber(val);
        }

        //[Prototype("getYear")]
        //internal static JSValue GetYear(in Arguments a)
        //{
        //    if (!(a.This is JSDate d))
        //        throw JSContext.Current.NewTypeError("Method Date.prototype.getYear called on incompatible receiver");
        //    return new JSNumber(d.value.Year - 2000);
        //}


        /// <summary>
        /// Given the components of a date, returns the equivalent .NET date.
        /// </summary>
        /// <param name="year"> The full year. </param>
        /// <param name="month"> The month as an integer between 0 and 11 (january to december). </param>
        /// <param name="day"> The day of the month, from 1 to 31.  Defaults to 1. </param>
        /// <param name="hour"> The number of hours since midnight, from 0 to 23.  Defaults to 0. </param>
        /// <param name="minute"> The number of minutes, from 0 to 59.  Defaults to 0. </param>
        /// <param name="second"> The number of seconds, from 0 to 59.  Defaults to 0. </param>
        /// <param name="millisecond"> The number of milliseconds, from 0 to 999.  Defaults to 0. </param>
        /// <param name="kind"> Indicates whether the components are in UTC or local time. </param>
        /// <returns> The equivalent .NET date. </returns>
        internal static DateTimeOffset ToDateTime(
            int year, int month, int day, 
            int hour, int minute, int second, int millisecond,
            TimeSpan offset)
        {
            // DateTime doesn't support years below year 1.
            if (year < 0)
                return JSDate.InvalidDate;

            // This step was missing from Jurrasic, add 1900 to year < 2000 to get full year. 
            if (0 <= year && year <= 99)
            {
                year += 1900;
            }

            // var offset = TimeZoneInfo.Local.BaseUtcOffset;

            if (month >= 0 && month < 12 &&
                day >= 1 && day <= DateTime.DaysInMonth(year, month + 1) &&
                hour >= 0 && hour < 24 &&
                minute >= 0 && minute < 60 &&
                second >= 0 && second < 60 &&
                millisecond >= 0 && millisecond < 1000)
            {
                // All parameters are in range.
                return new DateTimeOffset(year, month + 1, day, hour, minute, second, millisecond,offset);
            }
            else
            {
                // One or more parameters are out of range.
                try
                {
                    DateTimeOffset value = new DateTimeOffset(year, 1, 1, 0, 0, 0, offset);
                    value = value.AddMonths(month);
                    if (day != 1)
                        value = value.AddDays(day - 1);
                    if (hour != 0)
                        value = value.AddHours(hour);
                    if (minute != 0)
                        value = value.AddMinutes(minute);
                    if (second != 0)
                        value = value.AddSeconds(second);
                    if (millisecond != 0)
                        value = value.AddMilliseconds(millisecond);
                    return value;
                }
                catch (ArgumentOutOfRangeException)
                {
                    // One or more of the parameters was NaN or way too big or way too small.
                    // Return a sentinel invalid date.
                    return JSDate.InvalidDate;
                }
            }
        }


    }
}
