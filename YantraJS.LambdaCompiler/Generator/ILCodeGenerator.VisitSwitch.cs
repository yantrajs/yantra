using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using YantraJS.Core;
using YantraJS.Expressions;

namespace YantraJS.Generator
{
    public class ActionList : IDisposable
    {
        List<Action> actions = new List<Action>();

        public void Add(Action action)
        {
            actions.Add(action);
        }

        public void Dispose()
        {
            foreach(var a in actions)
            {
                a();
            }
        }
    }

    public partial class ILCodeGenerator
    {



        private MethodInfo StringEqualsMethod =
            typeof(string)
            .GetMethod(nameof(string.Equals), BindingFlags.Public | BindingFlags.Static, null, new Type[] { 
                typeof(string),
                typeof(string)
            }, null);

        protected override CodeInfo VisitSwitch(YSwitchExpression node)
        {

            // save local if it is not parameter...
            Action loadTarget = LoadTargetMethod();

            var jumpMethod = LoadCompareMethod();

            var @break = il.DefineLabel("break", il.Top);

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
                        using (il.Branch(false))
                        {
                            il.MarkLabel(jump);
                            Visit(@case.Body);
                        }
                        il.Emit(OpCodes.Br, @break);
                    });

                }

                if (node.Default != null)
                {
                    using (il.Branch(false))
                    {
                        Visit(node.Default);
                    }
                }
                il.Emit(OpCodes.Br, @break);
            }

            il.MarkLabel(@break);

            // lets leave one value...
            if (node.Type != typeof(void))
            {
                il.IncrementStack();
            }

            return true;

            Action LoadTargetMethod()
            {
                if (node.Target.NodeType == YExpressionType.Parameter)
                {
                    var t = node.Target;
                    return () => Visit(t);
                }
                var tmp = tempVariables[node.Target.Type];
                Visit(node.Target);
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
                
                if(node.Target.Type == typeof(int)) {
                    return CompareInteger;
                }

                var cm = node.CompareMethod;

                if (node.Target.Type == typeof(string))
                    cm = StringEqualsMethod;

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
