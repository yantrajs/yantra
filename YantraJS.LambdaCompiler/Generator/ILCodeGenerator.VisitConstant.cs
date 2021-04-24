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

        protected override CodeInfo VisitConstant(YConstantExpression yConstantExpression)
        {
            var value = yConstantExpression.Value;

            switch (value)
            {
                case string @string:
                    il.EmitConstant(@string);
                    return true;
                case int @int:
                    il.EmitConstant(@int);
                    return true;
                case bool b:
                    il.EmitConstant(b);
                    return true;
                case float f:
                    il.EmitConstant(f);
                    return true;
                case double d:
                    il.EmitConstant(d);
                    return true;
            }

            throw new NotSupportedException($"Constant of type  {yConstantExpression.Type} not supported, you must use a factory to create value of specified type");
        }

    }
}
