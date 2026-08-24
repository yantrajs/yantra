using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using YantraJS.Core.Clr;
using YantraJS.Extensions;
using YantraJS.Core;
namespace YantraJS.Core.Core.DataView { 
partial class DataView {
public static JSFunction CreateClass(JSContext context, bool register = true) {

                        JSObject prototype = null;
                        var @class = new JSClassFunction(context, (in Arguments a) => new DataView(in a)
                            , "DataView"
                            , "function DataView() { [native code] }"
                            , length:3);
                        if (register) {
                            context["DataView".ToKeyString()] = @class;
                        }
                        prototype = @class.prototype;
                        
// Exporting Buffer as buffer
prototype.FastAddProperty(
                "buffer".ToKeyString(),
                new JSFunction(context, (in Arguments a) =>
                    a.This is DataView @this
                        ? @this.Buffer
                        : throw JSContext.Current.NewTypeError("Failed to convert this to DataView") ,
                "get buffer"),
                null,
                JSPropertyAttributes.ConfigurableProperty);
// Exporting ByteLength as byteLength
prototype.FastAddProperty(
                "byteLength".ToKeyString(),
                new JSFunction(context, (in Arguments a) =>
                    a.This is DataView @this
                        ? ClrProxy.Marshal(@this.ByteLength)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to DataView") ,
                "get byteLength"),
                null,
                JSPropertyAttributes.ConfigurableProperty);
// Exporting ByteOffset as byteOffset
prototype.FastAddProperty(
                "byteOffset".ToKeyString(),
                new JSFunction(context, (in Arguments a) =>
                    a.This is DataView @this
                        ? ClrProxy.Marshal(@this.ByteOffset)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to DataView") ,
                "get byteOffset"),
                null,
                JSPropertyAttributes.ConfigurableProperty);
// Exporting GetBigInt64 as getBigInt64
prototype.FastAddValue("getBigInt64".ToKeyString(), new JSFunction(context, (in Arguments a) =>
                    a.This is DataView @this
                        ? @this.GetBigInt64(in a)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to DataView")
                        , "getBigInt64"
                        ,"function getBigInt64() { [native] }", createPrototype: false, length: 1), JSPropertyAttributes.ConfigurableValue);
// Exporting GetBigUInt64 as getBigUInt64
prototype.FastAddValue("getBigUInt64".ToKeyString(), new JSFunction(context, (in Arguments a) =>
                    a.This is DataView @this
                        ? @this.GetBigUInt64(in a)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to DataView")
                        , "getBigUInt64"
                        ,"function getBigUInt64() { [native] }", createPrototype: false, length: 1), JSPropertyAttributes.ConfigurableValue);
// Exporting GetFloat32 as getFloat32
prototype.FastAddValue("getFloat32".ToKeyString(), new JSFunction(context, (in Arguments a) =>
                    a.This is DataView @this
                        ? @this.GetFloat32(in a)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to DataView")
                        , "getFloat32"
                        ,"function getFloat32() { [native] }", createPrototype: false, length: 2), JSPropertyAttributes.ConfigurableValue);
// Exporting GetFloat64 as getFloat64
prototype.FastAddValue("getFloat64".ToKeyString(), new JSFunction(context, (in Arguments a) =>
                    a.This is DataView @this
                        ? @this.GetFloat64(in a)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to DataView")
                        , "getFloat64"
                        ,"function getFloat64() { [native] }", createPrototype: false, length: 2), JSPropertyAttributes.ConfigurableValue);
// Exporting GetInt16 as getInt16
prototype.FastAddValue("getInt16".ToKeyString(), new JSFunction(context, (in Arguments a) =>
                    a.This is DataView @this
                        ? @this.GetInt16(in a)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to DataView")
                        , "getInt16"
                        ,"function getInt16() { [native] }", createPrototype: false, length: 2), JSPropertyAttributes.ConfigurableValue);
// Exporting GetInt32 as getInt32
prototype.FastAddValue("getInt32".ToKeyString(), new JSFunction(context, (in Arguments a) =>
                    a.This is DataView @this
                        ? @this.GetInt32(in a)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to DataView")
                        , "getInt32"
                        ,"function getInt32() { [native] }", createPrototype: false, length: 2), JSPropertyAttributes.ConfigurableValue);
// Exporting GetInt8 as getInt8
prototype.FastAddValue("getInt8".ToKeyString(), new JSFunction(context, (in Arguments a) =>
                    a.This is DataView @this
                        ? @this.GetInt8(in a)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to DataView")
                        , "getInt8"
                        ,"function getInt8() { [native] }", createPrototype: false, length: 1), JSPropertyAttributes.ConfigurableValue);
// Exporting GetUint16 as getUint16
prototype.FastAddValue("getUint16".ToKeyString(), new JSFunction(context, (in Arguments a) =>
                    a.This is DataView @this
                        ? @this.GetUint16(in a)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to DataView")
                        , "getUint16"
                        ,"function getUint16() { [native] }", createPrototype: false, length: 2), JSPropertyAttributes.ConfigurableValue);
// Exporting GetUint32 as getUint32
prototype.FastAddValue("getUint32".ToKeyString(), new JSFunction(context, (in Arguments a) =>
                    a.This is DataView @this
                        ? @this.GetUint32(in a)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to DataView")
                        , "getUint32"
                        ,"function getUint32() { [native] }", createPrototype: false, length: 2), JSPropertyAttributes.ConfigurableValue);
// Exporting GetUint8 as getUint8
prototype.FastAddValue("getUint8".ToKeyString(), new JSFunction(context, (in Arguments a) =>
                    a.This is DataView @this
                        ? @this.GetUint8(in a)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to DataView")
                        , "getUint8"
                        ,"function getUint8() { [native] }", createPrototype: false, length: 1), JSPropertyAttributes.ConfigurableValue);
// Exporting SetBigInt64 as setBigInt64
prototype.FastAddValue("setBigInt64".ToKeyString(), new JSFunction(context, (in Arguments a) =>
                    a.This is DataView @this
                        ? @this.SetBigInt64(in a)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to DataView")
                        , "setBigInt64"
                        ,"function setBigInt64() { [native] }", createPrototype: false, length: 2), JSPropertyAttributes.ConfigurableValue);
// Exporting SetBigUInt64 as setBigUInt64
prototype.FastAddValue("setBigUInt64".ToKeyString(), new JSFunction(context, (in Arguments a) =>
                    a.This is DataView @this
                        ? @this.SetBigUInt64(in a)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to DataView")
                        , "setBigUInt64"
                        ,"function setBigUInt64() { [native] }", createPrototype: false, length: 2), JSPropertyAttributes.ConfigurableValue);
// Exporting SetFloat32 as setFloat32
prototype.FastAddValue("setFloat32".ToKeyString(), new JSFunction(context, (in Arguments a) =>
                    a.This is DataView @this
                        ? @this.SetFloat32(in a)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to DataView")
                        , "setFloat32"
                        ,"function setFloat32() { [native] }", createPrototype: false, length: 3), JSPropertyAttributes.ConfigurableValue);
// Exporting SetFloat64 as setFloat64
prototype.FastAddValue("setFloat64".ToKeyString(), new JSFunction(context, (in Arguments a) =>
                    a.This is DataView @this
                        ? @this.SetFloat64(in a)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to DataView")
                        , "setFloat64"
                        ,"function setFloat64() { [native] }", createPrototype: false, length: 3), JSPropertyAttributes.ConfigurableValue);
// Exporting SetInt16 as setInt16
prototype.FastAddValue("setInt16".ToKeyString(), new JSFunction(context, (in Arguments a) =>
                    a.This is DataView @this
                        ? @this.SetInt16(in a)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to DataView")
                        , "setInt16"
                        ,"function setInt16() { [native] }", createPrototype: false, length: 3), JSPropertyAttributes.ConfigurableValue);
// Exporting SetInt32 as setInt32
prototype.FastAddValue("setInt32".ToKeyString(), new JSFunction(context, (in Arguments a) =>
                    a.This is DataView @this
                        ? @this.SetInt32(in a)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to DataView")
                        , "setInt32"
                        ,"function setInt32() { [native] }", createPrototype: false, length: 3), JSPropertyAttributes.ConfigurableValue);
// Exporting SetInt8 as setInt8
prototype.FastAddValue("setInt8".ToKeyString(), new JSFunction(context, (in Arguments a) =>
                    a.This is DataView @this
                        ? @this.SetInt8(in a)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to DataView")
                        , "setInt8"
                        ,"function setInt8() { [native] }", createPrototype: false, length: 2), JSPropertyAttributes.ConfigurableValue);
// Exporting SetUint16 as setUint16
prototype.FastAddValue("setUint16".ToKeyString(), new JSFunction(context, (in Arguments a) =>
                    a.This is DataView @this
                        ? @this.SetUint16(in a)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to DataView")
                        , "setUint16"
                        ,"function setUint16() { [native] }", createPrototype: false, length: 3), JSPropertyAttributes.ConfigurableValue);
// Exporting SetUint32 as setUint32
prototype.FastAddValue("setUint32".ToKeyString(), new JSFunction(context, (in Arguments a) =>
                    a.This is DataView @this
                        ? @this.SetUint32(in a)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to DataView")
                        , "setUint32"
                        ,"function setUint32() { [native] }", createPrototype: false, length: 3), JSPropertyAttributes.ConfigurableValue);
// Exporting SetUint8 as setUint8
prototype.FastAddValue("setUint8".ToKeyString(), new JSFunction(context, (in Arguments a) =>
                    a.This is DataView @this
                        ? @this.SetUint8(in a)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to DataView")
                        , "setUint8"
                        ,"function setUint8() { [native] }", createPrototype: false, length: 2), JSPropertyAttributes.ConfigurableValue);
return @class;
}
}
}
