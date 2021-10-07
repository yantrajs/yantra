using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Text;

namespace YantraJS.Expressions
{    

    public class YInt32ConstantExpression : YExpression
    {
        public readonly int Value;

        public YInt32ConstantExpression(int value) : base(YExpressionType.Int32Constant, typeof(int))
        {
            this.Value = value;
        }

        public override void Print(IndentedTextWriter writer)
        {
            writer.Write(Value);
        }
    }

    public class YUInt32ConstantExpression : YExpression
    {
        public readonly uint Value;

        public YUInt32ConstantExpression(uint value) : base(YExpressionType.UInt32Constant, typeof(uint))
        {
            this.Value = value;
        }

        public override void Print(IndentedTextWriter writer)
        {
            writer.Write(Value);
        }
    }

    public class YInt64ConstantExpression : YExpression
    {
        public readonly long Value;

        public YInt64ConstantExpression(long value) : base(YExpressionType.Int64Constant, typeof(long))
        {
            this.Value = value;
        }

        public override void Print(IndentedTextWriter writer)
        {
            writer.Write(Value);
        }
    }

    public class YUInt64ConstantExpression : YExpression
    {
        public readonly ulong Value;

        public YUInt64ConstantExpression(ulong value) : base(YExpressionType.UInt64Constant, typeof(ulong))
        {
            this.Value = value;
        }

        public override void Print(IndentedTextWriter writer)
        {
            writer.Write(Value);
        }
    }

    public class YDoubleConstantExpression : YExpression
    {
        public readonly double Value;

        public YDoubleConstantExpression(double value) : base(YExpressionType.DoubleConstant, typeof(double))
        {
            this.Value = value;
        }

        public override void Print(IndentedTextWriter writer)
        {
            writer.Write(Value);
        }
    }

    public class YFloatConstantExpression : YExpression
    {
        public readonly float Value;

        public YFloatConstantExpression(float value) : base(YExpressionType.FloatConstant, typeof(float))
        {
            this.Value = value;
        }

        public override void Print(IndentedTextWriter writer)
        {
            writer.Write(Value);
        }
    }

    public class YBooleanConstantExpression : YExpression
    {
        public readonly bool Value;

        public YBooleanConstantExpression(bool value) : base(YExpressionType.BooleanConstant, typeof(bool))
        {
            this.Value = value;
        }

        public override void Print(IndentedTextWriter writer)
        {
            writer.Write(Value);
        }
    }
    public class YStringConstantExpression : YExpression
    {
        public readonly string Value;

        public YStringConstantExpression(string value) : base(YExpressionType.StringConstant, typeof(string))
        {
            this.Value = value;
        }

        public override void Print(IndentedTextWriter writer)
        {
            writer.Write(Value);
        }
    }

}
