using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using YantraJS.Core;
using YantraJS.Core.Core.Storage;

namespace YantraJS.ExpHelper
{
    internal class KeyStringsBuilder
    {
        private static MethodInfo _GetOrAdd =
            typeof(KeyStrings).InternalMethod(nameof(KeyStrings.GetOrCreate), StringSpanBuilder.RefType);

        public static readonly Type RefType = typeof(KeyString).MakeByRefType();

        public static Expression GetOrCreate(Expression text)
        {
            return Expression.Call(null, _GetOrAdd, text);
        }

        public readonly static StringMap<MemberExpression> Fields =
            ToStringMap(typeof(KeyStrings).GetFields());

        private static StringMap<MemberExpression> ToStringMap(FieldInfo[] fields)
        {
            StringMap<MemberExpression> map = new StringMap<MemberExpression>();
            foreach(var field in fields)
            {
                map[field.Name] = Expression.Field(null, field);
            }
            return map;
        }
    }


}
