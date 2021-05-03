using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using YantraJS.Expressions;

namespace YantraJS
{
    public class ClosureScopeStack : LinkedStack<ClosureScopeStack.ClosureScopeItem> {

        public void Register(YParameterExpression pe)
        {
            Top.PendingReplacements.Variables.Add(pe, null);
        }


        public ClosureScopeItem Push(YExpression exp)
        {
            return Push(new ClosureScopeItem(exp));
        }

        public class ClosureScopeItem : LinkedStackItem<ClosureScopeItem>
        {
            public readonly YExpression Expression;

            public readonly PendingReplacements PendingReplacements;

            public void Register(YParameterExpression[] list)
            {
                foreach (var item in list)
                {
                    if (!PendingReplacements.Variables.ContainsKey(item))
                        PendingReplacements.Variables.Add(item, null);
                }

            }

            public ClosureScopeItem(YExpression exp)
            {
                this.Expression = exp;
                PendingReplacements = exp.GetPendingReplacements();
            }

            private int index = 0;

            public int Length => index;

            internal YExpression Access(YParameterExpression node, bool box = false)
            {
                return AccessInternal(node, box).expression;
            }

            internal (YExpression expression, YParameterExpression parameter) AccessInternal(YParameterExpression node, bool box = false)
            {
                if(PendingReplacements.Variables.TryGetValue(node, out var be))
                {
                    if(box && be == null)
                    {
                        var boxType = node.Type; 
                        if(boxType.IsByRef)
                        {
                            boxType = boxType.ReflectedType;
                        }

                        var pe = YExpression.Parameter(typeof(Box<>).MakeGenericType(node.Type));
                        be = new BoxParamter {
                            Parameter = pe,
                            Type = node.Type,
                            Expression = YExpression.Field(pe, "Value" ),
                            Create = true
                        };
                        PendingReplacements.Variables[node] = be;
                    }
                    if (be != null)
                    {
                        return (be.Expression, be.Parameter);
                    }
                    return (node, node);
                }

                var (pn, pp) = Parent.AccessInternal(node, true );
                var n = YExpression.Parameter(typeof(Box<>).MakeGenericType(node.Type));
                var bp = new BoxParamter
                {
                    Parent = pn,
                    ParentParameter = pp,
                    Index = index++,
                    Parameter = n,
                    Expression = YExpression.Field(n, "Value")
                };
                PendingReplacements.Variables[node] = bp;
                return (bp.Expression, n);
            }

        }

        internal YExpression Access(YParameterExpression node)
        {
            return Top.Access(node);
        }

        internal (YExpression expression, YParameterExpression parameter) AccessParameter(YParameterExpression node)
        {
            return Top.AccessInternal(node);
        }

    }
}
