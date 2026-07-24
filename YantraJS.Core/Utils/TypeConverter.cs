using System;
using System.Collections.Generic;
using System.Text;
using YantraJS.Core;

namespace YantraJS.Utils
{
    public class TypeConverter
    {

        public static JSValue FromBasic(object value)
        {
            switch(value)
            {
                case null:
                    return JSNull.Value;
                case JSValue jv:
                    return jv;
                case bool b1:
                    return b1 ? JSBoolean.True : JSBoolean.False;
                case uint ui1:
                    return JSNumber.From(ui1);
                case int i1:
                    return JSNumber.From(i1);
                case float f1:
                    return JSNumber.From(f1);
                case double d1:
                    return JSNumber.From(d1);
                case decimal d2:
                    return JSNumber.From((double)d2);
                case string str:
                    return new JSString(str);
            }

            throw new NotSupportedException();
        }

    }
}
