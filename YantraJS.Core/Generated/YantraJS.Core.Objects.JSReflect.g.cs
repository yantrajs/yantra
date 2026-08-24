using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using YantraJS.Core.Clr;
using YantraJS.Extensions;
using YantraJS.Core;
namespace YantraJS.Core.Objects { 
partial class JSReflect {
public static JSObject CreateClass(JSContext context, bool register = true) {

                    var @class = new JSObject();
                    if (register) {
                        context[KeyString.Reflect] = @class;
                    }
                
// Exporting Apply as apply
@class.FastAddValue(KeyString.apply, new JSFunction(context, JSReflect.Apply, "apply" ,"function apply() { [native] }", createPrototype: false, length: 3), JSPropertyAttributes.ConfigurableValue);
// Exporting Construct as construct
@class.FastAddValue(KeyString.construct, new JSFunction(context, JSReflect.Construct, "construct" ,"function construct() { [native] }", createPrototype: false, length: 2), JSPropertyAttributes.ConfigurableValue);
// Exporting DefineProperty as defineProperty
@class.FastAddValue(KeyString.defineProperty, new JSFunction(context, JSReflect.DefineProperty, "defineProperty" ,"function defineProperty() { [native] }", createPrototype: false, length: 3), JSPropertyAttributes.ConfigurableValue);
// Exporting DeleteProperty as deleteProperty
@class.FastAddValue(KeyString.deleteProperty, new JSFunction(context, JSReflect.DeleteProperty, "deleteProperty" ,"function deleteProperty() { [native] }", createPrototype: false, length: 2), JSPropertyAttributes.ConfigurableValue);
// Exporting Get as get
@class.FastAddValue(KeyString.get, new JSFunction(context, JSReflect.Get, "get" ,"function get() { [native] }", createPrototype: false, length: 2), JSPropertyAttributes.ConfigurableValue);
// Exporting GetOwnPropertyDescriptor as getOwnPropertyDescriptor
@class.FastAddValue(KeyString.getOwnPropertyDescriptor, new JSFunction(context, JSReflect.GetOwnPropertyDescriptor, "getOwnPropertyDescriptor" ,"function getOwnPropertyDescriptor() { [native] }", createPrototype: false, length: 2), JSPropertyAttributes.ConfigurableValue);
// Exporting Has as has
@class.FastAddValue(KeyString.has, new JSFunction(context, JSReflect.Has, "has" ,"function has() { [native] }", createPrototype: false, length: 2), JSPropertyAttributes.ConfigurableValue);
// Exporting IsExtensible as isExtensible
@class.FastAddValue(KeyString.isExtensible, new JSFunction(context, JSReflect.IsExtensible, "isExtensible" ,"function isExtensible() { [native] }", createPrototype: false, length: 1), JSPropertyAttributes.ConfigurableValue);
// Exporting OwnKeys as ownKeys
@class.FastAddValue(KeyString.ownKeys, new JSFunction(context, JSReflect.OwnKeys, "ownKeys" ,"function ownKeys() { [native] }", createPrototype: false, length: 1), JSPropertyAttributes.ConfigurableValue);
// Exporting PreventExtensions as preventExtensions
@class.FastAddValue(KeyString.preventExtensions, new JSFunction(context, JSReflect.PreventExtensions, "preventExtensions" ,"function preventExtensions() { [native] }", createPrototype: false, length: 1), JSPropertyAttributes.ConfigurableValue);
// Exporting Set as set
@class.FastAddValue(KeyString.set, new JSFunction(context, JSReflect.Set, "set" ,"function set() { [native] }", createPrototype: false, length: 2), JSPropertyAttributes.ConfigurableValue);
// Exporting SetPrototypeOf as setPrototypeOf
@class.FastAddValue(KeyString.setPrototypeOf, new JSFunction(context, JSReflect.SetPrototypeOf, "setPrototypeOf" ,"function setPrototypeOf() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
return @class;
}
}
}
