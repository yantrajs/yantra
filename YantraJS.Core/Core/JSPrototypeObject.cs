namespace YantraJS.Core;

// This is simply a placeholder class to avoid
// passing JSObject of anything
public readonly struct JSPrototypeObject 
{

    public static JSPrototypeObject NewTarget
    {
        get
        {
            return new JSPrototypeObject(JSContext.CurrentContext.CurrentNewTarget.prototype);
        }
    }
    
    public readonly JSPrototype prototype;
    
    public JSPrototypeObject(JSObject @object) 
    {
        this.prototype = @object.PrototypeObject;
    }
    
    public JSPrototypeObject(JSPrototype prototype) 
    {
        this.prototype = prototype;
    }
    
    public static implicit operator JSPrototypeObject (JSPrototype p)
    {
        return new JSPrototypeObject(p);
    }
    
    public static implicit operator JSPrototypeObject (in Arguments a)
    {
        return new JSPrototypeObject(a.This.prototypeChain);
    }
}