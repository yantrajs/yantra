#r "nuget: YantraJS.Core,1.1.106"
using System;
using System.Linq;
using System.Collections.Generic;
using YantraJS.Core;
using YantraJS.Core.Clr;
using YantraJS.Core.Core.Storage;



public class Event {

}

public class EventInfo {
    public JSValue Listener;

    public Action<Event> Delegate;

}

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
        this.AddEventListener(name, (e) => a.Get2().InvokeFunction(new Arguments(JSUndefined.Value, e.Marshal() )));
        return JSUndefined.Value;
    }

    public JSValue AddListener(in Arguments a) => On(in a);

}
