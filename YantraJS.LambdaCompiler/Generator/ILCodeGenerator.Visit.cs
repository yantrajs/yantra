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

        public void Visit(YExpression exp)
        {
            switch (exp.NodeType)
            {
                case YExpressionType.Binary:
                    break;
                case YExpressionType.Constant:
                    break;
                case YExpressionType.Conditional:
                    break;
                case YExpressionType.Assign:
                    break;
                case YExpressionType.Parameter:
                    break;
                case YExpressionType.Block:
                    break;
                case YExpressionType.Call:
                    break;
                case YExpressionType.New:
                    break;
                case YExpressionType.Field:
                    break;
                case YExpressionType.Property:
                    break;
                case YExpressionType.NewArray:
                    break;
                case YExpressionType.GoTo:
                    break;
                case YExpressionType.Return:
                    break;
                case YExpressionType.Loop:
                    break;
                case YExpressionType.TypeAs:
                    break;
                case YExpressionType.Lambda:
                    break;
                case YExpressionType.Label:
                    break;
            }
        }
    }
}
