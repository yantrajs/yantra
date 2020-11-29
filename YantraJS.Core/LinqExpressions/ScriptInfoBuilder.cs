using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using YantraJS.Core.CodeGen;
using YantraJS.Core.Core.Storage;
using YantraJS.ExpHelper;

namespace YantraJS.Core.LinqExpressions
{
    public static class ScriptInfoBuilder
    {

        public static readonly Type type = typeof(ScriptInfo);

        private static readonly ConstructorInfo _new =
            type.GetConstructor(new Type[] { });

        private static readonly FieldInfo _code =
            type.GetField(nameof(ScriptInfo.Code));

        private static readonly FieldInfo _fileName =
            type.GetField(nameof(ScriptInfo.FileName));

        private static readonly FieldInfo _indices =
            type.GetField(nameof(ScriptInfo.Indices));

        public static Expression New(string fileName, string code)
        {
            return
                Expression.MemberInit(Expression.New(_new),
                Expression.Bind(_code, Expression.Constant(code)),
                Expression.Bind(_fileName, Expression.Constant(fileName)));
        }

        public static Expression Code(Expression scriptInfo)
        {
            return Expression.Field(scriptInfo, _code);
        }

        public static Expression FileName(Expression scriptInfo)
        {
            return Expression.Field(scriptInfo, _fileName);
        }

        public static Expression KeyString(Expression scriptInfo, int index)
        {
            return Expression.ArrayIndex(Expression.Field(scriptInfo, _indices), Expression.Constant(index));
        }

        private static Expression Indices(Expression scriptInfo)
        {
            return Expression.Field(scriptInfo, _indices);
        }

        public static Expression Build(Expression scriptInfo, StringArray keyStrings)
        {
            List<Expression> list = new List<Expression>();
            foreach(var item in keyStrings.List)
            {
                var code = Code(scriptInfo);
                var key = item.Offset > 0 
                    ? KeyStringsBuilder.GetOrCreate(StringSpanBuilder.New(code, item.Offset, item.Length))
                    : KeyStringsBuilder.GetOrCreate(StringSpanBuilder.New(Expression.Constant(item.Value), 0, item.Length));
                list.Add(key);
            }
            return Expression.Assign(
                Expression.Field(scriptInfo, _indices), Expression.NewArrayInit(typeof(KeyString), list));
        }
    }
}
