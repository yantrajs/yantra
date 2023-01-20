using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using YantraJS.Core.Clr;

namespace YantraJS.Core.Core.Intl
{
    public class JSIntl
    {
        [JSExportSameName]
        public static JSValue DateTimeFormat => ClrType.From(typeof(JSIntlDateTimeFormat));

        [JSExportSameName]
        public static JSValue RelativeTimeFormat => ClrType.From(typeof(JSIntlRelativeTimeFormat));

    }

    public class JSIntlRelativeTimeFormat : JavaScriptObject
    {
        public JSIntlRelativeTimeFormat(in Arguments a) : base(a)
        {
        }

        [JSExport]
        public JSValue Format(in Arguments a)
        {
            return a[0] ?? JSUndefined.Value;
        }

    }

    public class JSIntlDateTimeFormat : JavaScriptObject
    {
        private static ConcurrentDictionary<string, JSIntlDateTimeFormat> formats = new ConcurrentDictionary<string, JSIntlDateTimeFormat>();
        private readonly CultureInfo locale;

        public static JSIntlDateTimeFormat Get(CultureInfo culture)
        {
            return formats.GetOrAdd(culture.DisplayName, (key) => new JSIntlDateTimeFormat(culture));
        }

        [JSExport]
        public JSValue Format(in Arguments a)
        {
            return a[0] ?? JSUndefined.Value;
        }

        internal JSValue Format(DateTimeOffset value, JSObject format)
        {
            return new JSString( value.ToString());
        }

        public JSIntlDateTimeFormat(in Arguments a) : base(a)
        {
        }

        internal JSIntlDateTimeFormat(CultureInfo locale): base(Arguments.Empty)
        {
            this.locale = locale;
        }


    }

}
