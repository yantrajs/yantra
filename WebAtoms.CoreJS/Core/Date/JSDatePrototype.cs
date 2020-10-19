using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;
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
            var dateString = a.Get1();
            if (dateString.IsNumber && double.IsNaN(dateString.DoubleValue))
            {
                return JSDate.invalidDate;
            }
            if (a.Length == 1) {
                if (dateString.IsNumber) {
                    date = DateTimeOffset.FromUnixTimeMilliseconds(dateString.BigIntValue);
                    return new JSDate(date.ToOffset(JSDate.Local));
                }
                date = DateParser.Parse(dateString.ToString());
                if (date == DateTimeOffset.MinValue)
                    return JSDate.invalidDate;
                return new JSDate(date.ToLocalTime());
            }
            var (year, month, day, hours, minutes, seconds, millis ) = a.Get7Int();

            day = day - 1;
            try
            {
                year = year >= 0 && year < 100 ? year + 1900 : year;
                date = new DateTimeOffset(year, 1, 1, 0, 0, 0, 0, JSDate.Local);
                date = date.AddMilliseconds(millis);
                date = date.AddSeconds(seconds);
                date = date.AddMinutes(minutes);
                date = date.AddHours(hours);
                date = date.AddDays(day);
                date = date.AddMonths(month);

                return new JSDate(date);
            }
            catch (ArgumentOutOfRangeException) {
                return JSDate.invalidDate;
            }

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

            try
            {
                @this.value = @this.value.AddDays(-@this.value.Day + diffValue);
            }
            catch (ArgumentOutOfRangeException) {
                @this.value = DateTimeOffset.MinValue;
            }
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

            var month = _month.IsUndefined ? date.Month - 1 : _month.IntValue;
            var day = (_day.IsUndefined ? date.Day : _day.IntValue) - 1;

            try
            {
                date = new DateTimeOffset((int)year, 1, 1, 
                    date.Hour, date.Minute, date.Second, 
                    date.Millisecond, @this.value.Offset);
                date = date.AddDays(day);
                date = date.AddMonths(month);
                @this.value = date;
            } catch (ArgumentOutOfRangeException)
            {
                @this.value = DateTimeOffset.MinValue;
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
            try
            {
                date = new DateTimeOffset(date.Year, date.Month, date.Day, 0, 0, 0, 0, 
                    date.Offset);
                date = date.AddMilliseconds(millis);
                date = date.AddSeconds(seconds);
                date = date.AddMinutes(mins);
                date = date.AddHours(hrs);
                @this.value = date;
            }
            catch (ArgumentOutOfRangeException) {
                @this.value = DateTimeOffset.MinValue;
            }
            return new JSNumber(@this.value.ToJSDate());
        }



        [Prototype("setMilliseconds", Length = 1)]
        internal static JSValue SetMilliseconds(in Arguments a)
        {
            var @this = a.This.AsJSDate();
            if (!IsValid(@this, a.Get1(), out var _millis))

                return JSNumber.NaN;

            var date = @this.value;
            try
            {
                date = new DateTimeOffset(date.Year, date.Month, date.Day, date.Hour, 
                                                date.Minute, date.Second, 0, date.Offset);
                date = @this.value.AddMilliseconds(_millis);
                @this.value = date;
            }
            catch (ArgumentOutOfRangeException) {
                @this.value = DateTimeOffset.MinValue;
            }
            return new JSNumber(@this.value.ToJSDate());
        }

        [Prototype("setMinutes", Length =3)]
        internal static JSValue SetMinutes(in Arguments a)
        {
            var @this = a.This.AsJSDate();
            if (!IsValid(@this, a.Get1(), out var minutes))

                return JSNumber.NaN;

            var date = @this.value;
            var (_mins, _seconds, _millis) = a.Get3();
            var mins = _mins.IsUndefined ? date.Minute : _mins.IntValue;
            var seconds = _seconds.IsUndefined ? date.Second : _seconds.IntValue;
            var millis = _millis.IsUndefined ? date.Millisecond : _millis.IntValue;

            try
            {
                date = new DateTimeOffset(date.Year, date.Month, date.Day, date.Hour, 0, 0, 0, date.Offset);
                date = date.AddMilliseconds(millis);
                date = date.AddSeconds(seconds);
                date = date.AddMinutes(mins);
                @this.value = date;
            }
            catch (ArgumentOutOfRangeException) {
                @this.value = DateTimeOffset.MinValue;
            }
            return new JSNumber(@this.value.ToJSDate());
        }

        [Prototype("setMonth", Length = 2)]
        internal static JSValue SetMonth(in Arguments a)
        {
            var @this = a.This.AsJSDate();
            if (!IsValid(@this, a.Get1(), out var mnth))

                return JSNumber.NaN;

            var date = @this.value;
            var (_month, _days) = a.Get2();
            var month = _month.IsUndefined ? date.Month : _month.IntValue;
            var days = (_days.IsUndefined ? date.Day : _days.IntValue) - 1;

            try
            {
                date = new DateTimeOffset(date.Year, 1, 1, date.Hour, date.Minute, date.Second, date.Millisecond, date.Offset);
                date = date.AddDays(days);
                date = date.AddMonths(month);
                @this.value = date;

            }
            catch (ArgumentOutOfRangeException) {
                @this.value = DateTimeOffset.MinValue;
            }
            return new JSNumber(@this.value.ToJSDate());
        }


        [Prototype("setSeconds", Length = 2)]
        internal static JSValue SetSeconds(in Arguments a)
        {
            var @this = a.This.AsJSDate();
            if (!IsValid(@this, a.Get1(), out var secs))

                return JSNumber.NaN;

            var date = @this.value;
            var (_seconds, _millis) = a.Get2();
            var seconds = _seconds.IsUndefined ? date.Second : _seconds.IntValue;
            var millis = _millis.IsUndefined ? date.Millisecond : _millis.IntValue;

            try
            {
                date = new DateTimeOffset(date.Year, date.Month, date.Day, date.Hour, date.Minute, 0, 0, date.Offset);
                date = date.AddMilliseconds(millis);
                date = date.AddSeconds(seconds);
                @this.value = date;
            }
            catch (ArgumentOutOfRangeException) {
                @this.value = DateTimeOffset.MinValue;
            }
            return new JSNumber(@this.value.ToJSDate());
        }


        [Prototype("setTime", Length = 1)]
        internal static JSValue SetTime(in Arguments a)
        {
            var @this = a.This.AsJSDate();
            if (!IsValid(@this, a.Get1(), out var _time))

                return JSNumber.NaN;
            try
            {
                @this.value = DateTimeOffset.FromUnixTimeMilliseconds((long)_time).ToOffset(JSDate.Local);
            }
            catch (ArgumentOutOfRangeException) {
                @this.value = DateTimeOffset.MinValue;
            }
            return new JSNumber(@this.value.ToJSDate());
        }


        [Prototype("setUTCDate", Length = 1)]
        internal static JSValue setUTCDate(in Arguments a)
        {
            var @this = a.This.AsJSDate();
            if (!IsValid(@this, a.Get1(), out var _date))

                return JSNumber.NaN;
            try
            {
                var date = @this.value;
                var offset = date.Offset;
                var utc = date.ToUniversalTime();
                utc = utc.AddDays(-utc.Day + _date);
                @this.value = utc.ToOffset(offset);                
            }
            catch (ArgumentOutOfRangeException)
            {
                @this.value = DateTimeOffset.MinValue;
            }
            return new JSNumber(@this.value.ToJSDate());
        }

        [Prototype("setUTCFullYear", Length = 1)]
        internal static JSValue setUTCFullYear(in Arguments a)
        {
            var @this = a.This.AsJSDate();
            if (!IsValid(@this, a.Get1(), out var year))

                return JSNumber.NaN;

            if (year <= 0)
            {
                @this.value = DateTimeOffset.MinValue;
                return JSNumber.NaN;
            }


            var date = @this.value;
            var (_year, _month, _day) = a.Get3();

            var offset = date.Offset;
            var utc = date.ToUniversalTime();

            var month = _month.IsUndefined ? utc.Month - 1: _month.IntValue;
            var day = (_day.IsUndefined ? utc.Day : _day.IntValue) - 1;

            try
            {

                utc = new DateTimeOffset((int)year, 1, 1,
                    utc.Hour, utc.Minute, utc.Second,
                    utc.Millisecond, utc.Offset);
                utc = utc.AddDays(day);
                utc = utc.AddMonths(month);
                @this.value = utc.ToOffset(offset);
            }
            catch (ArgumentOutOfRangeException)
            {
                @this.value = DateTimeOffset.MinValue;
            }
            return new JSNumber(@this.value.ToJSDate());

        }


        [Prototype("setUTCHours", Length = 4)]
        internal static JSValue SetUTCHours(in Arguments a)
        {
            var @this = a.This.AsJSDate();
            if (!IsValid(@this, a.Get1(), out var hours))

                return JSNumber.NaN;

            var date = @this.value;

            var (_hours, _mins, _seconds, _millis) = a.Get4();

            var offset = date.Offset;
            var utc = date.ToUniversalTime();

            var hrs = _hours.IsUndefined ? utc.Hour : _hours.IntValue;
            var mins = _mins.IsUndefined ? utc.Minute : _mins.IntValue;
            var seconds = _seconds.IsUndefined ? utc.Second : _seconds.IntValue;
            var millis = _millis.IsUndefined ? utc.Millisecond : _millis.IntValue;
            try
            {
                utc = new DateTimeOffset(utc.Year, utc.Month, utc.Day, 0, 0, 0, 0,
                    utc.Offset);
                utc = utc.AddMilliseconds(millis);
                utc = utc.AddSeconds(seconds);
                utc = utc.AddMinutes(mins);
                utc = utc.AddHours(hrs);
                @this.value = utc.ToOffset(offset);
            }
            catch (ArgumentOutOfRangeException)
            {
                @this.value = DateTimeOffset.MinValue;
            }
            return new JSNumber(@this.value.ToJSDate());
        }



        [Prototype("setUTCMilliseconds", Length = 1)]
        internal static JSValue SetUTCMilliseconds(in Arguments a)
        {
            var @this = a.This.AsJSDate();
            if (!IsValid(@this, a.Get1(), out var _millis))

                return JSNumber.NaN;

            var date = @this.value;
            var offset = date.Offset;
            var utc = date.ToUniversalTime();
           
            try
            {
                utc = new DateTimeOffset(utc.Year, utc.Month, utc.Day, utc.Hour,
                                                utc.Minute, utc.Second, 0, utc.Offset);
                utc = utc.AddMilliseconds(_millis);
                
                @this.value = utc.ToOffset(offset);
            }
            catch (ArgumentOutOfRangeException)
            {
                @this.value = DateTimeOffset.MinValue;
            }
            return new JSNumber(@this.value.ToJSDate());
        }


        [Prototype("setUTCMinutes", Length = 3)]
        internal static JSValue SetUTCMinutes(in Arguments a)
        {
            var @this = a.This.AsJSDate();
            if (!IsValid(@this, a.Get1(), out var minutes))

                return JSNumber.NaN;

            var date = @this.value;
            var offset = date.Offset;
            var utc = date.ToUniversalTime();
            var (_mins, _seconds, _millis) = a.Get3();
            var mins = _mins.IsUndefined ? utc.Minute : _mins.IntValue;
            var seconds = _seconds.IsUndefined ? utc.Second : _seconds.IntValue;
            var millis = _millis.IsUndefined ? utc.Millisecond : _millis.IntValue;

            try
            {
                utc = new DateTimeOffset(utc.Year, utc.Month, utc.Day, utc.Hour, 0, 0, 0, utc.Offset);
                utc = utc.AddMilliseconds(millis);
                utc = utc.AddSeconds(seconds);
                utc = utc.AddMinutes(mins);
                @this.value = utc.ToOffset(offset);
            }
            catch (ArgumentOutOfRangeException)
            {
                @this.value = DateTimeOffset.MinValue;
            }
            return new JSNumber(@this.value.ToJSDate());
        }


        [Prototype("setUTCMonth", Length = 2)]
        internal static JSValue SetUTCMonth(in Arguments a)
        {
            var @this = a.This.AsJSDate();
            if (!IsValid(@this, a.Get1(), out var mnth))

                return JSNumber.NaN;

            var date = @this.value;
            var offset = date.Offset;
            var utc = date.ToUniversalTime();
            var (_month, _days) = a.Get2();
            var month = _month.IsUndefined ? utc.Month : _month.IntValue;
            var days = (_days.IsUndefined ? utc.Day : _days.IntValue) - 1;

            try
            {
                utc = new DateTimeOffset(utc.Year, 1, 1, utc.Hour, utc.Minute, utc.Second, utc.Millisecond, utc.Offset);
                utc = utc.AddDays(days);
                utc = utc.AddMonths(month);
                @this.value = utc.ToOffset(offset);

            }
            catch (ArgumentOutOfRangeException)
            {
                @this.value = DateTimeOffset.MinValue;
            }
            return new JSNumber(@this.value.ToJSDate());
        }



        [Prototype("setUTCSeconds", Length = 2)]
        internal static JSValue SetUTCSeconds(in Arguments a)
        {
            var @this = a.This.AsJSDate();
            if (!IsValid(@this, a.Get1(), out var secs))

                return JSNumber.NaN;

            var date = @this.value;
            var offset = date.Offset;
            var utc = date.ToUniversalTime();
            var (_seconds, _millis) = a.Get2();
            var seconds = _seconds.IsUndefined ? utc.Second : _seconds.IntValue;
            var millis = _millis.IsUndefined ? utc.Millisecond : _millis.IntValue;

            try
            {
                utc = new DateTimeOffset(utc.Year, utc.Month, utc.Day, utc.Hour, utc.Minute, 0, 0, utc.Offset);
                utc = utc.AddMilliseconds(millis);
                utc = utc.AddSeconds(seconds);
                @this.value = utc.ToOffset(offset);
            }
            catch (ArgumentOutOfRangeException)
            {
                @this.value = DateTimeOffset.MinValue;
            }
            return new JSNumber(@this.value.ToJSDate());
        }


        [Prototype("toDateString", Length = 0)]
        internal static JSValue ToDateString(in Arguments a)
        {
            var @this = a.This.AsJSDate();
            if (@this.value == JSDate.InvalidDate)
                return new JSString("Invalid Date");
            var date =  @this.value.ToLocalTime().ToString("ddd MMM dd yyyy", System.Globalization.DateTimeFormatInfo.InvariantInfo);

            return new JSString(date);
            
        }

        [Prototype("toISOString", Length = 0)]
        internal static JSValue ToISOString(in Arguments a)
        {
            var @this = a.This.AsJSDate();
            if (@this.value == JSDate.InvalidDate)
                return new JSString("Invalid Date");
            var date = @this.value.ToUniversalTime().ToString("yyyy-MM-dd'T'HH:mm:ss.fff'Z'", System.Globalization.DateTimeFormatInfo.InvariantInfo);

            return new JSString(date);

        }


        [Prototype("toJSON", Length = 1)]
        internal static JSValue ToJSON(in Arguments a)
        {
            var @this = a.This.AsJSDate();
            if (@this.value == JSDate.InvalidDate)
                return JSNull.Value;
            var date = @this.value.ToUniversalTime().ToString("yyyy-MM-dd'T'HH:mm:ss.fff'Z'", System.Globalization.DateTimeFormatInfo.InvariantInfo);

            return new JSString(date);

        }


        [Prototype("toLocaleDateString", Length = 0)]
        internal static JSValue ToLocaleDateString(in Arguments a)
        {
            var @this = a.This.AsJSDate();
            if (@this.value == JSDate.InvalidDate)
                return new JSString("Invalid Date");
            var (locale, format) = a.Get2();
            string date = null;
            if (locale.IsNullOrUndefined)
            {
                date = @this.value.ToString("D", System.Globalization.DateTimeFormatInfo.CurrentInfo);
            }
            else {
                var culture = CultureInfo.GetCultureInfo(locale.ToString());
                if (format.IsNullOrUndefined) {
                    date = @this.value.ToString("D", culture);
                } else
                {
                    if (format.IsString)
                    {
                        date = @this.value.ToString(format.ToString(), culture);
                    }
                    else {
                        throw JSContext.Current.NewTypeError("Options not supported, use .Net String Formats");
                    }
                }
            }
            return new JSString(date);

        }


        [Prototype("toLocaleString", Length = 0)]
        internal static JSValue ToLocaleString(in Arguments a)
        {
            var @this = a.This.AsJSDate();
            if (@this.value == JSDate.InvalidDate)
                return new JSString("Invalid Date");
            var (locale, format) = a.Get2();
            string date = null;
            if (locale.IsNullOrUndefined)
            {
                date = @this.value.ToString("F", System.Globalization.DateTimeFormatInfo.CurrentInfo);
            }
            else
            {
                var culture = CultureInfo.GetCultureInfo(locale.ToString());
                if (format.IsNullOrUndefined)
                {
                    date = @this.value.ToString("F", culture);
                }
                else
                {
                    if (format.IsString)
                    {
                        date = @this.value.ToString(format.ToString(), culture);
                    }
                    else
                    {
                        throw JSContext.Current.NewTypeError("Options not supported, use .Net String Formats");
                    }
                }
            }
            return new JSString(date);

        }


        [Prototype("toString", Length = 0)]
        internal static JSValue ToString(in Arguments a)
        {
            var @this = a.This.AsJSDate();
            if (@this.value == JSDate.InvalidDate)
                return new JSString("Invalid Date");
            var date = @this.value.
                       ToString("ddd MMM dd yyyy HH:mm:ss ", System.Globalization.DateTimeFormatInfo.InvariantInfo) +
                       ToTimeZoneString(@this);

            return new JSString(date);

        }



        [Prototype("toTimeString", Length = 0)]
        internal static JSValue ToTimeString(in Arguments a)
        {
            var @this = a.This.AsJSDate();
            if (@this.value == JSDate.InvalidDate)
                return new JSString("Invalid Date");
            // DateTimeFormatInfo.CurrentInfo.LongTimePattern
            var date = @this.value.
                       ToString("HH:mm:ss ", System.Globalization.DateTimeFormatInfo.InvariantInfo) +
                       ToTimeZoneString(@this);

            return new JSString(date);

        }




        [Prototype("toUTCString", Length = 0)]
        internal static JSValue ToUTCString(in Arguments a)
        {
            var @this = a.This.AsJSDate();
            if (@this.value == JSDate.InvalidDate)
                return new JSString("Invalid Date");
            var date = @this.value.ToUniversalTime().
                       ToString("ddd, dd MMM yyyy HH:mm:ss 'GMT'", 
                       System.Globalization.DateTimeFormatInfo.InvariantInfo);

            return new JSString(date);

        }







        [Prototype("valueOf", Length = 0)]
        internal static JSValue ValueOf(in Arguments a)
        {
            var @this = a.This.AsJSDate();
            if (@this.value == DateTimeOffset.MinValue)
                return JSNumber.NaN;
            var result = @this.value.ToJSDate();
            return new JSNumber(result);
        }


        internal static string ToTimeZoneString(JSDate @this) {
            var timeZone = TimeZoneInfo.Local;
            // Compute the time zone offset in hours-minutes.
            int offsetInMinutes = (int)timeZone.GetUtcOffset(@this.value).TotalMinutes;
            int hhmm = offsetInMinutes / 60 * 100 + offsetInMinutes % 60;

            // Get the time zone name.
            string zoneName;
            if (timeZone.IsDaylightSavingTime(@this.value))
                zoneName = timeZone.DaylightName;
            else
                zoneName = timeZone.StandardName;

            if (hhmm < 0)
                return $"GMT{hhmm:d4} ({zoneName})";
            else
                return $"GMT+{hhmm:d4} ({zoneName})";
        }

    }
}
