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

        /// <summary>
        /// 
        /// https://sharplab.io/#gist:5048f7ec17ccf5740862929280bb306f
        /// </summary>
        /// <param name="yPropertyExpression"></param>
        /// <returns></returns>
        protected override CodeInfo VisitProperty(YPropertyExpression yPropertyExpression)
        {
            Visit(yPropertyExpression.Target);
            il.EmitCall(yPropertyExpression.GetMethod);

            if (RequiresAddress)
            {
                // we need to store this in temporary variable and send the address..
                var temp = this.tempVariables[yPropertyExpression.PropertyInfo.PropertyType];
                il.EmitSaveLocal(temp.LocalIndex);
                il.EmitLoadLocalAddress(temp.LocalIndex);
                return true;
            }
            return true;
        }

    }
}
