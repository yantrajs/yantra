using System;
using System.Collections.Generic;
using System.Text;
using YantraJS.Core.Date;

namespace YantraJS.Core
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

        internal JSDate(JSObject prototype, DateTimeOffset time) : base(prototype)
        {
            this.value = time;
        }


        public JSDate(DateTimeOffset time): base(JSContext.Current.DatePrototype)
        {
            this.value = time;
        }

        //public override string ToString()
        //{
        //    return this.value.ToString();
        //}

        public override string ToDetailString()
        {
            return this.value.ToString();
        }

        public override bool ConvertTo(Type type, out object value)
        {
            if (type == typeof(DateTime))
            {
                value = this.value.LocalDateTime;
                return true;
            }
            if (type == typeof(DateTimeOffset))
            {
                value = this.value;
                return true;
            }
            if (type.IsAssignableFrom(typeof(JSDate)))
            {
                value = this;
                return true;
            }
            if (type == typeof(object))
            {
                value = this.value;
                return true;
            }
            return base.ConvertTo(type, out value);
        }

    }
}
