using System;
using System.Collections.Generic;
using System.Text;
using WebAtoms.CoreJS.Core.Date;

namespace WebAtoms.CoreJS.Core
{
    [JSRuntime(typeof(JSDateStatic), typeof(JSDatePrototype))]
    public class JSDate: JSObject
    {

        internal DateTime value;

        public DateTime Value
        {
            get => value;
            set => this.value = value;
        }

        public JSDate(DateTime time): base(JSContext.Current.DatePrototype)
        {
            this.value = time;
        }

    }
}
