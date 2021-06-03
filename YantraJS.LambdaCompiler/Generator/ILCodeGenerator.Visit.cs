using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection.Emit;
using System.Text;
using YantraJS.Expressions;

namespace YantraJS.Generator
{
    public struct CodeInfo
    {
        public readonly bool Success;


        public CodeInfo(bool success)
        {
            this.Success = success;
        }


        public static implicit operator CodeInfo(bool success)
        {
            return new CodeInfo(success);
        }

        public static implicit operator bool (CodeInfo ci) => ci.Success;

    }

    public partial class ILCodeGenerator: YExpressionVisitor<CodeInfo>
    {

    }
}
