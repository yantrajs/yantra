using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using YantraJS.Core;
using YantraJS.Core.Core.Storage;
using YantraJS.Core.LambdaGen;
using YantraJS.Expressions;
using Exp = YantraJS.Expressions.YExpression;
using Expression = YantraJS.Expressions.YExpression;
using ParameterExpression = YantraJS.Expressions.YParameterExpression;

namespace YantraJS.ExpHelper
{
    internal class KeyStringsBuilder
    {
        public static readonly Type RefType = typeof(KeyString).MakeByRefType();

        public static Expression GetOrCreate(Expression text)
        {
            return NewLambdaExpression.StaticCallExpression<KeyString>(() =>
                () => KeyStrings.GetOrCreate((StringSpan)"")
            , text);
            // return Expression.Call(null, _GetOrAdd, text);
        }

        public readonly static StringMap<YFieldExpression> Fields =
            ToStringMap(typeof(KeyStrings).GetFields());

        private static StringMap<YFieldExpression> ToStringMap(FieldInfo[] fields)
        {
            StringMap<YFieldExpression> map = new StringMap<YFieldExpression>();
            foreach(var field in fields)
            {
                map.Put(field.Name) = Expression.Field(null, field);
            }
            return map;
        }
    }


}
