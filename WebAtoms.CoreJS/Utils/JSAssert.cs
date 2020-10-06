using System;
using System.Collections.Generic;
using System.Text;
using WebAtoms.CoreJS.Core;
using WebAtoms.CoreJS.Extensions;

namespace WebAtoms.CoreJS.Utils
{
    public class JSAssert: JSObject
    {
        [Prototype("strictEqual")]
        public static JSValue StrictEqual(in Arguments a)
        {
            var (left, right, msgObj) = a.Get3();
            if (!left.StrictEquals(right).BooleanValue)
            {
                var msg = !(msgObj.IsUndefined) ? msgObj.ToString() : $"Values {left},{right} are not same";
                throw new JSException(msg);
            }
            return JSUndefined.Value;
        }

        [Prototype("notStrictEqual")]
        public static JSValue NotStrictEqual(in Arguments a)
        {
            var (left, right, msgObj) = a.Get3();
            if (left.StrictEquals(right).BooleanValue)
            {
                var msg = !(msgObj.IsUndefined) ? msgObj.ToString() : $"Values {left},{right} are same";
                throw new JSException(msg);
            }
            return JSUndefined.Value;
        }

        [Prototype("equal")]
        public static JSValue Equal(in Arguments a)
        {
            var (left, right, msgObj) = a.Get3();
            if (!left.Equals(right).BooleanValue)
            {
                var msg = !(msgObj.IsUndefined) ? msgObj.ToString() : $"Values {left},{right} are not same";
                throw new JSException(msg);
            }
            return JSUndefined.Value;
        }

        [Prototype("doubleEqual")]
        public static JSValue DoubleEqual(in Arguments a)
        {
            var (left, right, msgObj) = a.Get3();
            if (!(Math.Abs(left.DoubleValue - right.DoubleValue) < 0.0001))
            {
                var msg = !(msgObj.IsUndefined) ? msgObj.ToString() : $"Values {left},{right} are not same";
                throw new JSException(msg);
            }
            return JSUndefined.Value;
        }


        [Prototype("notEqual")]
        public static JSValue NotEqual(in Arguments a)
        {
            var (left, right, msgObj) = a.Get3();
            if (left.Equals(right).BooleanValue)
            {
                var msg = !(msgObj.IsUndefined) ? msgObj.ToString() : $"Values {left},{right} are same";
                throw new JSException(msg);
            }
            return JSUndefined.Value;
        }

        [Prototype("throws")]
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

        [Prototype("fail")]
        public static JSValue Fail(in Arguments a)
        {
            var msgObj = a.Get1();
            var msg = !(msgObj.IsUndefined) ? msgObj.ToString() : $"Failed";
            throw new JSException(msg);
        }

        [Prototype("match")]
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
