using System;
using System.ComponentModel;
using System.Reflection;
using System.Threading.Tasks;
using YantraJS.Core.Clr;

namespace YantraJS.Core
{
    public static class JSPromiseExtensions
    {

        private static MethodInfo __convert =
            typeof(JSPromise).GetMethod(nameof(Convert),
                BindingFlags.Public | BindingFlags.Static | BindingFlags.Default | BindingFlags.DeclaredOnly);

        private static MethodInfo __toTask =
            typeof(JSPromise).GetMethod(nameof(ToTask),
                BindingFlags.Public | BindingFlags.Static | BindingFlags.Default | BindingFlags.DeclaredOnly);

        public static JSPromise ToPromise(this Task task)
        {
            var type = task.GetType();
            if (type.IsConstructedGenericType)
            {
                var factory = __convert.MakeGenericMethod(type.GetGenericArguments());
                return new JSPromise(factory.Invoke(null, new object[] { task }) as Task<JSValue>);
            }
            return new JSPromise(ConvertToUndefined(task));
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
            return __toTask.MakeGenericMethod(taskResultType.GetGenericArguments()).Invoke(null, new object[] { promise });
        }

        public static async Task<T> ToTask<T>(this JSPromise promise)
        {
            var task = promise.Task;
            var result = await task;
            return (T)result.ForceConvert(typeof(T));
        }
    }
}
