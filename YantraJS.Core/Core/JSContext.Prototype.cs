namespace YantraJS.Core;

partial class JSContext
{

    public JSPrototypeObject GetGlobalPrototype(string name)
    {
        var key = KeyStrings.Instance.GetOrCreate(name);
        var @object = this[key];
        if (@object[KeyString.prototype] is not JSObject o)
        {
            throw this.NewTypeError("Prototype not found");
        }
        return new JSPrototypeObject(o);
    }


    internal JSPrototype String_Prototype;
    internal JSPrototype Object_Prototype;
    internal JSPrototype Map_Prototype;
    internal JSPrototype Number_Prototype;
    internal JSPrototype Array_Prototype;
    internal JSPrototype Function_Prototype;
    internal JSPrototype Decimal_Prototype;
    internal JSPrototype Date_Prototype;
    internal JSPrototype FinalizationRegistry_Prototype;
    internal JSPrototype WeakSet_Prototype;
    internal JSPrototype WeakRef_Prototype;
    internal JSPrototype Boolean_Prototype;
    internal JSPrototype Reflect_Prototype;
    internal JSPrototype Proxy_Prototype;
    internal JSPrototype Set_Prototype;
    internal JSPrototype Symbol_Prototype;
    internal JSPrototype Promise_Prototype;
    internal JSPrototype RegExp_Prototype;
    internal JSPrototype BigInt_Prototype;
    internal JSPrototype WeakMap_Prototype;
    internal JSPrototype JSON_Prototype;
    internal JSPrototype Globals_Prototype;
    internal JSPrototype Module_Prototype;
    internal JSPrototype Generator_Prototype;

    internal JSPrototype EventTarget_Prototype;
    internal JSPrototype TypedArray_Prototype;
    internal JSPrototype ArrayBuffer_Prototype;
    internal JSPrototype DataView_Prototype;
    internal JSPrototype Int8Array_Prototype;
    internal JSPrototype Int16Array_Prototype;
    internal JSPrototype Int32Array_Prototype;
    internal JSPrototype Int64Array_Prototype;
    internal JSPrototype Uint8Array_Prototype;
    internal JSPrototype Uint16Array_Prototype;
    internal JSPrototype Uint32Array_Prototype;
    internal JSPrototype Float32Array_Prototype;
    internal JSPrototype Float64Array_Prototype;
    internal JSPrototype Uint8ClampedArray_Prototype;

    internal JSPrototype SuppressedError_Prototype;
    internal JSPrototype EvalError_Prototype;
    internal JSPrototype Math_Prototype;
    internal JSPrototype Assert_Prototype;
    internal JSPrototype URIError_Prototype;
    internal JSPrototype RangeError_Prototype;
    internal JSPrototype TypeError_Prototype;
    internal JSPrototype SyntaxError_Prototype;
    internal JSPrototype Error_Prototype;


}