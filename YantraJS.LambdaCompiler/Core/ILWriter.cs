#nullable enable
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
        private TextWriter? writer = null;

        private int Stack;
        private OpCode last;

        public int ILOffset => il.ILOffset;

        public bool IsTryBlock => tryStack.Top != null;

        public ILTryBlock Top => tryStack.Top;

        public ILWriter(ILGenerator il)
        {
            this.il = il;
        }

        public override string ToString()
        {
            return writer?.ToString() ?? "";
        }

        private void PrintOffset()
        {
            if (writer == null)
                return;
            writer.Write($"S{Stack:x2}_");
            writer.Write("IL_");
            writer.Write(il.ILOffset.ToString("X4"));
            writer.Write(": ");
        }

        public void Branch(ILWriterLabel label, int index = -1)
        {
            if (tryStack.Top != null)
            {
                tryStack.Top.Branch(label, index);
                return;
            }
            Goto(label, index);
        }

        internal ILWriterLabel DefineLabel(string label, ILTryBlock? tryBlock = null)
        {
            return new ILWriterLabel(il.DefineLabel(), label, tryBlock);
        }

        public void Comment(string comment)
        {
            writer?.WriteLine($"// {comment}");
        }

        public IDisposable RetainBranch()
        {
            var s = Stack;
            return new DisposableAction(() => {
                Stack = s;
            });
        }

        public IDisposable Branch(bool pop = true, int size = 0)
        {
            var s = Stack;
            Stack = size;
            return new DisposableAction(() => {
                var n = Stack;
                Stack = s;
                while(n > s)
                {
                    if (pop)
                    {
                        il.Emit(OpCodes.Pop);
                    }
                    n--;
                }
            });
        }

        internal void ClearStack()
        {
            Stack = 0;
        }

        internal void Goto(ILWriterLabel label, int index = -1)
        {
            if (index >= 0)
            {
                this.EmitLoadLocal(index);
            }
            PrintOffset();
            writer?.WriteLine($"{OpCodes.Br} {label}");
            UpdateStack(OpCodes.Br);
            il.Emit(OpCodes.Br, label.Value);
        }

        private void UpdateStack(in OpCode code)
        {
            last = code;
            Update(code.StackBehaviourPush);
            Update(code.StackBehaviourPop);
            void Update(StackBehaviour sb)
            {
                switch (sb)
                {
                    case StackBehaviour.Pop0:
                        break;
                    case StackBehaviour.Pop1:
                        Stack--;
                        break;
                    case StackBehaviour.Pop1_pop1:
                        Stack--;
                        Stack--;
                        break;
                    case StackBehaviour.Popi:
                        Stack--;
                        break;
                    case StackBehaviour.Popi_pop1:
                        Stack--;
                        Stack--;
                        break;
                    case StackBehaviour.Popi_popi:
                        Stack--;
                        Stack--;
                        break;
                    case StackBehaviour.Popi_popi_popi:
                        Stack--;
                        Stack--;
                        Stack--;
                        break;
                    case StackBehaviour.Popi_popi8:
                        Stack--;
                        Stack--;
                        break;
                    case StackBehaviour.Popi_popr4:
                        Stack--;
                        Stack--;
                        break;
                    case StackBehaviour.Popi_popr8:
                        Stack--;
                        Stack--;
                        break;
                    case StackBehaviour.Popref:
                        Stack--;
                        break;
                    case StackBehaviour.Popref_pop1:
                        Stack--;
                        Stack--;
                        break;
                    case StackBehaviour.Popref_popi:
                        Stack--;
                        Stack--;
                        break;
                    case StackBehaviour.Popref_popi_pop1:
                        Stack--;
                        Stack--;
                        Stack--;
                        break;
                    case StackBehaviour.Popref_popi_popi:
                        Stack--;
                        Stack--;
                        Stack--;
                        break;
                    case StackBehaviour.Popref_popi_popi8:
                        Stack--;
                        Stack--;
                        Stack--;
                        break;
                    case StackBehaviour.Popref_popi_popr4:
                        Stack--;
                        Stack--;
                        Stack--;
                        break;
                    case StackBehaviour.Popref_popi_popr8:
                        Stack--;
                        Stack--;
                        Stack--;
                        break;
                    case StackBehaviour.Popref_popi_popref:
                        Stack--;
                        Stack--;
                        Stack--;
                        break;
                    case StackBehaviour.Push0:
                        break;
                    case StackBehaviour.Push1:
                        Stack++;
                        break;
                    case StackBehaviour.Push1_push1:
                        Stack++;
                        Stack++;
                        break;
                    case StackBehaviour.Pushi:
                        Stack++;
                        break;
                    case StackBehaviour.Pushi8:
                        Stack++;
                        break;
                    case StackBehaviour.Pushr4:
                        Stack++;
                        break;
                    case StackBehaviour.Pushr8:
                        Stack++;
                        break;
                    case StackBehaviour.Pushref:
                        Stack++;
                        break;
                    case StackBehaviour.Varpop:
                        // Stack--;
                        break;
                    case StackBehaviour.Varpush:
                        // Stack++;
                        break;
                }
            }
        }

        internal void IncrementStack()
        {
            Stack++;
        }

        internal void EmptyStack()
        {
            while (Stack > 0)
            {
                Emit(OpCodes.Pop);
            }
        }

        internal void Emit(in OpCode code)
        {
            UpdateStack(code);
            PrintOffset();
            writer?.WriteLine(code.Name);
            il.Emit(code);
        }

        internal void Emit(in OpCode code, short value)
        {
            UpdateStack(code);
            PrintOffset();
            writer?.WriteLine($"{code.Name} {value}");
            il.Emit(code, value);
        }

        internal void Verify()
        {
            if (Stack > 1)
                throw new InvalidOperationException($"Stack is not empty {Stack}");

        }

        internal void Emit(in OpCode code, int value)
        {
            UpdateStack(code);
            PrintOffset();
            writer?.WriteLine($"{code.Name} {value}");
            il.Emit(code, value);
        }

        internal void MarkLabel(ILWriterLabel label)
        {
            label.Offset = il.ILOffset;
            writer?.WriteLine();
            writer?.WriteLine($"{label}:");
            il.MarkLabel(label.Value);

            
        }

        internal void EmitConsoleWriteLine(string text)
        {
            PrintOffset();
            writer?.WriteLine($"Console.Writeline({text.Quoted()})");
            il.EmitWriteLine(text);
        }

        internal void Emit(in OpCode code, ILWriterLabel label)
        {
            UpdateStack(code);
            PrintOffset();
            writer?.WriteLine($"{code.Name} {label}");
            il.Emit(code, label.Value);
        }

        internal void Emit(in OpCode code, FieldInfo field)
        {
            UpdateStack(code);
            PrintOffset();
            writer?.WriteLine($"{code.Name} {field.DeclaringType.GetFriendlyName()}.{field.Name}");
            il.Emit(code, field);
        }

        internal void Emit(in OpCode code, Type type)
        {
            UpdateStack(code);
            PrintOffset();
            writer?.WriteLine($"{code.Name} {type.GetFriendlyName()}");
            il.Emit(code, type);
        }

        internal void Emit(in OpCode code, float value)
        {
            UpdateStack(code);
            PrintOffset();
            writer?.WriteLine($"{code.Name} {value}");
            il.Emit(code, value);
        }

        internal void Emit(in OpCode code, double value)
        {
            UpdateStack(code);
            PrintOffset();
            writer?.WriteLine($"{code.Name} {value}");
            il.Emit(code, value);
        }

        internal void Emit(in OpCode code, ConstructorInfo value)
        {
            UpdateStack(code);

            Stack -= value.GetParameters().Length;
            if (code.Value != OpCodes.Newobj.Value)
                Stack--;
            // Stack++;
            PrintOffset();
            writer?.WriteLine($"{code.Name} {value.DeclaringType.GetFriendlyName()}");
            il.Emit(code, value);

        }

        internal void Emit(in OpCode code, MethodInfo method)
        {
            UpdateStack(code);
            if (code.Value != OpCodes.Ldftn.Value)
            {
                Stack -= method.GetParameters().Length;
                if (!method.IsStatic)
                {
                    Stack--;
                }

                if (method.ReturnType != typeof(void))
                {
                    Stack++;
                }
            }

            PrintOffset();
            writer?.WriteLine($"{code.Name} {method.DeclaringType.GetFriendlyName()}.{method.Name}");
            //if (method is DynamicMethod) {
            //    if (code == OpCodes.Ldftn)
            //    {
            //        il.Emit(code, (int)method.MethodHandle.GetFunctionPointer());
            //        return;
            //    }
            //}
            il.Emit(code, method);

        }

        internal void Emit(in OpCode code, string value)
        {
            UpdateStack(code);
            PrintOffset();
            writer?.WriteLine($"{code.Name} {value.Quoted()}");
            il.Emit(code, value);
        }

        internal ILTryBlock BeginTry()
        {
            PrintOffset();
            writer?.WriteLine("try:");
            var label = il.BeginExceptionBlock();
            var ilb = tryStack.Push(new ILTryBlock(this, label));
            return ilb;
        }

        internal void BeginCatchBlock(Type type)
        {
            PrintOffset();
            writer?.WriteLine("catch:");
            il.BeginCatchBlock(type);
        }

        internal void BeginFinallyBlock()
        {
            PrintOffset();
            writer?.WriteLine("finally:");
            il.BeginFinallyBlock();
        }

        internal void EndExceptionBlock()
        {
            PrintOffset();
            writer?.WriteLine("end try:");
            il.EndExceptionBlock();
        }
    }
}
