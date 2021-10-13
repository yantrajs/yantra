using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection.Emit;
using System.Text;
using YantraJS.Core;
using YantraJS.Expressions;

namespace YantraJS.Generator
{
    public partial class ILCodeGenerator
    {

        private void Goto(ILWriterLabel label)
        {
            il.Branch(label);
        }

        internal void EmitConstructor(YLambdaExpression cnstrLambda)
        {
            il.EmitLoadArg(0);
            Emit(cnstrLambda);
        }
    }
}
