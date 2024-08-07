﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using YantraJS.Core.CodeGen;
using YantraJS.Core.Core.Storage;
using YantraJS.ExpHelper;
using Exp = YantraJS.Expressions.YExpression;
using Expression = YantraJS.Expressions.YExpression;
using ParameterExpression = YantraJS.Expressions.YParameterExpression;

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

        private static readonly FieldInfo _functions =
            type.GetField(nameof(ScriptInfo.Functions));

        public static Expression Functions(Expression info)
        {
            return Expression.Field(info, _functions);
        }

        public static Expression Function(Expression info, int index, Type type)
        {
            var fs = Expression.Field(info, _functions);
            var fi = Expression.ArrayIndex(fs, Expression.Constant(index));
            return Expression.TypeAs(fi, type);
        }


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
            Sequence<Expression> list = new Sequence<Expression>(keyStrings.List.Count);
            foreach(var item in keyStrings.List)
            {
                var code = Code(scriptInfo);
                var key = item.Offset > 0 
                    ? KeyStringsBuilder.GetOrCreate(StringSpanBuilder.New(code, item.Offset, item.Length))
                    : KeyStringsBuilder.GetOrCreate(StringSpanBuilder.New(item.Value));
                list.Add(key);
            }
            return Expression.Assign(
                Expression.Field(scriptInfo, _indices), Expression.NewArrayInit(typeof(KeyString), list));
        }
    }
}
