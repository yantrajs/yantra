using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace WebAtoms.CoreJS.Utils
{
    /// <summary>
    /// 
    /// </summary>
    public static class DateParser
    {
        internal static readonly string[] DefaultFormats = {
            "yyyy-MM-ddTHH:mm:ss.FFF",
            "yyyy-MM-ddTHH:mm:ss",
            "yyyy-MM-ddTHH:mm",
            "yyyy-MM-dd",
            "yyyy-MM",
            "yyyy"
        };

        internal static readonly string[] SecondaryFormats = {
            // Formats used in DatePrototype toString methods
            "ddd MMM dd yyyy HH:mm:ss 'GMT'K",
            "ddd MMM dd yyyy",
            // ES Date Format
            "MMMM dd, yyyy HH:mm:ss \\G\\M\\TK",
            "HH:mm:ss 'GMT'K",

            // standard formats
             "yyyy-M-dTH:m:s.FFFK",
            "yyyy/M/dTH:m:s.FFFK",
            "yyyy-M-dTH:m:sK", 
             "yyyy/M/dTH:m:sK",
            // "yyyy-M-dTH:mK", //commented for this TC assert(isNaN(Date.parse('1970-01-01T5:34'))); & assert(isNaN(Date.parse('1970-01-01T05:3')))
            // "yyyy/M/dTH:mK",
             "yyyy-M-d H:m:s.FFFK",
             "yyyy/M/d H:m:s.FFFK",
             "yyyy-M-d H:m:sK",
            "yyyy/M/d H:m:sK",
            "yyyy-M-d H:mK",
            "yyyy/M/d H:mK",
            "yyyy-M-dK",
            "yyyy/M/dK",
            "yyyy-MK",
            "yyyy/MK",
            "yyyyK",
            "THH:mm:ss.FFFK",
            "THH:mm:ssK",
            // "THH:mmK", // Commented for TC - assert(isNaN(Date.parse('T12:34Z')));
            "THHK",
            // new formats added from DateTests.cs of Jurrasic
            // "yyyy-MM-dTH:m",
            //"yyyy/M/dTH:m",
            "yyyyTH:m"

        };

        internal static DateTime Parse(string text) {
            // DateTime result;
            if (!DateTime.TryParseExact(text, DateParser.DefaultFormats, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out var result))
            {
                if (!DateTime.TryParseExact(text, DateParser.SecondaryFormats, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out result))
                {
                    if (!DateTime.TryParse(text, CultureInfo.CurrentCulture, DateTimeStyles.AssumeLocal, out result))
                    {
                        if (!DateTime.TryParse(text, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out result))
                        {
                            // unrecognized dates should return NaN (15.9.4.2)
                            return DateTime.MinValue;
                        }
                    }
                }
            }
            return result;
        }

    }
}
