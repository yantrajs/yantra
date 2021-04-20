using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;

namespace YantraJS
{
    public class ClosureScopeStack : LinkedStack<ClosureScopeStack.ClosureScopeItem> {

        public void Register(ParameterExpression pe)
        {
            Top.PendingReplacements.Variables.Add(pe, null);
        }

        public void Register(ReadOnlyCollection<ParameterExpression> list)
        {
            foreach(var item in list)
                Top.PendingReplacements.Variables.Add(item, null);
        }

        public ClosureScopeItem Push(Expression exp)
        {
            return Push(new ClosureScopeItem(exp));
        }

        public class ClosureScopeItem : LinkedStackItem<ClosureScopeItem>
        {
            public readonly Expression Expression;

            public readonly PendingReplacements PendingReplacements;


            public ClosureScopeItem(Expression exp)
            {
                this.Expression = exp;
                PendingReplacements = exp.GetPendingReplacements();
            }

            internal Expression Access(ParameterExpression node, bool box = false)
            {
                if(PendingReplacements.Variables.TryGetValue(node, out var be))
                {
                    if(box && be == null)
                    {
                        var pe = Expression.Parameter(typeof(Box<>).MakeGenericType(node.Type));
                        be = new BoxParamter {
                            Parameter = pe,
                            Type = node.Type,
                            Expression = Expression.Field(pe, "Value" ),
                            Create = true
                        };
                        PendingReplacements.Variables[node] = be;
                    }
                    return be.Expression;
                }

                var pn = Parent.Access(node, true );
                var n = Expression.Parameter(typeof(Box<>).MakeGenericType(node.Type));
                var bp = new BoxParamter
                {
                    Parent = pn,
                    Parameter = n,
                    Expression = Expression.Field(n, "Value")
                };
                PendingReplacements.Variables[n] = bp;
                return bp.Expression;
            }

        }

        internal Expression Access(ParameterExpression node)
        {
            return Top.Access(node);
        }
    }
}
