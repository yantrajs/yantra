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
                    return new JSNumber(ui1);
                case int i1:
                    return new JSNumber(i1);
                case float f1:
                    return new JSNumber(f1);
                case double d1:
                    return new JSNumber(d1);
                case decimal d2:
                    return new JSNumber((double)d2);
                case string str:
                    return new JSString(str);
            }

            throw new NotSupportedException();
        }

    }
}
