using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace YantraJS.Core;

partial class JSContext
{

    internal JSObject StringPrototype;
    internal JSObject NumberPrototype;

    public JSValue NewString(string text)
    {
        return new JSString(StringPrototype, text);
    }

    public JSValue NewNumber(double value)
    {
        return new JSNumber(NumberPrototype, value);
    }

}

