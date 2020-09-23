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
        public static JSValue StrictEqual(JSValue t, params JSValue[] a)
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
        public static JSValue NotStrictEqual(JSValue t, params JSValue[] a)
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
        public static JSValue Equal(JSValue t, params JSValue[] a)
        {
            var (left, right, msgObj) = a.Get3();
            if (!left.Equals(right).BooleanValue)
            {
                var msg = !(msgObj.IsUndefined) ? msgObj.ToString() : $"Values {left},{right} are not same";
                throw new JSException(msg);
            }
            return JSUndefined.Value;
        }

        [Prototype("notEqual")]
        public static JSValue NotEqual(JSValue t, params JSValue[] a)
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
        public static JSValue Throws(JSValue t, params JSValue[] a)
        {
            var (left, error, msgObj) = a.Get3();
            if (!left.IsFunction)
                throw new JSException("assert.throws expect first parameter to be Function");
            try
            {
                left.InvokeFunction(t);
                var msg = !(msgObj.IsUndefined) ? msgObj.ToString() : $"Function fn did not throw any error";
                throw new JSException(msg);
            }
            catch (Exception ex)
            {
                if (!error.IsUndefined)
                {
                    var errorText = error.ToString();
                    if (errorText != ex.Message)
                    {
                        var msg = !(msgObj.IsUndefined) ? msgObj.ToString() : $"Error was expected {errorText} but received {ex.Message}";
                        throw new JSException(msg);
                    }
                }
            }
            return JSUndefined.Value;
        }

        [Prototype("fail")]
        public static JSValue Fail(JSValue t, params JSValue[] a)
        {
            var msgObj = a.Get1();
            var msg = !(msgObj.IsUndefined) ? msgObj.ToString() : $"Failed";
            throw new JSException(msg);
        }

        [Prototype("match")]
        public static JSValue Match(JSValue t, params JSValue[] a)
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
