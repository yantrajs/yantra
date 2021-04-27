using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading;

namespace YantraJS.Core
{
    public class ILWriterLabel
    {
        public readonly Label Value;

        public readonly int ID;

        public int Offset;

        private static int nextID = 1;

        public ILWriterLabel(Label value)
        {
            this.Value = value;
            this.ID = Interlocked.Increment(ref nextID);
        }

        public override string ToString()
        {
            if(Offset>0)
                return $"L_{ID}_{Offset}";
            return $"L_{ID}";
        }
    }

    public class ILWriter
    {
        private readonly ILGenerator il;
        private StringWriter writer = new StringWriter();

        public ILWriter(ILGenerator il)
        {
            this.il = il;
        }

        public override string ToString()
        {
            return writer.ToString();
        }

        internal void Emit(OpCode code)
        {
            writer.WriteLine(code.Name);
            il.Emit(code);
        }

        internal ILWriterLabel DefineLabel()
        {
            return new ILWriterLabel(il.DefineLabel());
        }

        internal void Emit(OpCode code, short value)
        {
            writer.WriteLine($"{code.Name} {value}");
            il.Emit(code, value);
        }

        internal void Emit(OpCode code, int value)
        {
            writer.WriteLine($"{code.Name} {value}");
            il.Emit(code, value);
        }

        internal void MarkLabel(ILWriterLabel label)
        {
            label.Offset = il.ILOffset;
            writer.WriteLine($"{label}:");
            il.MarkLabel(label.Value);
        }

        internal void Emit(OpCode code, ILWriterLabel label)
        {
            writer.WriteLine($"{code.Name} {label}");
            il.Emit(code, label.Value);
        }

        internal void Emit(OpCode code, FieldInfo field)
        {
            writer.WriteLine($"{code.Name} {field.DeclaringType.FullName}.{field.Name}");
            il.Emit(code, field);
        }

        internal void Emit(OpCode code, Type type)
        {
            writer.WriteLine($"{code.Name} {type.FullName}");
            il.Emit(code, type);
        }

        internal void Emit(OpCode code, float value)
        {
            writer.WriteLine($"{code.Name} {value}");
            il.Emit(code, value);
        }

        internal void Emit(OpCode code, double value)
        {
            writer.WriteLine($"{code.Name} {value}");
            il.Emit(code, value);
        }

        internal void Emit(OpCode code, ConstructorInfo value)
        {
            writer.WriteLine($"{code.Name} {value.DeclaringType.FullName}");
            il.Emit(code, value);

        }

        internal void Emit(OpCode code, MethodInfo method)
        {
            writer.WriteLine($"{code.Name} {method.DeclaringType.FullName}.{method.Name}");
            il.Emit(code, method);

        }

        internal void Emit(OpCode code, string value)
        {
            writer.WriteLine($"{code.Name} {value}");
            il.Emit(code, value);
        }
    }
}
