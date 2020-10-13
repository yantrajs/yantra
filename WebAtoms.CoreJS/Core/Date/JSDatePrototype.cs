using System;
using System.Collections.Generic;
using System.Text;
using WebAtoms.CoreJS.Utils;

namespace WebAtoms.CoreJS.Core.Date
{
    public static class JSDatePrototype
    {

        [Constructor( Length = 7 )]
        internal static JSValue Constructor(in Arguments a) {
            DateTimeOffset date;
            if (a.Length == 0)
            {
                return new JSDate(DateTimeOffset.Now);
            }
            if (a.Length == 1) {
                var dateString = a.Get1();
                if (dateString.IsNumber) {
                    date = DateTimeOffset.FromUnixTimeMilliseconds(dateString.BigIntValue);
                    //var utc = dateString.BigIntValue * 10000;
                    //utc += JSDateStatic.epoch;
                    //date = new DateTime(utc, DateTimeKind.Utc);
                    return new JSDate(date.ToLocalTime());
                }
                date = DateParser.Parse(dateString.ToString());
                if (date == DateTimeOffset.MinValue)
                    return JSDate.invalidDate;
                return new JSDate(date.ToLocalTime());
            }
            throw new NotImplementedException();
        }



        [Prototype("getDate", Length = 0)]
        internal static JSValue GetDate(in Arguments a)
        {
            var @this = a.This.AsJSDate();
            if (@this.value == DateTimeOffset.MinValue)
                return JSNumber.NaN;
            var result = @this.value.Day;
            return new JSNumber(result);
        }

        /// <summary>
        /// If invalid date, return false
        /// If diff is undefined or NaN, return false
        /// else return true
        /// </summary>
        /// <param name="this"></param>
        /// <param name="diff"></param>
        /// <param name="diffValue"></param>
        /// <returns></returns>
        internal static bool IsValid(JSDate @this, JSValue diff, out double diffValue)
        {
            diffValue = 0;
            if (@this.value == DateTimeOffset.MinValue)
                return false;

            if (diff.IsUndefined)
            {
                @this.value = DateTimeOffset.MinValue;
                return false;
            }

            diffValue = diff.DoubleValue;
            if (double.IsNaN(diffValue))
            {
                @this.value = DateTimeOffset.MinValue;
                return false;
            }
            return true;
        }

        /// <summary>
        /// The setDate() method sets the day of the Date object relative to the beginning of the currently set month.
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        [Prototype("setDate", Length = 1)]
        internal static JSValue SetDate(in Arguments a)
        {
            var @this = a.This.AsJSDate();
            if(!IsValid(@this, a.Get1(),out var diffValue))
                return JSNumber.NaN;


            @this.value = @this.value.AddDays(-@this.value.Day + diffValue);

            return new JSNumber(@this.value.ToJSDate());
        }

        [Prototype("getDay", Length = 0)]
        internal static JSValue GetDay(in Arguments a)
        {
            var @this = a.This.AsJSDate();
            if (@this.value == DateTimeOffset.MinValue)
                return JSNumber.NaN;
            var result = @this.value.DayOfWeek;
            return new JSNumber((double)result);
        }

        [Prototype("getFullYear", Length = 0)]
        internal static JSValue GetFullYear(in Arguments a)
        {
            var @this = a.This.AsJSDate();
            if (@this.value == DateTimeOffset.MinValue)
                return JSNumber.NaN;
            var result = @this.value.Year;
            return new JSNumber(result);
        }

        [Prototype("getHours", Length = 0)]
        internal static JSValue GetHours(in Arguments a)
        {
            var @this = a.This.AsJSDate();
            if (@this.value == DateTimeOffset.MinValue)
                return JSNumber.NaN;
            var result = @this.value.Hour;
            return new JSNumber(result);
        }



        [Prototype("getMilliseconds", Length = 0)]
        internal static JSValue GetMilliSeconds(in Arguments a)
        {
            var @this = a.This.AsJSDate();
            if (@this.value == DateTimeOffset.MinValue)
                return JSNumber.NaN;
            var result = @this.value.Millisecond;
            return new JSNumber(result);
        }



        [Prototype("getMinutes", Length = 0)]
        internal static JSValue GetMinutes(in Arguments a)
        {
            var @this = a.This.AsJSDate();
            if (@this.value == DateTimeOffset.MinValue)
                return JSNumber.NaN;
            var result = @this.value.Minute;
            return new JSNumber(result);
        }

        [Prototype("getMonth", Length = 0)]
        internal static JSValue GetMonth(in Arguments a)
        {
            var @this = a.This.AsJSDate();
            if (@this.value == DateTimeOffset.MinValue)
                return JSNumber.NaN;
            var result = @this.value.Month - 1;
            return new JSNumber(result);
        }


        [Prototype("getSeconds", Length = 0)]
        internal static JSValue GetSeconds(in Arguments a)
        {
            var @this = a.This.AsJSDate();
            if (@this.value == DateTimeOffset.MinValue)
                return JSNumber.NaN;
            var result = @this.value.Second;
            return new JSNumber(result);
        }

        [Prototype("getTime", Length = 0)]
        internal static JSValue GetTime(in Arguments a)
        {
            var @this = a.This.AsJSDate();
            if (@this.value == DateTimeOffset.MinValue)
                return JSNumber.NaN;
            var result = @this.value.ToJSDate();
            return new JSNumber(result);
        }

        [Prototype("getTimezoneOffset", Length = 0)]
        internal static JSValue GetTimezoneOffset(in Arguments a)
        {
            var @this = a.This.AsJSDate();
            if (@this.value == DateTimeOffset.MinValue)
                return JSNumber.NaN;
            var result = -(int)TimeZoneInfo.Local.GetUtcOffset(@this.Value).TotalMinutes;
            return new JSNumber(result);
        }

        [Prototype("getUTCDate", Length = 0)]
        internal static JSValue GetUTCDate(in Arguments a)
        {
            var @this = a.This.AsJSDate();
            if (@this.value == DateTimeOffset.MinValue)
                return JSNumber.NaN;
            var result = @this.value.ToUniversalTime().Day;
            return new JSNumber(result);
        }

        [Prototype("getUTCDay", Length = 0)]
        internal static JSValue GetUTCDay(in Arguments a)
        {
            var @this = a.This.AsJSDate();
            if (@this.value == DateTimeOffset.MinValue)
                return JSNumber.NaN;
            var result = @this.value.ToUniversalTime().DayOfWeek;
            return new JSNumber((double)result);
        }

        [Prototype("getUTCFullYear", Length = 0)]
        internal static JSValue GetUTCFullYear(in Arguments a)
        {
            var @this = a.This.AsJSDate();
            if (@this.value == DateTimeOffset.MinValue)
                return JSNumber.NaN;
            var result = @this.value.ToUniversalTime().Year;
            return new JSNumber(result);
        }

        [Prototype("getUTCHours", Length = 0)]
        internal static JSValue GetUTCHours(in Arguments a)
        {
            var @this = a.This.AsJSDate();
            if (@this.value == DateTimeOffset.MinValue)
                return JSNumber.NaN;
            var result = @this.value.ToUniversalTime().Hour;
            return new JSNumber(result);
        }

        [Prototype("getUTCMilliseconds", Length = 0)]
        internal static JSValue GetUTCMilliseconds(in Arguments a)
        {
            var @this = a.This.AsJSDate();
            if (@this.value == DateTimeOffset.MinValue)
                return JSNumber.NaN;
            var result = @this.value.ToUniversalTime().Millisecond;
            return new JSNumber(result);
        }

        [Prototype("getUTCMinutes", Length = 0)]
        internal static JSValue GetUTCMinutes(in Arguments a)
        {
            var @this = a.This.AsJSDate();
            if (@this.value == DateTimeOffset.MinValue)
                return JSNumber.NaN;
            var result = @this.value.ToUniversalTime().Minute;
            return new JSNumber(result);
        }

        [Prototype("getUTCMonth", Length = 0)]
        internal static JSValue GetUTCMonth(in Arguments a)
        {
            var @this = a.This.AsJSDate();
            if (@this.value == DateTimeOffset.MinValue)
                return JSNumber.NaN;
            var result = @this.value.ToUniversalTime().Month - 1;
            return new JSNumber(result);
        }

        [Prototype("getUTCSeconds", Length = 0)]
        internal static JSValue GetUTCSeconds(in Arguments a)
        {
            var @this = a.This.AsJSDate();
            if (@this.value == DateTimeOffset.MinValue)
                return JSNumber.NaN;
            var result = @this.value.ToUniversalTime().Second;
            return new JSNumber(result);
        }


        //[Prototype("getYear", Length = 0)]
        //internal static JSValue GetYear(in Arguments a)
        //{
        //    var @this = a.This.AsJSDate();
        //    if (@this.value == DateTimeOffset.MinValue)
        //        return JSNumber.NaN;
        //    var result = @this.value.Year;
        //    return new JSNumber(result);
        //}

        [Prototype("setFullYear", Length = 3)]
        internal static JSValue SetFullYear(in Arguments a)
        {
            var @this = a.This.AsJSDate();
            if (!IsValid(@this, a.Get1(), out var year))

                return JSNumber.NaN;

            if (year <= 0) {
                @this.value = DateTimeOffset.MinValue;
                return JSNumber.NaN;
            }


            var date = @this.value;

            var (_year, _month, _day) = a.Get3();

            var month = _month.IsUndefined ? date.Month : _month.IntValue + 1;
            var day = _day.IsUndefined ? date.Day : _day.IntValue;

            var extraMonths = 0;
            var extraDays = 0;


            if (month > 12)
            {
                extraMonths = month - 12;
                month = 12;
            }

            if (month < 1) {
                extraMonths = month - 1;
                month = 1;
            }

            if (day > 28) {
                extraDays = day - 28;
                day = 28;
            }

            if (day < 0)
            {
                extraDays = day - 1;
                day = 1;
            }

            if (day == 0)
            {
                extraDays = -1;
                day = 1;
            }

            @this.value = new DateTimeOffset((int)year,month,day,date.Hour,date.Minute,date.Second,date.Millisecond,@this.value.Offset);

            if (extraDays != 0) {
                @this.value = @this.value.AddDays(extraDays);
            }

            if (extraMonths != 0)
            {
                @this.value = @this.value.AddMonths(extraMonths);
            }


            return new JSNumber(@this.value.ToJSDate());
        }

        [Prototype("setHours", Length = 4)]
        internal static JSValue SetHours(in Arguments a)
        {
            var @this = a.This.AsJSDate();
            if (!IsValid(@this, a.Get1(), out var hours))

                return JSNumber.NaN;

            var date = @this.value;

            var (_hours, _mins, _seconds, _millis) = a.Get4();

            var hrs = _hours.IsUndefined ? date.Hour : _hours.IntValue;
            var mins = _mins.IsUndefined ? date.Minute : _mins.IntValue;
            var seconds = _seconds.IsUndefined ? date.Second : _seconds.IntValue;
            var millis = _millis.IsUndefined ? date.Millisecond : _millis.IntValue;

            var extraHours = 0;
            var extraMins = 0;
            var extraSeconds = 0;
            var extraMillis = 0;

            if (hrs > 23) {
                extraHours = hrs;
                hrs = 0;
            }

            if (hrs < 0) {
                extraHours = hrs - 23;
                hrs = 23;

            }

            if (mins > 59) {
                extraMins = mins;
                mins = 0;
            }

            if (mins < 0) {

                extraMins = mins - 59;
                mins = 59;
            }

            if (seconds > 59) {
                extraSeconds = seconds;
                seconds = 0;
            }

            if (seconds < 0) {
                
                extraSeconds = seconds - 59;
                seconds = 59;
            }

            if (millis > 999) {
                extraMillis = millis;
                millis = 0;
            }

            if (millis < 0) {
                
                extraMillis = millis - 999;
                millis = 999;
            }

            @this.value = new DateTimeOffset(date.Year,date.Month,date.Day,hrs,mins,seconds,millis,@this.value.Offset);

            if (extraMillis != 0)
            {
                @this.value = @this.value.AddMilliseconds(extraMillis);
            }

            if (extraSeconds != 0)
            {
                @this.value = @this.value.AddSeconds(extraSeconds);
            }


            if (extraMins != 0)
            {
                @this.value = @this.value.AddMinutes(extraMins);
            }


            if (extraHours != 0)
            {
                @this.value = @this.value.AddHours(extraHours);
            }



          


            return new JSNumber(@this.value.ToJSDate());
        }
    }
}
