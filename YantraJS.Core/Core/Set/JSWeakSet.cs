using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Yantra.Core;
using YantraJS.Core.Clr;
using YantraJS.Core.Core.Storage;
using YantraJS.Extensions;

namespace YantraJS.Core.Set
{
    [JSClassGenerator("WeakSet")]
    public partial class JSWeakSet : JSObject
    {

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class WeakValue
        {
            private readonly JSWeakSet weakSet;
            private readonly HashedString key;
            /// <summary>
            /// This is public so that compiler will not remove it...
            /// </summary>
            public readonly JSValue value;

            public WeakValue(JSWeakSet weakSet, HashedString key, JSValue value)
            {
                this.weakSet = weakSet;
                this.key = key;
                this.value = value;
            }

            ~WeakValue()
            {
                lock (weakSet)
                {
                    weakSet.index.RemoveAt(key.Value);
                }
            }
        }

        private StringMap<WeakValue> index;

        public JSWeakSet(in Arguments a) : base(a.NewPrototype)
        {

        }

        [JSExport("add")]
        public JSValue Add(JSObject a)
        {
            HashedString key = a.ToUniqueID();
            lock (this)
            {
                if (!index.TryGetValue(key, out var w))
                {
                    index.Put(key) = new(this, key, a);
                }
            }
            return a;
        }


        [JSExport("delete")]
        public JSValue Delete(in Arguments a)
        {
            var key = a.Get1().ToUniqueID();
            lock (this)
            {
                if (index.TryRemove(key, out var w))
                {
                    GC.SuppressFinalize(w);
                    return JSBoolean.True;
                }
            }

            return JSBoolean.False;
        }

        [Prototype("has")]
        public JSValue Has(in Arguments a)
        {
            var key = a.Get1().ToUniqueID();
            lock (this)
            {
                if (index.TryGetValue(key, out var v))
                {
                    return JSBoolean.True;
                }
            }

            return JSUndefined.Value;
        }
    }
}
