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
            DateTime date;
            if (a.Length == 1) {
                var dateString = a.Get1();
                if (dateString.IsNumber) {
                    var utc = dateString.BigIntValue * 10000;
                    utc += JSDateStatic.epoch;
                    date = new DateTime(utc, DateTimeKind.Utc);
                    return new JSDate(date.ToLocalTime());
                }
                date = DateParser.Parse(dateString.ToString());
                if (date == DateTime.MinValue)
                    return JSDate.invalidDate;
                return new JSDate(date.ToLocalTime());
            }
            throw new NotImplementedException();
        }



        [Prototype("getDate", Length = 0)]
        internal static JSValue GetDate(in Arguments a)
        {
            var @this = a.This.AsJSDate();
            if (@this.value == DateTime.MinValue)
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
            if (@this.value == DateTime.MinValue)
                return false;

            if (diff.IsUndefined)
            {
                @this.value = DateTime.MinValue;
                return false;
            }

            diffValue = diff.DoubleValue;
            if (double.IsNaN(diffValue))
            {
                @this.value = DateTime.MinValue;
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
            if (@this.value == DateTime.MinValue)
                return JSNumber.NaN;
            var result = @this.value.DayOfWeek;
            return new JSNumber((double)result);
        }

        [Prototype("getFullYear", Length = 0)]
        internal static JSValue GetFullYear(in Arguments a)
        {
            var @this = a.This.AsJSDate();
            if (@this.value == DateTime.MinValue)
                return JSNumber.NaN;
            var result = @this.value.Year;
            return new JSNumber(result);
        }

        [Prototype("getHours", Length = 0)]
        internal static JSValue GetHours(in Arguments a)
        {
            var @this = a.This.AsJSDate();
            if (@this.value == DateTime.MinValue)
                return JSNumber.NaN;
            var result = @this.value.Hour;
            return new JSNumber(result);
        }



        [Prototype("getMilliseconds", Length = 0)]
        internal static JSValue GetMilliSeconds(in Arguments a)
        {
            var @this = a.This.AsJSDate();
            if (@this.value == DateTime.MinValue)
                return JSNumber.NaN;
            var result = @this.value.Millisecond;
            return new JSNumber(result);
        }



        [Prototype("getMinutes", Length = 0)]
        internal static JSValue GetMinutes(in Arguments a)
        {
            var @this = a.This.AsJSDate();
            if (@this.value == DateTime.MinValue)
                return JSNumber.NaN;
            var result = @this.value.Minute;
            return new JSNumber(result);
        }

        [Prototype("getMonth", Length = 0)]
        internal static JSValue GetMonth(in Arguments a)
        {
            var @this = a.This.AsJSDate();
            if (@this.value == DateTime.MinValue)
                return JSNumber.NaN;
            var result = @this.value.Month - 1;
            return new JSNumber(result);
        }


        [Prototype("getSeconds", Length = 0)]
        internal static JSValue GetSeconds(in Arguments a)
        {
            var @this = a.This.AsJSDate();
            if (@this.value == DateTime.MinValue)
                return JSNumber.NaN;
            var result = @this.value.Second;
            return new JSNumber(result);
        }

        [Prototype("getTime", Length = 0)]
        internal static JSValue GetTime(in Arguments a)
        {
            var @this = a.This.AsJSDate();
            if (@this.value == DateTime.MinValue)
                return JSNumber.NaN;
            var result = @this.value.ToJSDate();
            return new JSNumber(result);
        }

        [Prototype("getTimezoneOffset", Length = 0)]
        internal static JSValue GetTimezoneOffset(in Arguments a)
        {
            var @this = a.This.AsJSDate();
            if (@this.value == DateTime.MinValue)
                return JSNumber.NaN;
            var result = -(int)TimeZoneInfo.Local.GetUtcOffset(@this.Value).TotalMinutes;
            return new JSNumber(result);
        }

        [Prototype("getUTCDate", Length = 0)]
        internal static JSValue GetUTCDate(in Arguments a)
        {
            var @this = a.This.AsJSDate();
            if (@this.value == DateTime.MinValue)
                return JSNumber.NaN;
            var result = @this.value.ToUniversalTime().Day;
            return new JSNumber(result);
        }

        [Prototype("getUTCDay", Length = 0)]
        internal static JSValue GetUTCDay(in Arguments a)
        {
            var @this = a.This.AsJSDate();
            if (@this.value == DateTime.MinValue)
                return JSNumber.NaN;
            var result = @this.value.ToUniversalTime().DayOfWeek;
            return new JSNumber((double)result);
        }

        [Prototype("getUTCFullYear", Length = 0)]
        internal static JSValue GetUTCFullYear(in Arguments a)
        {
            var @this = a.This.AsJSDate();
            if (@this.value == DateTime.MinValue)
                return JSNumber.NaN;
            var result = @this.value.ToUniversalTime().Year;
            return new JSNumber(result);
        }

        [Prototype("getUTCHours", Length = 0)]
        internal static JSValue GetUTCHours(in Arguments a)
        {
            var @this = a.This.AsJSDate();
            if (@this.value == DateTime.MinValue)
                return JSNumber.NaN;
            var result = @this.value.ToUniversalTime().Hour;
            return new JSNumber(result);
        }

        [Prototype("getUTCMilliseconds", Length = 0)]
        internal static JSValue GetUTCMilliseconds(in Arguments a)
        {
            var @this = a.This.AsJSDate();
            if (@this.value == DateTime.MinValue)
                return JSNumber.NaN;
            var result = @this.value.ToUniversalTime().Millisecond;
            return new JSNumber(result);
        }

        [Prototype("getUTCMinutes", Length = 0)]
        internal static JSValue GetUTCMinutes(in Arguments a)
        {
            var @this = a.This.AsJSDate();
            if (@this.value == DateTime.MinValue)
                return JSNumber.NaN;
            var result = @this.value.ToUniversalTime().Minute;
            return new JSNumber(result);
        }

        [Prototype("getUTCMonth", Length = 0)]
        internal static JSValue GetUTCMonth(in Arguments a)
        {
            var @this = a.This.AsJSDate();
            if (@this.value == DateTime.MinValue)
                return JSNumber.NaN;
            var result = @this.value.ToUniversalTime().Month - 1;
            return new JSNumber(result);
        }

        [Prototype("getUTCSeconds", Length = 0)]
        internal static JSValue GetUTCSeconds(in Arguments a)
        {
            var @this = a.This.AsJSDate();
            if (@this.value == DateTime.MinValue)
                return JSNumber.NaN;
            var result = @this.value.ToUniversalTime().Second;
            return new JSNumber(result);
        }

    }
}
