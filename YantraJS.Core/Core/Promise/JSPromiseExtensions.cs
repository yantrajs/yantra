using System;
using System.ComponentModel;
using System.Reflection;
using System.Threading.Tasks;
using YantraJS.Core.Clr;

namespace YantraJS.Core
{
    public static class JSPromiseExtensions
    {

        public static JSPromise ToPromise(this Task task)
        {
            var type = task.GetType();
            if (type.IsConstructedGenericType)
            {
                return Generic.InvokeAs(type.GetGenericArguments()[0], ToTypedPromise<object>, task);
                //var factory = __convert.MakeGenericMethod(type.GenericTypeArguments[0]);
                //return new JSPromise(factory.Invoke(null, new object[] { task }) as Task<JSValue>);
            }
            return new JSPromise(ConvertToUndefined(task));
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static JSPromise ToTypedPromise<T>(this Task task)
        {
            return new JSPromise(Convert<T>((Task<T>)task));
        }


        public static JSPromise ToPromise<T>(this Task<T> task)
        {
            return new JSPromise(Convert<T>(task));
        }


        [EditorBrowsable(EditorBrowsableState.Never)]
        public static async Task<JSValue> ConvertToUndefined(Task task)
        {
            await task;
            return JSUndefined.Value;
        }

        public static async Task<JSValue> Convert<T>(Task<T> task)
        {
            object result = await task;
            if (typeof(T) == typeof(JSValue))
                return (JSValue)result;
            return ClrProxy.Marshal(result);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static object ToTaskInternal(this JSPromise promise, Type taskResultType)
        {
            return Generic.InvokeAs(taskResultType.GetGenericArguments()[0], ToTask<object>, promise);
            // return __toTask.MakeGenericMethod(taskResultType.GetGenericArguments()).Invoke(null, new object[] { promise });
        }

        public static async Task<T> ToTask<T>(this JSPromise promise)
        {
            var task = promise.Task;
            var result = await task;
            return (T)result.ForceConvert(typeof(T));
        }
    }
}
