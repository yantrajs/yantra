using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using YantraJS.Core;

namespace YantraJS.Expressions
{
    public class YNewExpression: YExpression
    {
        public readonly ConstructorInfo constructor;
        public readonly IFastEnumerable<YExpression> args;

        /// <summary>
        /// Base class constructors must be called a a 'call' instruction and not 'new'
        /// </summary>
        public readonly bool AsCall;

        public YNewExpression(ConstructorInfo constructor, IFastEnumerable<YExpression> args, bool asCall = false)
            : base(YExpressionType.New, constructor.DeclaringType)
        {
            this.constructor = constructor;
            this.args = args;
            if (args.Any(x => x == null))
                throw new ArgumentNullException();
            this.AsCall = asCall;
        }

        public YNewExpression Update(ConstructorInfo constructor, IFastEnumerable<YExpression> args)
        {
            return new YNewExpression(constructor, args, AsCall);
        }

        public override void Print(IndentedTextWriter writer)
        {
            if (AsCall)
            {
                writer.Write($"call {constructor.DeclaringType.GetFriendlyName()}(");
            }
            else
            {
                writer.Write($"new {constructor.DeclaringType.GetFriendlyName()}(");
            }
            writer.PrintCSV(args);
            writer.Write(")");
        }
    }
}