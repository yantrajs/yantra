using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using YantraJS.Core.Clr;
using YantraJS.Extensions;
namespace YantraJS.Core { 
partial class JSArray {
public static JSFunction CreateClass(JSContext context, bool register = true) {

                        JSObject prototype = null;
                        var @class = new JSFunction(context, (in Arguments a) => JSArray.Constructor(in a)
                            , "Array"
                            , "function Array() { [native code] }"
                            , length:1);
                        if (register) {
                            context[KeyString.Array] = @class;
                        }
                        prototype = @class.prototype;
                        
// Exporting ArrayLength as length
prototype.FastAddProperty(
                KeyString.length,
                new JSFunction(context, (in Arguments a) =>
                    a.This is JSArray @this
                        ? ClrProxy.Marshal(@this.ArrayLength)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to JSArray") ,
                "get length"),
                new JSFunction(context, (in Arguments a) => {
                    if(a.This is JSArray @this) {
                         @this.ArrayLength = JSValueToClrConverter.ToDouble(a[0], "length");
                    }
                        else throw JSContext.Current.NewTypeError("Failed to convert this to JSArray");
                    return JSUndefined.Value;
                },
                "set length"),
                JSPropertyAttributes.ConfigurableProperty);
// Exporting At as at
prototype.FastAddValue(KeyString.at, new JSFunction(context, JSArray.At, "at" ,"function at() { [native] }", createPrototype: false, length: 1), JSPropertyAttributes.ConfigurableValue);
// Exporting Concat as concat
prototype.FastAddValue(KeyString.concat, new JSFunction(context, JSArray.Concat, "concat" ,"function concat() { [native] }", createPrototype: false, length: 1), JSPropertyAttributes.ConfigurableValue);
// Exporting Every as every
prototype.FastAddValue(KeyString.every, new JSFunction(context, JSArray.Every, "every" ,"function every() { [native] }", createPrototype: false, length: 1), JSPropertyAttributes.ConfigurableValue);
// Exporting CopyWithin as copyWithin
prototype.FastAddValue(KeyString.copyWithin, new JSFunction(context, JSArray.CopyWithin, "copyWithin" ,"function copyWithin() { [native] }", createPrototype: false, length: 2), JSPropertyAttributes.ConfigurableValue);
// Exporting Entries as entries
prototype.FastAddValue(KeyString.entries, new JSFunction(context, JSArray.Entries, "entries" ,"function entries() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting Fill as fill
prototype.FastAddValue(KeyString.fill, new JSFunction(context, JSArray.Fill, "fill" ,"function fill() { [native] }", createPrototype: false, length: 1), JSPropertyAttributes.ConfigurableValue);
// Exporting Filter as filter
prototype.FastAddValue(KeyString.filter, new JSFunction(context, JSArray.Filter, "filter" ,"function filter() { [native] }", createPrototype: false, length: 1), JSPropertyAttributes.ConfigurableValue);
// Exporting Find as find
prototype.FastAddValue(KeyString.find, new JSFunction(context, JSArray.Find, "find" ,"function find() { [native] }", createPrototype: false, length: 1), JSPropertyAttributes.ConfigurableValue);
// Exporting Flat as flat
prototype.FastAddValue(KeyString.flat, new JSFunction(context, JSArray.Flat, "flat" ,"function flat() { [native] }", createPrototype: false, length: 0), JSPropertyAttributes.ConfigurableValue);
// Exporting FlatMap as flatMap
prototype.FastAddValue(KeyString.flatMap, new JSFunction(context, JSArray.FlatMap, "flatMap" ,"function flatMap() { [native] }", createPrototype: false, length: 1), JSPropertyAttributes.ConfigurableValue);
// Exporting FindIndex as findIndex
prototype.FastAddValue(KeyString.findIndex, new JSFunction(context, JSArray.FindIndex, "findIndex" ,"function findIndex() { [native] }", createPrototype: false, length: 1), JSPropertyAttributes.ConfigurableValue);
// Exporting ForEach as forEach
prototype.FastAddValue(KeyString.forEach, new JSFunction(context, JSArray.ForEach, "forEach" ,"function forEach() { [native] }", createPrototype: false, length: 1), JSPropertyAttributes.ConfigurableValue);
// Exporting Includes as includes
prototype.FastAddValue(KeyString.includes, new JSFunction(context, JSArray.Includes, "includes" ,"function includes() { [native] }", createPrototype: false, length: 1), JSPropertyAttributes.ConfigurableValue);
// Exporting IndexOf as indexOf
prototype.FastAddValue(KeyString.indexOf, new JSFunction(context, JSArray.IndexOf, "indexOf" ,"function indexOf() { [native] }", createPrototype: false, length: 1), JSPropertyAttributes.ConfigurableValue);
// Exporting Join as join
prototype.FastAddValue(KeyString.join, new JSFunction(context, JSArray.Join, "join" ,"function join() { [native] }", createPrototype: false, length: 1), JSPropertyAttributes.ConfigurableValue);
// Exporting Keys as keys
prototype.FastAddValue(KeyString.keys, new JSFunction(context, JSArray.Keys, "keys" ,"function keys() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting LastIndexOf as lastIndexOf
prototype.FastAddValue(KeyString.lastIndexOf, new JSFunction(context, JSArray.LastIndexOf, "lastIndexOf" ,"function lastIndexOf() { [native] }", createPrototype: false, length: 1), JSPropertyAttributes.ConfigurableValue);
// Exporting Map as map
prototype.FastAddValue(KeyString.map, new JSFunction(context, JSArray.Map, "map" ,"function map() { [native] }", createPrototype: false, length: 1), JSPropertyAttributes.ConfigurableValue);
// Exporting Push as push
prototype.FastAddValue(KeyString.push, new JSFunction(context, JSArray.Push, "push" ,"function push() { [native] }", createPrototype: false, length: 1), JSPropertyAttributes.ConfigurableValue);
// Exporting Pop as pop
prototype.FastAddValue(KeyString.pop, new JSFunction(context, JSArray.Pop, "pop" ,"function pop() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting Reduce as reduce
prototype.FastAddValue(KeyString.reduce, new JSFunction(context, JSArray.Reduce, "reduce" ,"function reduce() { [native] }", createPrototype: false, length: 1), JSPropertyAttributes.ConfigurableValue);
// Exporting ReduceRight as reduceRight
prototype.FastAddValue(KeyString.reduceRight, new JSFunction(context, JSArray.ReduceRight, "reduceRight" ,"function reduceRight() { [native] }", createPrototype: false, length: 1), JSPropertyAttributes.ConfigurableValue);
// Exporting Reverse as reverse
prototype.FastAddValue(KeyString.reverse, new JSFunction(context, JSArray.Reverse, "reverse" ,"function reverse() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting Shift as shift
prototype.FastAddValue(KeyString.shift, new JSFunction(context, JSArray.Shift, "shift" ,"function shift() { [native] }", createPrototype: false, length: 0), JSPropertyAttributes.ConfigurableValue);
// Exporting Slice as slice
prototype.FastAddValue(KeyString.slice, new JSFunction(context, JSArray.Slice, "slice" ,"function slice() { [native] }", createPrototype: false, length: 2), JSPropertyAttributes.ConfigurableValue);
// Exporting Some as some
prototype.FastAddValue(KeyString.some, new JSFunction(context, JSArray.Some, "some" ,"function some() { [native] }", createPrototype: false, length: 1), JSPropertyAttributes.ConfigurableValue);
// Exporting Sort as sort
prototype.FastAddValue(KeyString.sort, new JSFunction(context, JSArray.Sort, "sort" ,"function sort() { [native] }", createPrototype: false, length: 1), JSPropertyAttributes.ConfigurableValue);
// Exporting Splice as splice
prototype.FastAddValue(KeyString.splice, new JSFunction(context, JSArray.Splice, "splice" ,"function splice() { [native] }", createPrototype: false, length: 2), JSPropertyAttributes.ConfigurableValue);
// Exporting Unshift as unshift
prototype.FastAddValue(KeyString.unshift, new JSFunction(context, JSArray.Unshift, "unshift" ,"function unshift() { [native] }", createPrototype: false, length: 1), JSPropertyAttributes.ConfigurableValue);
// Exporting ToLocaleString as toLocaleString
prototype.FastAddValue(KeyString.toLocaleString, new JSFunction(context, JSArray.ToLocaleString, "toLocaleString" ,"function toLocaleString() { [native] }", createPrototype: false, length: 0), JSPropertyAttributes.ConfigurableValue);
// Exporting ToString as toString
prototype.FastAddValue(KeyString.toString, new JSFunction(context, JSArray.ToString, "toString" ,"function toString() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting Values as values
{
var fx = new JSFunction(context, JSArray.Values, "values" ,"function values() { [native] }", createPrototype: false, length: 2);
prototype.FastAddValue(KeyString.values, fx, JSPropertyAttributes.ConfigurableValue);
prototype.FastAddValue( JSSymbol.GlobalSymbol("@@iterator"), fx, JSPropertyAttributes.ConfigurableValue);
}
// Exporting StaticFrom as from
@class.FastAddValue(KeyString.from, new JSFunction(context, JSArray.StaticFrom, "from" ,"function from() { [native] }", createPrototype: false, length: 1), JSPropertyAttributes.ConfigurableValue);
// Exporting StaticIsArray as isArray
@class.FastAddValue(KeyString.isArray, new JSFunction(context, JSArray.StaticIsArray, "isArray" ,"function isArray() { [native] }", createPrototype: false, length: 1), JSPropertyAttributes.ConfigurableValue);
// Exporting StaticOf as of
@class.FastAddValue(KeyString.of, new JSFunction(context, JSArray.StaticOf, "of" ,"function of() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
context.Array_Prototype = prototype.PrototypeObject;
return @class;
}
}
}
