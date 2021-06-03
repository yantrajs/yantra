using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using YantraJS.Core.Core.Storage;
using YantraJS.Core.Runtime;
using YantraJS.Extensions;

namespace YantraJS.Core
{
    public partial class JSWeakMap: JSObject
    {

        private StringMap<(WeakReference<JSValue> key, JSValue value)>
            items = new StringMap<(WeakReference<JSValue> key, JSValue value)>();

        public JSWeakMap(): base(JSContext.Current.WeakMapPrototype)
        {
            this.SetTimeout();
        }

        private void ClearWeak()
        {
            lock (this)
            {
                var keysToDelete = new List<StringSpan>();
                int count= 0;
                foreach (var item in items.AllValues())
                {
                    if (!item.Value.key.TryGetTarget(out var v))
                    {
                        keysToDelete.Add(item.Key);
                        continue;
                    }
                    count++;
                }
                foreach (var key in keysToDelete)
                {
                    items.RemoveAt(key);
                }

                if (count > 0)
                {
                    SetTimeout();
                }
            }
        }

        private void SetTimeout()
        {
            Task.Run(async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds(30));
                this.ClearWeak();
            });
        }

        [Constructor]
        public static JSValue Constructor(in Arguments a)
        {
            return new JSWeakMap();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static JSWeakMap ToWeakMap(JSValue t, [CallerMemberName] string name = null)
        {
            if (t is JSWeakMap w)
                return w;
            throw JSContext.Current.NewTypeError($"WeakMap.prototype.{name} was not called with receiver WeakMap");
        }

        [Prototype("delete")]
        public static JSValue Delete(in Arguments a)
        {
            var w = ToWeakMap(a.This);
            lock (w)
            {
                var key = a.Get1().ToUniqueID();
                if (w.items.RemoveAt(key))
                    return JSBoolean.True;
                
            }
            return JSBoolean.False;
        }

        [Prototype("get")]
        public static JSValue Get(in Arguments a)
        {
            var w = ToWeakMap(a.This);
            lock (w)
            {
                var a1 = a.Get1();
                if (!a1.IsObject)
                    return JSUndefined.Value;
                var key = a.Get1().ToUniqueID();
                if (w.items.TryGetValue(key, out var v))
                {
                    if (v.key.TryGetTarget(out var vk))
                    {
                        return v.value;
                    }
                    w.items.RemoveAt(key);
                }

            }
            return JSUndefined.Value;

        }

        [Prototype("set")]
        public static JSValue Set(in Arguments a)
        {
            var w = ToWeakMap(a.This);
            var (first, second) = a.Get2();
            if (!(first is JSObject))
                throw JSContext.Current.NewTypeError($"Key cannot be a primitive value");
            lock (w)
            {
                var key = first.ToUniqueID();
                w.items.Save(key, ( new WeakReference<JSValue>(first), second ));
            }
            return a.This;
        }

        [Prototype("has")]
        public static JSValue Has(in Arguments a)
        {
            var w = ToWeakMap(a.This);
            lock (w)
            {
                var key = a.Get1().ToUniqueID();
                if (w.items.TryGetValue(key, out var v))
                {
                    if (v.key.TryGetTarget(out var vk))
                    {
                        return JSBoolean.True;
                    }
                    w.items.RemoveAt(key);
                }

            }
            return JSBoolean.False;

        }
    }
}
