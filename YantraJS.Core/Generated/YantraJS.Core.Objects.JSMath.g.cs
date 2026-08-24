using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using YantraJS.Core.Clr;
using YantraJS.Extensions;
using YantraJS.Core;
namespace YantraJS.Core.Objects { 
partial class JSMath {
public static JSObject CreateClass(JSContext context, bool register = true) {

                    var @class = new JSObject();
                    if (register) {
                        context[KeyString.Math] = @class;
                    }
                
// Exporting E as E
@class.FastAddValue(
                KeyString.E,
                ClrProxy.Marshal(JSMath.E),
                JSPropertyAttributes.ReadonlyValue);
// Exporting LN10 as LN10
@class.FastAddValue(
                KeyString.LN10,
                ClrProxy.Marshal(JSMath.LN10),
                JSPropertyAttributes.ReadonlyValue);
// Exporting LN2 as LN2
@class.FastAddValue(
                KeyString.LN2,
                ClrProxy.Marshal(JSMath.LN2),
                JSPropertyAttributes.ReadonlyValue);
// Exporting LOG10E as LOG10E
@class.FastAddValue(
                KeyString.LOG10E,
                ClrProxy.Marshal(JSMath.LOG10E),
                JSPropertyAttributes.ReadonlyValue);
// Exporting LOG2E as LOG2E
@class.FastAddValue(
                KeyString.LOG2E,
                ClrProxy.Marshal(JSMath.LOG2E),
                JSPropertyAttributes.ReadonlyValue);
// Exporting PI as PI
@class.FastAddValue(
                KeyString.PI,
                ClrProxy.Marshal(JSMath.PI),
                JSPropertyAttributes.ReadonlyValue);
// Exporting SQRT1_2 as SQRT1_2
@class.FastAddValue(
                KeyString.SQRT1_2,
                ClrProxy.Marshal(JSMath.SQRT1_2),
                JSPropertyAttributes.ReadonlyValue);
// Exporting SQRT2 as SQRT2
@class.FastAddValue(
                KeyString.SQRT2,
                ClrProxy.Marshal(JSMath.SQRT2),
                JSPropertyAttributes.ReadonlyValue);
// Exporting Random as random
@class.FastAddValue(KeyString.random, new JSFunction(context, JSMath.Random, "random" ,"function random() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting Round as round
@class.FastAddValue(KeyString.round, new JSFunction(context, JSMath.Round, "round" ,"function round() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting Floor as floor
@class.FastAddValue(KeyString.floor, new JSFunction(context, JSMath.Floor, "floor" ,"function floor() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting Acos as acos
@class.FastAddValue(KeyString.acos, new JSFunction(context, JSMath.Acos, "acos" ,"function acos() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting Abs as abs
@class.FastAddValue(KeyString.abs, new JSFunction(context, JSMath.Abs, "abs" ,"function abs() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting Acosh as acosh
@class.FastAddValue(KeyString.acosh, new JSFunction(context, JSMath.Acosh, "acosh" ,"function acosh() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting Asin as asin
@class.FastAddValue(KeyString.asin, new JSFunction(context, JSMath.Asin, "asin" ,"function asin() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting Asinh as asinh
@class.FastAddValue(KeyString.asinh, new JSFunction(context, JSMath.Asinh, "asinh" ,"function asinh() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting Atan as atan
@class.FastAddValue(KeyString.atan, new JSFunction(context, JSMath.Atan, "atan" ,"function atan() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting Atan2 as atan2
@class.FastAddValue(KeyString.atan2, new JSFunction(context, JSMath.Atan2, "atan2" ,"function atan2() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting Atanh as atanh
@class.FastAddValue(KeyString.atanh, new JSFunction(context, JSMath.Atanh, "atanh" ,"function atanh() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting Cbrt as cbrt
@class.FastAddValue(KeyString.cbrt, new JSFunction(context, JSMath.Cbrt, "cbrt" ,"function cbrt() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting Ceil as ceil
@class.FastAddValue(KeyString.ceil, new JSFunction(context, JSMath.Ceil, "ceil" ,"function ceil() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting Clz32 as clz32
@class.FastAddValue(KeyString.clz32, new JSFunction(context, JSMath.Clz32, "clz32" ,"function clz32() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting Cos as cos
@class.FastAddValue(KeyString.cos, new JSFunction(context, JSMath.Cos, "cos" ,"function cos() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting Cosh as cosh
@class.FastAddValue(KeyString.cosh, new JSFunction(context, JSMath.Cosh, "cosh" ,"function cosh() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting Exp as exp
@class.FastAddValue(KeyString.exp, new JSFunction(context, JSMath.Exp, "exp" ,"function exp() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting Expm1 as expm1
@class.FastAddValue(KeyString.expm1, new JSFunction(context, JSMath.Expm1, "expm1" ,"function expm1() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting Fround as fround
@class.FastAddValue(KeyString.fround, new JSFunction(context, JSMath.Fround, "fround" ,"function fround() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting Hypot as hypot
@class.FastAddValue(KeyString.hypot, new JSFunction(context, JSMath.Hypot, "hypot" ,"function hypot() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting Imul as imul
@class.FastAddValue(KeyString.imul, new JSFunction(context, JSMath.Imul, "imul" ,"function imul() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting Log as log
@class.FastAddValue(KeyString.log, new JSFunction(context, JSMath.Log, "log" ,"function log() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting Log10 as log10
@class.FastAddValue(KeyString.log10, new JSFunction(context, JSMath.Log10, "log10" ,"function log10() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting Log1p as log1p
@class.FastAddValue(KeyString.log1p, new JSFunction(context, JSMath.Log1p, "log1p" ,"function log1p() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting Log2 as log2
@class.FastAddValue(KeyString.log2, new JSFunction(context, JSMath.Log2, "log2" ,"function log2() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting Max as max
@class.FastAddValue(KeyString.max, new JSFunction(context, JSMath.Max, "max" ,"function max() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting Min as min
@class.FastAddValue(KeyString.min, new JSFunction(context, JSMath.Min, "min" ,"function min() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting Pow as pow
@class.FastAddValue(KeyString.pow, new JSFunction(context, JSMath.Pow, "pow" ,"function pow() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting Sign as sign
@class.FastAddValue(KeyString.sign, new JSFunction(context, JSMath.Sign, "sign" ,"function sign() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting Sin as sin
@class.FastAddValue(KeyString.sin, new JSFunction(context, JSMath.Sin, "sin" ,"function sin() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting Sinh as sinh
@class.FastAddValue(KeyString.sinh, new JSFunction(context, JSMath.Sinh, "sinh" ,"function sinh() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting Sqrt as sqrt
@class.FastAddValue(KeyString.sqrt, new JSFunction(context, JSMath.Sqrt, "sqrt" ,"function sqrt() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting Tan as tan
@class.FastAddValue(KeyString.tan, new JSFunction(context, JSMath.Tan, "tan" ,"function tan() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting Tanh as tanh
@class.FastAddValue(KeyString.tanh, new JSFunction(context, JSMath.Tanh, "tanh" ,"function tanh() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting Trunc as trunc
@class.FastAddValue(KeyString.trunc, new JSFunction(context, JSMath.Trunc, "trunc" ,"function trunc() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
return @class;
}
}
}
