using System;
using System.Collections.Generic;
using System.Text;
using Yantra.Core;
using YantraJS.Core;
using YantraJS.Core.Clr;
using YantraJS.Extensions;

namespace YantraJS.Utils
{
    [JSBaseClass("Function")]
    [JSFunctionGenerator("Assert")]
    public partial class JSAssert: JSFunction
    {
        [JSExport(IsConstructor = true)]
        public static JSValue Assert(in Arguments args)
        {
            var (test, message) = args.Get2();
            if (!test.BooleanValue)
            {
                message = message.IsUndefined ? new JSString($"Assert failed, no message, {test}") : message;
                throw new JSException(message);
            }
            return JSUndefined.Value;
        }

        [JSExport("strictEqual")]
        public static JSValue StrictEqual(in Arguments a)
        {
            var (left, right, msgObj) = a.Get3();
            if (!left.StrictEquals(right))
            {
                var msg = !(msgObj.IsUndefined) ? msgObj.ToString() : $"Values {left},{right} are not same";
                throw new JSException(msg);
            }
            return JSUndefined.Value;
        }

        [JSExport("notStrictEqual")]
        public static JSValue NotStrictEqual(in Arguments a)
        {
            var (left, right, msgObj) = a.Get3();
            if (left.StrictEquals(right))
            {
                var msg = !(msgObj.IsUndefined) ? msgObj.ToString() : $"Values {left},{right} are same";
                throw new JSException(msg);
            }
            return JSUndefined.Value;
        }

        [JSExport("equal")]
        public static JSValue Equal(in Arguments a)
        {
            var (left, right, msgObj) = a.Get3();
            if (!left.Equals(right))
            {
                var msg = !(msgObj.IsUndefined) ? msgObj.ToString() : $"Values {left},{right} are not same";
                throw new JSException(msg);
            }
            return JSUndefined.Value;
        }

        [JSExport("doubleEqual")]
        public static JSValue DoubleEqual(in Arguments a)
        {
            var (left, right, msgObj) = a.Get3();
            var diff = Math.Abs(left.DoubleValue - right.DoubleValue);
            // var leftDiff = 
            var marginalDiff = Math.Max(0.0001, Math.Max(left.DoubleValue, right.DoubleValue)/ 1000);
            if (!(diff < marginalDiff))
            {
                var msg = !(msgObj.IsUndefined) ? msgObj.ToString() : $"Values {left},{right} are not same difference is {diff}";
                throw new JSException(msg);
            }
            return JSUndefined.Value;
        }


        [JSExport("notEqual")]
        public static JSValue NotEqual(in Arguments a)
        {
            var (left, right, msgObj) = a.Get3();
            if (left.Equals(right))
            {
                var msg = !(msgObj.IsUndefined) ? msgObj.ToString() : $"Values {left},{right} are same";
                throw new JSException(msg);
            }
            return JSUndefined.Value;
        }

        [JSExport("throws")]
        public static JSValue Throws(in Arguments a)
        {
            var (left, error, msgObj) = a.Get3();
            if (!left.IsFunction)
                throw new JSException("assert.throws expect first parameter to be Function");
            try
            {
                left.InvokeFunction(a);
            }
            catch (Exception ex)
            {
                if (!error.IsUndefined)
                {
                    var errorText = error.ToString();
                    if (errorText != ex.Message)
                    {
                        var msg1 = !(msgObj.IsUndefined) ? msgObj.ToString() : $"Error was expected {errorText} but received {ex.Message}";
                        throw new JSException(msg1);
                    }
                }
                return JSUndefined.Value;
            }
            var msg = !(msgObj.IsUndefined) ? msgObj.ToString() : $"Function fn did not throw any error";
            throw new JSException(msg);
        }

        [JSExport("fail")]
        public static JSValue Fail(in Arguments a)
        {
            var msgObj = a.Get1();
            var msg = !(msgObj.IsUndefined) ? msgObj.ToString() : $"Failed";
            throw new JSException(msg);
        }

        [JSExport("match")]
        public static JSValue Match(in Arguments a)
        {
            var (text, regex, msgObj) = a.Get3();
            if(!(regex is JSRegExp match))
            {
                throw new JSException($"Second parameter must be regex");
            }
            if (!match.value.IsMatch(text.ToString())) {
                var msg = !(msgObj.IsUndefined) ? msgObj.ToString() : $"Regex match failed of {text} with {regex}";
                throw new JSException(msg);
            }
            return JSUndefined.Value;
        }

    }
}
