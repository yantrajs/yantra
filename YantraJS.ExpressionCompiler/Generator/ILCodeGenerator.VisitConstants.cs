using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection.Emit;
using System.Text;
using YantraJS.Expressions;

namespace YantraJS.Generator
{
    public partial class ILCodeGenerator
    {
        protected override CodeInfo VisitBooleanConstant(YBooleanConstantExpression node)
        {
            il.EmitConstant(node.Value);
            return true;
        }

        protected override CodeInfo VisitDoubleConstant(YDoubleConstantExpression node)
        {
            il.EmitConstant(node.Value);
            return true;
        }

        protected override CodeInfo VisitFloatConstant(YFloatConstantExpression node)
        {
            il.EmitConstant(node.Value);
            return true;
        }

        protected override CodeInfo VisitInt32Constant(YInt32ConstantExpression node)
        {
            il.EmitConstant(node.Value);
            return true;
        }

        protected override CodeInfo VisitInt64Constant(YInt64ConstantExpression node)
        {
            il.EmitConstant(node.Value);
            return true;
        }

        protected override CodeInfo VisitStringConstant(YStringConstantExpression node)
        {
            il.EmitConstant(node.Value);
            return true;
        }

        protected override CodeInfo VisitUInt32Constant(YUInt32ConstantExpression node)
        {
            il.EmitConstant(node.Value);
            return true;
        }

        protected override CodeInfo VisitUInt64Constant(YUInt64ConstantExpression node)
        {
            il.EmitConstant(node.Value);
            return true;
        }

        protected override CodeInfo VisitByteConstant(YByteConstantExpression node)
        {
            il.EmitConstant(node.Value);
            return true;
        }

        protected override CodeInfo VisitTypeConstant(YTypeConstantExpression node)
        {
            il.EmitConstant(node.Value);
            return true;
        }

        protected override CodeInfo VisitMethodConstant(YMethodConstantExpression node)
        {
            il.EmitConstant(node.Value);
            return true;
        }
    }
}
