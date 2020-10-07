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
    }
}
