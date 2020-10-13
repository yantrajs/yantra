using System;
using System.Collections.Generic;
using System.Text;
using WebAtoms.CoreJS.Core.Date;

namespace WebAtoms.CoreJS.Core
{
    [JSRuntime(typeof(JSDateStatic), typeof(JSDatePrototype))]
    public class JSDate: JSObject
    {

        internal static readonly DateTimeOffset InvalidDate = DateTimeOffset.MinValue;

        internal static readonly JSDate invalidDate = new JSDate(DateTimeOffset.MinValue);

        internal static TimeSpan Local => TimeZoneInfo.Local.BaseUtcOffset;
        //internal static TimeSpan Local {
        //    get {
        //        return TimeZoneInfo.Local.BaseUtcOffset;
        //    }
        //}

        internal DateTimeOffset value;

        public DateTimeOffset Value
        {
            get => value;
            set => this.value = value;
        }

        public JSDate(DateTimeOffset time): base(JSContext.Current.DatePrototype)
        {
            this.value = time;
        }

        public override string ToString()
        {
            return this.value.ToString();
        }

        public override string ToDetailString()
        {
            return this.value.ToString();
        }


    }
}
