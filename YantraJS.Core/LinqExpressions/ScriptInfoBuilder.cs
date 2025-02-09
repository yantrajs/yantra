using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using YantraJS.Core.CodeGen;
using YantraJS.Core.Core.Storage;
using YantraJS.Core.LambdaGen;
using YantraJS.Core.Types;
using YantraJS.ExpHelper;
using Exp = YantraJS.Expressions.YExpression;
using Expression = YantraJS.Expressions.YExpression;
using ParameterExpression = YantraJS.Expressions.YParameterExpression;

namespace YantraJS.Core.LinqExpressions
{
    public static class ScriptInfoBuilder
    {

        // public static readonly Type type = typeof(ScriptInfo);

        //private static readonly ConstructorInfo _new =
        //    type.GetConstructor(new Type[] { });

        //private static readonly FieldInfo _code =
        //    type.GetField(nameof(ScriptInfo.Code));

        //private static readonly FieldInfo _fileName =
        //    type.GetField(nameof(ScriptInfo.FileName));

        //private static readonly FieldInfo _indices =
        //    type.GetField(nameof(ScriptInfo.Indices));

        //private static readonly FieldInfo _functions =
        //    type.GetField(nameof(ScriptInfo.Functions));

        //public static Expression Functions(Expression info)
        //{
        //    return Expression.Field(info, _functions);
        //}

        //public static Expression Function(Expression info, int index, Type type)
        //{
        //    var fs = Expression.Field(info, _functions);
        //    var fi = Expression.ArrayIndex(fs, Expression.Constant(index));
        //    return Expression.TypeAs(fi, type);
        //}


        public static Expression New(string fileName, string code)
        {
            var _code = TypeQuery.QueryInstanceField<ScriptInfo, string>(() => (x) => x.Code);
            var _fileName = TypeQuery.QueryInstanceField<ScriptInfo, string>(() => (x) => x.FileName);
            return 
                Expression.MemberInit(
                    NewLambdaExpression.NewExpression<ScriptInfo>(() => () => new ScriptInfo()),
                    Expression.Bind(_code, Expression.Constant(code)),
                    Expression.Bind(_fileName, Expression.Constant(fileName)));
            //return
            //    Expression.MemberInit(Expression.New(_new),
            //    Expression.Bind(_code, Expression.Constant(code)),
            //    Expression.Bind(_fileName, Expression.Constant(fileName)));
        }

        public static Expression Code(Expression scriptInfo)
        {
            return scriptInfo.FieldExpression<ScriptInfo, string>(() => (x) => x.Code);
            // return Expression.Field(scriptInfo, _code);
        }

        public static Expression FileName(Expression scriptInfo)
        {
            return scriptInfo.FieldExpression<ScriptInfo, string>(() => (x) => x.FileName);
            // return Expression.Field(scriptInfo, _fileName);
        }

        public static Expression KeyString(Expression scriptInfo, int index)
        {
            return Expression.ArrayIndex(
                // Expression.Field(scriptInfo, _indices)
                scriptInfo.FieldExpression<ScriptInfo, KeyString[]>(() => (x) => x.Indices)
                , Expression.Constant(index));
        }

        //private static Expression Indices(Expression scriptInfo)
        //{
        //    return Expression.Field(scriptInfo, _indices);
        //}

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
                scriptInfo.FieldExpression<ScriptInfo, KeyString[]>(() => (x) => x.Indices)
                ,Expression.NewArrayInit(typeof(KeyString), list));
        }
    }
}
