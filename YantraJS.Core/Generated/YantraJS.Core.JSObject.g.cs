using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using YantraJS.Core.Clr;
using YantraJS.Extensions;
namespace YantraJS.Core { 
partial class JSObject {
public static new JSFunction CreateClass(JSContext context, bool register = true) {

                        JSObject prototype = null;
                        var @class = new JSFunction(context, (in Arguments a) => JSObject.Constructor(in a)
                            , "Object"
                            , "function Object() { [native code] }"
                            );
                        if (register) {
                            context["Object".ToKeyString()] = @class;
                        }
                        prototype = @class.prototype;
                        
// Exporting PropertyIsEnumerable as propertyIsEnumerable
prototype.FastAddValue("propertyIsEnumerable".ToKeyString(), new JSFunction(context, JSObject.PropertyIsEnumerable, "propertyIsEnumerable" ,"function propertyIsEnumerable() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting ToString as toString
prototype.FastAddValue("toString".ToKeyString(), new JSFunction(context, JSObject.ToString, "toString" ,"function toString() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting ObjectPrototype as __proto__
prototype.FastAddProperty(
                "__proto__".ToKeyString(),
                new JSFunction(context, (in Arguments a) =>
                    a.This is JSObject @this
                        ? @this.ObjectPrototype
                        : throw JSContext.Current.NewTypeError("Failed to convert this to JSObject") ,
                "get __proto__"),
                new JSFunction(context, (in Arguments a) => {
                    if(a.This is JSObject @this) {
                         @this.ObjectPrototype = a[0];
                    }
                        else throw JSContext.Current.NewTypeError("Failed to convert this to JSObject");
                    return JSUndefined.Value;
                },
                "set __proto__"),
                JSPropertyAttributes.ConfigurableProperty);
// Exporting HasOwnProperty as hasOwnProperty
prototype.FastAddValue("hasOwnProperty".ToKeyString(), new JSFunction(context, JSObject.HasOwnProperty, "hasOwnProperty" ,"function hasOwnProperty() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting ValueOf as valueOf
prototype.FastAddValue("valueOf".ToKeyString(), new JSFunction(context, JSObject.ValueOf, "valueOf" ,"function valueOf() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting IsPrototypeOf as isPrototypeOf
prototype.FastAddValue("isPrototypeOf".ToKeyString(), new JSFunction(context, JSObject.IsPrototypeOf, "isPrototypeOf" ,"function isPrototypeOf() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting StaticCreate as create
@class.FastAddValue("create".ToKeyString(), new JSFunction(context, JSObject.StaticCreate, "create" ,"function create() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting Assign as assign
@class.FastAddValue("assign".ToKeyString(), new JSFunction(context, JSObject.Assign, "assign" ,"function assign() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting StaticEntries as entries
@class.FastAddValue("entries".ToKeyString(), new JSFunction(context, JSObject.StaticEntries, "entries" ,"function entries() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting Freeze as freeze
@class.FastAddValue("freeze".ToKeyString(), new JSFunction(context, JSObject.Freeze, "freeze" ,"function freeze() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting DefineProperties as defineProperties
@class.FastAddValue("defineProperties".ToKeyString(), new JSFunction(context, JSObject.DefineProperties, "defineProperties" ,"function defineProperties() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting DefineProperty as defineProperty
@class.FastAddValue("defineProperty".ToKeyString(), new JSFunction(context, JSObject.DefineProperty, "defineProperty" ,"function defineProperty() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting GetEntries as entries
@class.FastAddValue("entries".ToKeyString(), new JSFunction(context, JSObject.GetEntries, "entries" ,"function entries() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting FromEntries as fromEntries
@class.FastAddValue("fromEntries".ToKeyString(), new JSFunction(context, JSObject.FromEntries, "fromEntries" ,"function fromEntries() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting Is as is
@class.FastAddValue("is".ToKeyString(), new JSFunction(context, JSObject.Is, "is" ,"function is() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting IsExtensible as isExtensible
@class.FastAddValue("isExtensible".ToKeyString(), new JSFunction(context, JSObject.IsExtensible, "isExtensible" ,"function isExtensible() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting IsFrozen as isFrozen
@class.FastAddValue("isFrozen".ToKeyString(), new JSFunction(context, JSObject.IsFrozen, "isFrozen" ,"function isFrozen() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting IsSealed as isSealed
@class.FastAddValue("isSealed".ToKeyString(), new JSFunction(context, JSObject.IsSealed, "isSealed" ,"function isSealed() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting Keys as keys
@class.FastAddValue("keys".ToKeyString(), new JSFunction(context, JSObject.Keys, "keys" ,"function keys() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting PreventExtensions as preventExtensions
@class.FastAddValue("preventExtensions".ToKeyString(), new JSFunction(context, JSObject.PreventExtensions, "preventExtensions" ,"function preventExtensions() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting Seal as seal
@class.FastAddValue("seal".ToKeyString(), new JSFunction(context, JSObject.Seal, "seal" ,"function seal() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting SetPrototypeOf as setPrototypeOf
@class.FastAddValue("setPrototypeOf".ToKeyString(), new JSFunction(context, JSObject.SetPrototypeOf, "setPrototypeOf" ,"function setPrototypeOf() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting Values as values
@class.FastAddValue("values".ToKeyString(), new JSFunction(context, JSObject.Values, "values" ,"function values() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting GetOwnPropertyDescriptor as getOwnPropertyDescriptor
@class.FastAddValue("getOwnPropertyDescriptor".ToKeyString(), new JSFunction(context, JSObject.GetOwnPropertyDescriptor, "getOwnPropertyDescriptor" ,"function getOwnPropertyDescriptor() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting GetOwnPropertyDescriptors as getOwnPropertyDescriptors
@class.FastAddValue("getOwnPropertyDescriptors".ToKeyString(), new JSFunction(context, JSObject.GetOwnPropertyDescriptors, "getOwnPropertyDescriptors" ,"function getOwnPropertyDescriptors() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting GetOwnPropertyNames as getOwnPropertyNames
@class.FastAddValue("getOwnPropertyNames".ToKeyString(), new JSFunction(context, JSObject.GetOwnPropertyNames, "getOwnPropertyNames" ,"function getOwnPropertyNames() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting GetOwnPropertySymbols as getOwnPropertySymbols
@class.FastAddValue("getOwnPropertySymbols".ToKeyString(), new JSFunction(context, JSObject.GetOwnPropertySymbols, "getOwnPropertySymbols" ,"function getOwnPropertySymbols() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting GetPrototypeOf as getPrototypeOf
@class.FastAddValue("getPrototypeOf".ToKeyString(), new JSFunction(context, JSObject.GetPrototypeOf, "getPrototypeOf" ,"function getPrototypeOf() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
return @class;
}
}
}
