using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Reflection;
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

        private static YInt32ConstantExpression MinusOne = new YInt32ConstantExpression(-1);

        private static YInt32ConstantExpression _0 = new YInt32ConstantExpression(0);

        private static YInt32ConstantExpression _1 = new YInt32ConstantExpression(1);

        private static YInt32ConstantExpression _2 = new YInt32ConstantExpression(2);

        private static YInt32ConstantExpression _3 = new YInt32ConstantExpression(3);

        private static YInt32ConstantExpression _4 = new YInt32ConstantExpression(4);

        private static YInt32ConstantExpression _5 = new YInt32ConstantExpression(5);

        private static YInt32ConstantExpression _6 = new YInt32ConstantExpression(6);

        private static YInt32ConstantExpression _7 = new YInt32ConstantExpression(7);

        private static YInt32ConstantExpression _8 = new YInt32ConstantExpression(8);

        private static YInt32ConstantExpression _16 = new YInt32ConstantExpression(16);

        private static YInt32ConstantExpression _32 = new YInt32ConstantExpression(32);

        private static YInt32ConstantExpression _64 = new YInt32ConstantExpression(64);

        private static YInt32ConstantExpression _128 = new YInt32ConstantExpression(128);

        private static YInt32ConstantExpression _256 = new YInt32ConstantExpression(256);

        private static YInt32ConstantExpression _512 = new YInt32ConstantExpression(512);

        private static YInt32ConstantExpression _1024 = new YInt32ConstantExpression(1024);

        internal static YInt32ConstantExpression For(int value)
        {
            switch (value)
            {
                case -1: return MinusOne;
                case 0: return _0;
                case 1: return _1;
                case 2: return _2;
                case 3: return _3;
                case 4: return _4;
                case 5: return _5;
                case 6: return _6;
                case 7: return _7;
                case 8: return _8;
                case 16: return _16;
                case 32: return _32;
                case 64: return _64;
                case 128: return _128;
                case 256: return _256;
                case 512: return _512;
                case 1024: return _1024;
            }
            return new YInt32ConstantExpression(value);
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

        private static YUInt32ConstantExpression _0 = new YUInt32ConstantExpression(0);

        private static YUInt32ConstantExpression _1 = new YUInt32ConstantExpression(1);

        private static YUInt32ConstantExpression _2 = new YUInt32ConstantExpression(2);

        private static YUInt32ConstantExpression _3 = new YUInt32ConstantExpression(3);

        private static YUInt32ConstantExpression _4 = new YUInt32ConstantExpression(4);

        private static YUInt32ConstantExpression _5 = new YUInt32ConstantExpression(5);

        private static YUInt32ConstantExpression _6 = new YUInt32ConstantExpression(6);

        private static YUInt32ConstantExpression _7 = new YUInt32ConstantExpression(7);

        private static YUInt32ConstantExpression _8 = new YUInt32ConstantExpression(8);

        private static YUInt32ConstantExpression _16 = new YUInt32ConstantExpression(16);

        private static YUInt32ConstantExpression _32 = new YUInt32ConstantExpression(32);

        private static YUInt32ConstantExpression _64 = new YUInt32ConstantExpression(64);

        private static YUInt32ConstantExpression _128 = new YUInt32ConstantExpression(128);

        private static YUInt32ConstantExpression _256 = new YUInt32ConstantExpression(256);

        private static YUInt32ConstantExpression _512 = new YUInt32ConstantExpression(512);

        private static YUInt32ConstantExpression _1024 = new YUInt32ConstantExpression(1024);

        internal static YUInt32ConstantExpression For(uint value)
        {
            switch (value)
            {
                case 0: return _0;
                case 1: return _1;
                case 2: return _2;
                case 3: return _3;
                case 4: return _4;
                case 5: return _5;
                case 6: return _6;
                case 7: return _7;
                case 8: return _8;
                case 16: return _16;
                case 32: return _32;
                case 64: return _64;
                case 128: return _128;
                case 256: return _256;
                case 512: return _512;
                case 1024: return _1024;
            }
            return new YUInt32ConstantExpression(value);
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

        public static YBooleanConstantExpression True = new YBooleanConstantExpression(true);

        public static YBooleanConstantExpression False = new YBooleanConstantExpression(false);

        private YBooleanConstantExpression(bool value) : base(YExpressionType.BooleanConstant, typeof(bool))
        {
            this.Value = value;
        }

        public override void Print(IndentedTextWriter writer)
        {
            writer.Write(Value);
        }
    }

    public class YByteConstantExpression : YExpression
    {
        public readonly byte Value;

        public YByteConstantExpression(byte value) : base(YExpressionType.ByteConstant, typeof(byte))
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

    public class YTypeConstantExpression: YExpression
    {
        public readonly Type Value;
        public YTypeConstantExpression(Type value) : base(YExpressionType.TypeConstant, typeof(Type))
        {
            this.Value = value;
        }

        public override void Print(IndentedTextWriter writer)
        {
            writer.Write(Value);
        }
    }

    public class YMethodConstantExpression : YExpression
    {
        public readonly MethodInfo Value;
        public YMethodConstantExpression(MethodInfo value) : base(YExpressionType.MethodConstant, typeof(Type))
        {
            this.Value = value;
        }

        public override void Print(IndentedTextWriter writer)
        {
            writer.Write(Value);
        }
    }

}
