#nullable enable
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection.Emit;
using System.Text;
using YantraJS.Expressions;

namespace YantraJS.Generator
{
    public readonly struct DataSource
    {
        public readonly YExpression? Expression;
        public readonly int Index;

        public DataSource(YExpression? exp, int index = -1)
        {
            this.Expression = exp;
            this.Index = index;
        }

        public static implicit operator DataSource(YExpression exp) 
            => new DataSource(exp);

        public static implicit  operator DataSource(int index)
            => new DataSource(null, index);
    }

    public partial class ILCodeGenerator
    {
        protected override CodeInfo VisitAssign(YAssignExpression yAssignExpression)
        {
            // we need to investigate each type of expression on the left...
            // Visit(yAssignExpression.Right);
            // return Assign(yAssignExpression.Left);

            // from block a non saving expression must be called with -1
            using var temp = il.NewTemp(yAssignExpression.Type);
            VisitAssign(yAssignExpression, temp.LocalIndex);
            il.EmitLoadLocal(temp.LocalIndex);
            return true;
        }

        private CodeInfo VisitSave(DataSource data, int index = -1)
        {
            var exp = data.Expression;
            if (exp == null)
            {
                il.EmitLoadLocal(data.Index);
                return true;
            }
            //switch (exp.NodeType)
            //{
            //    case YExpressionType.Assign:
            //        var a = (exp as YAssignExpression)!;
            //        if(index == -1)
            //        {
            //            index = tempVariables[a.Right.Type].LocalIndex;
            //        }
            //        VisitAssign(a, index);
            //        il.EmitLoadLocal(index);
            //        return true;
            //}

            Visit(exp);
            if(index != -1)
            {
                il.Emit(OpCodes.Dup);
                il.EmitSaveLocal(index);
            }
            return true;
        }

        protected CodeInfo VisitAssign(YAssignExpression exp, int savedIndex)
        {
            switch (exp.Left.NodeType)
            {
                case YExpressionType.Parameter:
                    return AssignParameter(exp.Right, exp.Left as YParameterExpression, savedIndex);
                case YExpressionType.Property:
                    return AssignProperty(exp.Right, (exp.Left as YPropertyExpression)!, savedIndex);
                case YExpressionType.Field:
                    return AssignField(exp.Right, (exp.Left as YFieldExpression)!, savedIndex);
                case YExpressionType.Index:
                    return AssignIndex(exp.Right, (exp.Left as YIndexExpression)!, savedIndex);
                case YExpressionType.ArrayIndex:
                    return AssignArrayIndex(exp.Right, exp.Left as YArrayIndexExpression, savedIndex);
            }
            throw new NotImplementedException();
        }

        private CodeInfo Assign(YExpression left, DataSource source, int savedIndex = -1)
        {
            switch (left.NodeType)
            {
                case YExpressionType.Parameter:
                    return AssignParameter(source, left as YParameterExpression, savedIndex);
                case YExpressionType.Property:
                    return AssignProperty(source, (left as YPropertyExpression)!, savedIndex);
                case YExpressionType.Field:
                    return AssignField(source, (left as YFieldExpression)!, savedIndex);
                case YExpressionType.Index:
                    return AssignIndex(source, (left as YIndexExpression)!, savedIndex);
                case YExpressionType.ArrayIndex:
                    return AssignArrayIndex(source, left as YArrayIndexExpression, savedIndex);
            }
            throw new NotImplementedException();
        }

        //private CodeInfo Assign(YExpression left, int savedIndex = -1)
        //{
        //    switch (left.NodeType)
        //    {
        //        case YExpressionType.Parameter:
        //            return AssignParameter(left as YParameterExpression, savedIndex);
        //        case YExpressionType.Field:
        //            return AssignField(left as YFieldExpression, savedIndex);
        //        case YExpressionType.Property:
        //            return AssignProperty(left as YPropertyExpression, savedIndex);
        //        case YExpressionType.Assign:
        //            var a = left as YAssignExpression;
        //            if (savedIndex >= 0) {
        //                il.EmitLoadLocal(savedIndex);
        //                return Assign(a.Right, savedIndex);
        //            }
        //            Visit(a.Right);
        //            return Assign(a.Left, savedIndex);
        //        case YExpressionType.ArrayIndex:
        //            return AssignArrayIndex(left as YArrayIndexExpression, savedIndex);
        //        case YExpressionType.Index:
        //            return AssignIndex(left as YIndexExpression, savedIndex);
        //    }

        //    throw new NotImplementedException();
        //}

        private CodeInfo AssignIndex(DataSource exp, YIndexExpression yIndexExpression, int savedIndex = -1)
        {
            Visit(yIndexExpression.Target);
            var pa = yIndexExpression.SetMethod!.GetParameters();
            for (int i = 0; i < pa.Length - 1; i++)
            {
                var pe = yIndexExpression.Arguments[i];
                var p = pa[i];
                if(p.IsIn || p.IsOut)
                {
                    if(p.ParameterType.IsValueType)
                    {
                        LoadAddress(pe);
                        continue;
                    }
                }

                if(pe.NodeType == YExpressionType.Assign)
                {
                    using var t = il.NewTemp(pe.Type);
                    var ti = t.LocalIndex;
                    VisitAssign((pe as YAssignExpression)!, ti);
                    il.EmitLoadLocal(ti);
                    continue;
                }

                Visit(pe);
            }
            VisitSave(exp, savedIndex);
            il.EmitCall(yIndexExpression.SetMethod);
            return true;
        }

        private CodeInfo AssignProperty(DataSource exp, YPropertyExpression yPropertyExpression, int savedIndex = -1)
        {
            if (!yPropertyExpression.IsStatic)
                Visit(yPropertyExpression.Target);
            VisitSave(exp, savedIndex);
            il.EmitCall(yPropertyExpression.SetMethod);
            return true;
        }

        private CodeInfo AssignField(DataSource exp, YFieldExpression yFieldExpression, int savedIndex = -1)
        {
            if (!yFieldExpression.FieldInfo.IsStatic)
                Visit(yFieldExpression.Target);
            VisitSave(exp, savedIndex);
            il.Emit(OpCodes.Stfld, yFieldExpression.FieldInfo);
            return true;
        }
    }
}
