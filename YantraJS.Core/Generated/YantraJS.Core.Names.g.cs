using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using YantraJS.Core.Clr;
using YantraJS.Extensions;
namespace YantraJS.Core { 
partial class Names {
static Names() {
}
static private void RegisterAll(JSContext context) {
YantraJS.Core.JSArray.CreateClass(context);
YantraJS.Core.Typed.JSArrayBuffer.CreateClass(context);
YantraJS.Core.Typed.JSTypedArray.CreateClass(context);
YantraJS.Core.Typed.JSUInt16Array.CreateClass(context);
YantraJS.Core.Typed.JSUInt32Array.CreateClass(context);
YantraJS.Core.Typed.JSUInt8Array.CreateClass(context);
YantraJS.Core.Typed.JSUint8ClampedArray.CreateClass(context);
YantraJS.Core.BigInt.JSBigInt.CreateClass(context);
YantraJS.Core.JSBoolean.CreateClass(context);
YantraJS.Core.Core.DataView.DataView.CreateClass(context);
YantraJS.Core.JSDate.CreateClass(context);
YantraJS.Core.JSDecimal.CreateClass(context);
YantraJS.Core.JSError.CreateClass(context);
YantraJS.Core.JSTypeError.CreateClass(context);
YantraJS.Core.JSSyntaxError.CreateClass(context);
YantraJS.Core.JSURIError.CreateClass(context);
YantraJS.Core.JSRangeError.CreateClass(context);
YantraJS.Core.JSEvalError.CreateClass(context);
Yantra.Core.Events.EventTarget.CreateClass(context);
YantraJS.Core.Generator.JSGenerator.CreateClass(context);
YantraJS.Core.JSGlobalStatic.CreateClass(context);
YantraJS.Core.JSJSON.CreateClass(context);
YantraJS.Core.JSMap.CreateClass(context);
YantraJS.Core.JSWeakMap.CreateClass(context);
YantraJS.Core.JSNumber.CreateClass(context);
YantraJS.Core.Objects.JSMath.CreateClass(context);
YantraJS.Core.Objects.JSReflect.CreateClass(context);
YantraJS.Core.JSPromise.CreateClass(context);
YantraJS.Core.JSProxy.CreateClass(context);
YantraJS.Core.JSRegExp.CreateClass(context);
YantraJS.Core.Set.JSSet.CreateClass(context);
YantraJS.Core.Set.JSWeakSet.CreateClass(context);
YantraJS.Core.JSString.CreateClass(context);
YantraJS.Core.JSSymbol.CreateClass(context);
YantraJS.Core.Weak.JSFinalizationRegistry.CreateClass(context);
YantraJS.Core.Weak.JSWeakRef.CreateClass(context);
YantraJS.Utils.JSAssert.CreateClass(context);
YantraJS.Core.Typed.JSFloat32Array.CreateClass(context);
YantraJS.Core.Typed.JSFloat64Array.CreateClass(context);
YantraJS.Core.Typed.JSInt16Array.CreateClass(context);
YantraJS.Core.Typed.JSInt32Array.CreateClass(context);
YantraJS.Core.Typed.JSInt8Array.CreateClass(context);
YantraJS.Core.Core.Error.JSSuppressedError.CreateClass(context);
}
}
}
