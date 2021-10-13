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

        /***
         * https://sharplab.io/#v2:C4LglgNgPgAgTARgLACgYGYAE9MGFMDeqmJ2WMALJgLIAUAlIcaQL7MntnYIBs3ADDQS0A9gCMAVgFMAxsEwBDRkRSk12AOyLFAZwEBuTmxScM3PmJEiINOKMmz5SpqvUkYWhZjB6YCfoauJMaoLEA==
         */

        protected override CodeInfo VisitTypeIs(YTypeIsExpression yTypeIsExpression)
        {
            Visit(yTypeIsExpression.Target);
            if (yTypeIsExpression.Type.IsAssignableFrom(yTypeIsExpression.Target.Type))
                return true;
            il.Emit(OpCodes.Isinst, yTypeIsExpression.TypeOperand);
            il.Emit(OpCodes.Ldnull);
            il.Emit(OpCodes.Cgt_Un);
            return true;
        }

        protected override CodeInfo VisitTypeAs(YTypeAsExpression typeAsExpression)
        {
            Visit(typeAsExpression.Target);
            if (typeAsExpression.Type.IsAssignableFrom(typeAsExpression.Target.Type))
                return true;
            il.Emit(OpCodes.Isinst, typeAsExpression.Type);
            return true;
        }
    }
}
