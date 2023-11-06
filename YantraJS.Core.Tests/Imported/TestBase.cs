using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;
using YantraJS.Core.Core.Primitive;

namespace YantraJS.Core.Tests.Imported
{
    public class TestBase
    {

        private JSContext context;
        public TestBase()
        {
            context = new JSContext();
        }

        public void Execute(string code)
        {
            context.Execute(code);
        }


        /// <summary>
        /// Changes the culture to run the the given action, then restores the culture.
        /// </summary>
        /// <param name="cultureName"> The culture name. </param>
        /// <param name="action"> The action to run under the modified culture. </param>
        public static T ChangeLocale<T>(string cultureName, Func<T> action)
        {
            // Save the current culture.
            var previousCulture = Thread.CurrentThread.CurrentCulture;

            // Replace it with a new culture.
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(cultureName, false);

            try
            {
                // Run the action.
                return action();
            }
            finally
            {
                // Restore the previous culture.
                Thread.CurrentThread.CurrentCulture = previousCulture;
            }
        }

        public object Evaluate(string text)
        {
            object ToPrimitive(object value)
            {
                switch (value)
                {
                    //case JSNull n:
                    //    return "null";
                    //case JSUndefined _:
                    //    return "undefined";
                    case JSPrimitiveObject primitiveObject:
                        return ToPrimitive(primitiveObject.value);
                    case JSString sv:
                        return sv.ToString();
                    case JSNumber number:
                        //if (double.IsNaN(number.value))
                        //    return "NaN";
                        if ((number.value) % 1 == 0) { 
                            if (number.value >= -2147483648.0 && number.value <= 2147483648.0)
                                return (int)number.value;
                        }
                        return number.value;
                    case JSBoolean boolean:
                        return boolean._value;
                }
                return value;
            }
            try
            {
                var v = context.Execute(text);
                return ToPrimitive(v);
            }catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message, ex);
            }
        }

        public object EvaluateExceptionType(string text)
        {
            try
            {
                context.Execute(text);
            } catch (Exception ex)
            {
                var ev = JSException.ErrorFrom(ex);
                var v = ev?.prototypeChain?[KeyStrings.constructor] as JSFunction;
                return v?.name.Value;
            }
            throw new Exception("No exception was thrown");
        }
        public object EvaluateExceptionMessage(string text)
        {
            try
            {
                context.Execute(text);
            } catch (Exception ex)
            {
                var ev = JSException.ErrorFrom(ex);
                var v = ev?.prototypeChain?[KeyStrings.constructor] as JSFunction;
                var r = v?.name.Value;
                return r;
            }
            throw new Exception("No exception was thrown");
        }

        public (JSValue Value, JSValue v2) Undefined => (JSUndefined.Value, null);

        public (JSValue Value, JSValue v2) Null => (JSNull.Value, null);

    }

}
