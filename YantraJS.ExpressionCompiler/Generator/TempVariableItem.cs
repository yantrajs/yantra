using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;
using YantraJS.Core;

namespace YantraJS.Generator
{

    public class TempVariables: LinkedStack<TempVariables.TempVariableItem>
    {
        private readonly ILWriter il;

        public TempVariables(ILWriter il)
        {
            this.il = il;
        }

        public TempVariableItem Push()
        {
            return Push(new TempVariableItem(il));
        }

        public LocalBuilder this[Type type]
        {
            get
            {
                return Top.Get(type);
            }
        }

        public class TempVariableItem : LinkedStackItem<TempVariableItem>
        {
            private Sequence<IDisposable> disposables = new Sequence<IDisposable>();
            private readonly ILWriter writer;

            public TempVariableItem(ILWriter writer)
            {
                this.writer = writer;
            }


            public LocalBuilder Get(Type type)
            {
                var t = writer.NewTemp(type);
                disposables.Add(t);
                return t.Local;
            }

            public override void Dispose()
            {
                base.Dispose();
                var en = disposables.GetFastEnumerator();
                while(en.MoveNext(out var d))
                    d.Dispose();
            }
        }

    }
}
