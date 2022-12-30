using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using YantraJS.Core.Clr;

namespace YantraJS.Core.Core.Intl
{
    public static class JSIntl
    {

        public static JSValue DateTimeFormat = ClrType.From(typeof(JSIntlDateTimeFormat));

    }

    public class JSIntlDateTimeFormat : JavaScriptObject
    {
        public JSIntlDateTimeFormat(in Arguments a) : base(a)
        {
        }


    }

    public static class JSIntlDateTimeExtensions
    {

        private static Dictionary<string[], Func<DateTimeOffset, JSValue>> formats
            = new Dictionary<string[], Func<DateTimeOffset, JSValue>>();

        static JSIntlDateTimeExtensions()
        {
            
        }

        /// <summary>
        /// 1. Save available formats in key:value pair
        /// 2. Loop through available formats
        /// 3. Save map of all possible formats
        /// 4. Create a map of parts as well.
        /// </summary>
        /// <param name="this"></param>
        /// <param name="culture"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>

        public static JSValue ToLocaleDateString(this DateTimeOffset @this, CultureInfo culture, JSObject format)
        {
            var weekday = format[KeyStrings.weekday];
            var year = format[KeyStrings.year];
            var month = format[KeyStrings.month];
            var day = format[KeyStrings.day];
            var hour = format[KeyStrings.hour];
            var minute = format[KeyStrings.minute];
            var second = format[KeyStrings.second];

            throw new NotImplementedException();
        }

    }
}
