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

        public readonly bool Stack;

        public CodeInfo(bool success, bool stack = false)
        {
            this.Success = success;
            this.Stack = stack;
        }

        public static CodeInfo HasStack => new CodeInfo(true, true);
        

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
