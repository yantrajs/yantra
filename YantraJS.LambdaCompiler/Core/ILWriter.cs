using System;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using YantraJS.Generator;

namespace YantraJS.Core
{

    public class ILWriter
    {
        private LinkedStack<ILTryBlock> tryStack 
            = new LinkedStack<ILTryBlock>();
        private readonly ILGenerator il;
        private StringWriter writer = new StringWriter();

        public bool IsTryBlock => tryStack.Top != null;

        public ILWriter(ILGenerator il)
        {
            this.il = il;
        }

        public override string ToString()
        {
            return writer.ToString();
        }

        private void PrintOffset()
        {
            writer.Write("IL_");
            writer.Write(il.ILOffset.ToString("X4"));
            writer.Write(": ");
        }

        public void Branch(ILWriterLabel label, int index = 0)
        {
            if(tryStack.Top != null)
            {
                tryStack.Top.Branch(label, index);
                return;
            }
            Goto(label, index);
        }

        internal void Goto(ILWriterLabel label, int index = 0)
        {
            if (index > 0)
            {
                this.EmitLoadLocal(index);
            }
            PrintOffset();
            writer.WriteLine($"{OpCodes.Br} {label}");
            il.Emit(OpCodes.Br, label.Value);
        }

        internal void Emit(OpCode code)
        {
            PrintOffset();
            writer.WriteLine(code.Name);
            il.Emit(code);
        }

        internal ILWriterLabel DefineLabel()
        {
            var newLabel = new ILWriterLabel(il.DefineLabel(), tryStack.Top);
            return newLabel;
        }

        internal void Emit(OpCode code, short value)
        {
            PrintOffset();
            writer.WriteLine($"{code.Name} {value}");
            il.Emit(code, value);
        }

        internal void Verify()
        {
            
        }

        internal void Emit(OpCode code, int value)
        {
            PrintOffset();
            writer.WriteLine($"{code.Name} {value}");
            il.Emit(code, value);
        }

        internal void MarkLabel(ILWriterLabel label)
        {
            label.Offset = il.ILOffset;
            writer.WriteLine();
            writer.WriteLine($"{label}:");
            il.MarkLabel(label.Value);
        }

        internal void Emit(OpCode code, ILWriterLabel label)
        {
            PrintOffset();
            writer.WriteLine($"{code.Name} {label}");
            il.Emit(code, label.Value);
        }

        internal void Emit(OpCode code, FieldInfo field)
        {
            PrintOffset();
            writer.WriteLine($"{code.Name} {field.DeclaringType.FullName}.{field.Name}");
            il.Emit(code, field);
        }

        internal void Emit(OpCode code, Type type)
        {
            PrintOffset();
            writer.WriteLine($"{code.Name} {type.GetFriendlyName()}");
            il.Emit(code, type);
        }

        internal void Emit(OpCode code, float value)
        {
            PrintOffset();
            writer.WriteLine($"{code.Name} {value}");
            il.Emit(code, value);
        }

        internal void Emit(OpCode code, double value)
        {
            PrintOffset();
            writer.WriteLine($"{code.Name} {value}");
            il.Emit(code, value);
        }

        internal void Emit(OpCode code, ConstructorInfo value)
        {
            PrintOffset();
            writer.WriteLine($"{code.Name} {value.DeclaringType.FullName}");
            il.Emit(code, value);

        }

        internal void Emit(OpCode code, MethodInfo method)
        {
            PrintOffset();
            writer.WriteLine($"{code.Name} {method.DeclaringType.FullName}.{method.Name}");
            il.Emit(code, method);

        }

        internal void Emit(OpCode code, string value)
        {
            PrintOffset();
            writer.WriteLine($"{code.Name} {value}");
            il.Emit(code, value);
        }

        internal ILTryBlock BeginTry()
        {
            PrintOffset();
            writer.WriteLine("try:");
            var label = il.BeginExceptionBlock();
            var ilb = tryStack.Push(new ILTryBlock(this, label));
            return ilb;
        }

        internal void BeginCatchBlock(Type type)
        {
            PrintOffset();
            writer.WriteLine("catch:");
            il.BeginCatchBlock(type);
        }

        internal void BeginFinallyBlock()
        {
            PrintOffset();
            writer.WriteLine("finally:");
            il.BeginFinallyBlock();
        }

        internal void EndExceptionBlock()
        {
            PrintOffset();
            writer.WriteLine("end try:");
            il.EndExceptionBlock();
        }
    }
}
