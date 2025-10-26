using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using YantraJS.Core.BigInt;

namespace YantraJS.Core;

partial class JSContext
{

    internal JSObject StringPrototype;
    internal JSObject NumberPrototype;
    internal JSObject RegExpPrototype;
    internal JSObject BigIntPrototype;

    public JSValue NewString(string text)
    {
        return new JSString(StringPrototype, text);
    }

    public JSValue NewNumber(double value)
    {
        return new JSNumber(NumberPrototype, value);
    }

    public JSValue NewRegExp(string regex, string flags)
    {
        return new JSRegExp(RegExpPrototype, regex, flags);
    }

    public JSValue NewBigInt(string value)
    {
        return new JSBigInt(BigIntPrototype, value);
    }

    public JSValue NewDecimal(string value)
    {
        return new JSDecimal(BigIntPrototype, value);
    }

}

