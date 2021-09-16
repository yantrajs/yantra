#r "nuget: YantraJS.Core,1.0.18"
using System;
using System.Linq;
using System.Collections.Generic;
using YantraJS.Core;
using YantraJS.Core.Clr;
using YantraJS.Core.Core.Storage;

[Export]
public class EventEmitter {

    private StringMap<List<JSValue>> listeners;

    private string ToKey(JSValue value)
        => value.IsString
            ? "key:" + value.ToString()
            : ((value is JSSymbol symbol) 
                ? "symbol:" + symbol.Key
                : throw JSContext.CurrentContext.NewTypeError("Key can only be string or symbol"));

    public EventEmitter(in Arguments a)
    {
        
    }

    public JSValue On(in Arguments a) {

        var name = ToKey(a.Get1());
        if (listeners.TryGetValue(name, out var l)){

        }
        return JSUndefined.Value;
    }

}
