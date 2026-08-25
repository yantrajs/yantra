using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using YantraJS.Core.Clr;
using YantraJS.Extensions;
using YantraJS.Core;
namespace YantraJS.Core.Typed { 
partial class JSTypedArray {
public static JSFunction CreateClass(JSContext context, bool register = true) {

                        JSObject prototype = null;
                        var @class = new JSClassFunction(context, (in Arguments a) => new JSTypedArray(in a)
                            , "TypedArray"
                            , "function TypedArray() { [native code] }"
                            );
                        if (register) {
                            context[KeyString.TypedArray] = @class;
                        }
                        prototype = @class.prototype;
                        
// Exporting From as from
@class.FastAddValue(KeyString.from, new JSFunction(context, JSTypedArray.From, "from" ,"function from() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting Of as of
@class.FastAddValue(KeyString.of, new JSFunction(context, JSTypedArray.Of, "of" ,"function of() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting buffer as buffer
prototype.FastAddProperty(
                KeyString.buffer,
                new JSFunction(context, (in Arguments a) =>
                    a.This is JSTypedArray @this
                        ? ClrProxy.Marshal(@this.buffer)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to JSTypedArray") ,
                "get buffer"),
                null,
                JSPropertyAttributes.ConfigurableProperty);
// Exporting byteOffset as byteOffset
prototype.FastAddProperty(
                KeyString.byteOffset,
                new JSFunction(context, (in Arguments a) =>
                    a.This is JSTypedArray @this
                        ? ClrProxy.Marshal(@this.byteOffset)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to JSTypedArray") ,
                "get byteOffset"),
                null,
                JSPropertyAttributes.ConfigurableProperty);
// Exporting bytesPerElement as BYTES_PER_ELEMENT
prototype.FastAddProperty(
                KeyString.BYTES_PER_ELEMENT,
                new JSFunction(context, (in Arguments a) =>
                    a.This is JSTypedArray @this
                        ? ClrProxy.Marshal(@this.bytesPerElement)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to JSTypedArray") ,
                "get BYTES_PER_ELEMENT"),
                null,
                JSPropertyAttributes.ConfigurableProperty);
// Exporting length as length
prototype.FastAddProperty(
                KeyString.length,
                new JSFunction(context, (in Arguments a) =>
                    a.This is JSTypedArray @this
                        ? ClrProxy.Marshal(@this.length)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to JSTypedArray") ,
                "get length"),
                null,
                JSPropertyAttributes.ConfigurableProperty);
// Exporting ByteLength as byteLength
prototype.FastAddProperty(
                KeyString.byteLength,
                new JSFunction(context, (in Arguments a) =>
                    a.This is JSTypedArray @this
                        ? ClrProxy.Marshal(@this.ByteLength)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to JSTypedArray") ,
                "get byteLength"),
                null,
                JSPropertyAttributes.ConfigurableProperty);
// Exporting ToString as toString
prototype.FastAddValue(KeyString.toString, new JSFunction(context, (in Arguments a) =>
                    a.This is JSTypedArray @this
                        ? @this.ToString(in a)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to JSTypedArray")
                        , "toString"
                        ,"function toString() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting CopyWithin as copyWithin
prototype.FastAddValue(KeyString.copyWithin, new JSFunction(context, (in Arguments a) =>
                    a.This is JSTypedArray @this
                        ? @this.CopyWithin(in a)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to JSTypedArray")
                        , "copyWithin"
                        ,"function copyWithin() { [native] }", createPrototype: false, length: 2), JSPropertyAttributes.ConfigurableValue);
// Exporting Entries as entries
prototype.FastAddValue(KeyString.entries, new JSFunction(context, (in Arguments a) =>
                    a.This is JSTypedArray @this
                        ? @this.Entries(in a)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to JSTypedArray")
                        , "entries"
                        ,"function entries() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting Every as every
prototype.FastAddValue(KeyString.every, new JSFunction(context, (in Arguments a) =>
                    a.This is JSTypedArray @this
                        ? @this.Every(in a)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to JSTypedArray")
                        , "every"
                        ,"function every() { [native] }", createPrototype: false, length: 1), JSPropertyAttributes.ConfigurableValue);
// Exporting Fill as fill
prototype.FastAddValue(KeyString.fill, new JSFunction(context, (in Arguments a) =>
                    a.This is JSTypedArray @this
                        ? @this.Fill(in a)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to JSTypedArray")
                        , "fill"
                        ,"function fill() { [native] }", createPrototype: false, length: 1), JSPropertyAttributes.ConfigurableValue);
// Exporting Filter as filter
prototype.FastAddValue(KeyString.filter, new JSFunction(context, (in Arguments a) =>
                    a.This is JSTypedArray @this
                        ? @this.Filter(in a)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to JSTypedArray")
                        , "filter"
                        ,"function filter() { [native] }", createPrototype: false, length: 1), JSPropertyAttributes.ConfigurableValue);
// Exporting Find as find
prototype.FastAddValue(KeyString.find, new JSFunction(context, (in Arguments a) =>
                    a.This is JSTypedArray @this
                        ? @this.Find(in a)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to JSTypedArray")
                        , "find"
                        ,"function find() { [native] }", createPrototype: false, length: 1), JSPropertyAttributes.ConfigurableValue);
// Exporting FindIndex as findIndex
prototype.FastAddValue(KeyString.findIndex, new JSFunction(context, (in Arguments a) =>
                    a.This is JSTypedArray @this
                        ? @this.FindIndex(in a)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to JSTypedArray")
                        , "findIndex"
                        ,"function findIndex() { [native] }", createPrototype: false, length: 1), JSPropertyAttributes.ConfigurableValue);
// Exporting ForEach as forEach
prototype.FastAddValue(KeyString.forEach, new JSFunction(context, (in Arguments a) =>
                    a.This is JSTypedArray @this
                        ? @this.ForEach(in a)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to JSTypedArray")
                        , "forEach"
                        ,"function forEach() { [native] }", createPrototype: false, length: 1), JSPropertyAttributes.ConfigurableValue);
// Exporting Includes as includes
prototype.FastAddValue(KeyString.includes, new JSFunction(context, (in Arguments a) =>
                    a.This is JSTypedArray @this
                        ? @this.Includes(in a)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to JSTypedArray")
                        , "includes"
                        ,"function includes() { [native] }", createPrototype: false, length: 1), JSPropertyAttributes.ConfigurableValue);
// Exporting IndexOf as indexOf
prototype.FastAddValue(KeyString.indexOf, new JSFunction(context, (in Arguments a) =>
                    a.This is JSTypedArray @this
                        ? @this.IndexOf(in a)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to JSTypedArray")
                        , "indexOf"
                        ,"function indexOf() { [native] }", createPrototype: false, length: 1), JSPropertyAttributes.ConfigurableValue);
// Exporting Join as join
prototype.FastAddValue(KeyString.join, new JSFunction(context, (in Arguments a) =>
                    a.This is JSTypedArray @this
                        ? @this.Join(in a)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to JSTypedArray")
                        , "join"
                        ,"function join() { [native] }", createPrototype: false, length: 1), JSPropertyAttributes.ConfigurableValue);
// Exporting Keys as keys
prototype.FastAddValue(KeyString.keys, new JSFunction(context, (in Arguments a) =>
                    a.This is JSTypedArray @this
                        ? @this.Keys(in a)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to JSTypedArray")
                        , "keys"
                        ,"function keys() { [native] }", createPrototype: false, length: 0), JSPropertyAttributes.ConfigurableValue);
// Exporting LastIndexOf as lastIndexOf
prototype.FastAddValue(KeyString.lastIndexOf, new JSFunction(context, (in Arguments a) =>
                    a.This is JSTypedArray @this
                        ? @this.LastIndexOf(in a)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to JSTypedArray")
                        , "lastIndexOf"
                        ,"function lastIndexOf() { [native] }", createPrototype: false, length: 1), JSPropertyAttributes.ConfigurableValue);
// Exporting Map as map
prototype.FastAddValue(KeyString.map, new JSFunction(context, (in Arguments a) =>
                    a.This is JSTypedArray @this
                        ? @this.Map(in a)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to JSTypedArray")
                        , "map"
                        ,"function map() { [native] }", createPrototype: false, length: 1), JSPropertyAttributes.ConfigurableValue);
// Exporting Reduce as reduce
prototype.FastAddValue(KeyString.reduce, new JSFunction(context, (in Arguments a) =>
                    a.This is JSTypedArray @this
                        ? @this.Reduce(in a)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to JSTypedArray")
                        , "reduce"
                        ,"function reduce() { [native] }", createPrototype: false, length: 1), JSPropertyAttributes.ConfigurableValue);
// Exporting ReduceRight as reduceRight
prototype.FastAddValue(KeyString.reduceRight, new JSFunction(context, (in Arguments a) =>
                    a.This is JSTypedArray @this
                        ? @this.ReduceRight(in a)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to JSTypedArray")
                        , "reduceRight"
                        ,"function reduceRight() { [native] }", createPrototype: false, length: 1), JSPropertyAttributes.ConfigurableValue);
// Exporting Reverse as reverse
prototype.FastAddValue(KeyString.reverse, new JSFunction(context, (in Arguments a) =>
                    a.This is JSTypedArray @this
                        ? @this.Reverse(in a)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to JSTypedArray")
                        , "reverse"
                        ,"function reverse() { [native] }", createPrototype: false, length: 0), JSPropertyAttributes.ConfigurableValue);
// Exporting Set as set
prototype.FastAddValue(KeyString.set, new JSFunction(context, (in Arguments a) =>
                    a.This is JSTypedArray @this
                        ? @this.Set(in a)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to JSTypedArray")
                        , "set"
                        ,"function set() { [native] }", createPrototype: false, length: 1), JSPropertyAttributes.ConfigurableValue);
// Exporting Slice as slice
prototype.FastAddValue(KeyString.slice, new JSFunction(context, (in Arguments a) =>
                    a.This is JSTypedArray @this
                        ? @this.Slice(in a)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to JSTypedArray")
                        , "slice"
                        ,"function slice() { [native] }", createPrototype: false, length: 2), JSPropertyAttributes.ConfigurableValue);
// Exporting Some as some
prototype.FastAddValue(KeyString.some, new JSFunction(context, (in Arguments a) =>
                    a.This is JSTypedArray @this
                        ? @this.Some(in a)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to JSTypedArray")
                        , "some"
                        ,"function some() { [native] }", createPrototype: false, length: 1), JSPropertyAttributes.ConfigurableValue);
// Exporting Sort as sort
prototype.FastAddValue(KeyString.sort, new JSFunction(context, (in Arguments a) =>
                    a.This is JSTypedArray @this
                        ? @this.Sort(in a)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to JSTypedArray")
                        , "sort"
                        ,"function sort() { [native] }", createPrototype: false, length: 1), JSPropertyAttributes.ConfigurableValue);
// Exporting SubArray as subarray
prototype.FastAddValue(KeyString.subarray, new JSFunction(context, (in Arguments a) =>
                    a.This is JSTypedArray @this
                        ? @this.SubArray(in a)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to JSTypedArray")
                        , "subarray"
                        ,"function subarray() { [native] }", createPrototype: false, length: 2), JSPropertyAttributes.ConfigurableValue);
// Exporting Values as values
prototype.FastAddValue(KeyString.values, new JSFunction(context, (in Arguments a) =>
                    a.This is JSTypedArray @this
                        ? @this.Values(in a)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to JSTypedArray")
                        , "values"
                        ,"function values() { [native] }", createPrototype: false, length: 2), JSPropertyAttributes.ConfigurableValue);
// Exporting ToLocaleString as toLocaleString
prototype.FastAddValue(KeyString.toLocaleString, new JSFunction(context, (in Arguments a) =>
                    a.This is JSTypedArray @this
                        ? @this.ToLocaleString(in a)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to JSTypedArray")
                        , "toLocaleString"
                        ,"function toLocaleString() { [native] }", createPrototype: false, length: 0), JSPropertyAttributes.ConfigurableValue);
context.TypedArray_Prototype = prototype.PrototypeObject;
return @class;
}
}
}
