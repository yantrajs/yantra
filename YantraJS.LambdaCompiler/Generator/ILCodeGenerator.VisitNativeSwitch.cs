using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using YantraJS.Core;
using YantraJS.Expressions;
using YantraJS.Internals;

namespace YantraJS.Generator
{
    public partial class ILCodeGenerator
    {
        protected override CodeInfo VisitNativeSwitch(YNativeSwitchExpression node)
        {
            using var tmp = tempVariables.Push();

            bool isString = node.Target.Type == typeof(string);

            LocalVariableInfo hash = null;

            // save local if it is not parameter...
            Action loadTarget = LoadTargetMethod();

            var jumpMethod = LoadCompareMethod();

            var @break = labels[node.Break];

            using (var caseBodies = new ActionList())
            {
                foreach (var @case in node.Cases)
                {
                    var jump = il.DefineLabel("caseBody", il.Top);
                    foreach (var test in @case.TestValues)
                    {
                        loadTarget();
                        jumpMethod(test, jump);

                    }
                    caseBodies.Add(() => {
                        il.MarkLabel(jump);
                        Visit(@case.Body);
                        if(@case.Body.Type != typeof(void))
                        {
                            il.Emit(OpCodes.Pop);
                        }
                    });

                }

                if (node.Default != null)
                {
                    Visit(node.Default);
                    if (node.Default.Type != typeof(void))
                    {
                        il.Emit(OpCodes.Pop);
                    }
                }
            }

            il.MarkLabel(@break);

            // lets leave one value...
            //if (node.Type != typeof(void))
            //{
            //    il.IncrementStack();
            //}

            return true;

            Action LoadTargetMethod()
            {
                var t = node.Target;
                var isParameter = t.NodeType == YExpressionType.Parameter;
                if (isParameter && !isString)
                {
                    return () => Visit(t);
                }
                var tmp = tempVariables[t.Type];
                Visit(t);
                if (isString)
                {
                    hash = tempVariables[typeof(int)];
                    if (!isParameter)
                    {
                        il.Emit(OpCodes.Dup);
                    }
                    il.EmitConstant(0);
                    il.EmitConstant(0);
                    il.EmitCall(UnsafeGetHashCode);
                    il.EmitSaveLocal(hash.LocalIndex);
                    if (!isParameter)
                    {
                        il.EmitSaveLocal(tmp.LocalIndex);
                    }
                    return () => {
                        if (isParameter)
                        {
                            Visit(t);
                        }
                        else
                        {
                            il.EmitLoadLocal(tmp.LocalIndex);
                        }
                        il.EmitLoadLocal(hash.LocalIndex);
                    };
                }
                il.EmitSaveLocal(tmp.LocalIndex);
                return () => il.EmitLoadLocal(tmp.LocalIndex);
            }

            Action<YExpression, ILWriterLabel> LoadCompareMethod()
            {
                void CompareInteger(YExpression test, ILWriterLabel target)
                {
                    Visit(test);
                    il.Emit(OpCodes.Beq, target);
                }

                if (node.Target.Type == typeof(int))
                {
                    return CompareInteger;
                }

                var cm = node.CompareMethod;

                if (isString)
                {
                    cm = StringEqualsMethod;
                    void CompareString(YExpression test, ILWriterLabel target)
                    {
                        var hash = (test as YConstantExpression).Value.ToString().UnsafeGetHashCode();
                        Visit(test);
                        il.EmitConstant(hash);
                        il.EmitCall(HashMatch);
                        il.Emit(OpCodes.Brtrue, target);
                    }
                    return CompareString;
                }

                void Compare(YExpression test, ILWriterLabel target)
                {
                    Visit(test);
                    il.EmitCall(cm);
                    il.Emit(OpCodes.Brtrue, target);
                }

                return Compare;
            }
        }
    }
}
