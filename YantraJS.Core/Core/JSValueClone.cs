using System;
using System.Collections.Generic;
using System.Text;

namespace YantraJS.Core
{
    internal static class JSValueClone
    {

        public static void CloneFrom(this JSFunction copy, JSFunction jsf)
        {
            // copy prototype members

            ref var target = ref copy.prototype.GetOwnProperties();
            var en = new PropertySequence.ValueEnumerator(jsf.prototype, false);
            while (en.MoveNextProperty(out var Value, out var Key))
            {
                if (Key.Key != KeyStrings.constructor.Key)
                {
                    target.Put(Key.Key) = Value;
                }
            }

            // copy static members

            ref var ro = ref copy.GetOwnProperties();
            en = new PropertySequence.ValueEnumerator(jsf, false);
            while (en.MoveNextProperty(out var Value, out var Key))
            {
                /// this is the case when we do not
                /// want to overwrite Function.prototype
                if (Key.Key != KeyStrings.prototype.Key)
                {
                    ro.Put(Key.Key) = Value;
                }
            }


            // copy prototype symbols

            ref var symbols = ref copy.prototype.GetSymbols();

            foreach(var symbol in jsf.prototype.GetSymbols().All)
            {
                symbols.Put(symbol.Key) = symbol.Value;
            }

            // copy static symbols

            symbols = ref copy.GetSymbols();

            foreach (var symbol in jsf.GetSymbols().All)
            {
                symbols.Put(symbol.Key) = symbol.Value;
            }

        }

    }
}
