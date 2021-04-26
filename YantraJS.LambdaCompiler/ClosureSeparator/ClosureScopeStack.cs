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

        public void Register(YParameterExpression[] list)
        {
            foreach (var item in list)
            {
                if(!Top.PendingReplacements.Variables.ContainsKey(item))
                    Top.PendingReplacements.Variables.Add(item, null);
            }
        }

        public ClosureScopeItem Push(YExpression exp)
        {
            return Push(new ClosureScopeItem(exp));
        }

        public class ClosureScopeItem : LinkedStackItem<ClosureScopeItem>
        {
            public readonly YExpression Expression;

            public readonly PendingReplacements PendingReplacements;


            public ClosureScopeItem(YExpression exp)
            {
                this.Expression = exp;
                PendingReplacements = exp.GetPendingReplacements();
            }

            private int index = 0;

            public int Length => index;

            internal YExpression Access(YParameterExpression node, bool box = false)
            {
                if(PendingReplacements.Variables.TryGetValue(node, out var be))
                {
                    if(box && be == null)
                    {
                        var pe = YExpression.Parameter(typeof(Box<>).MakeGenericType(node.Type));
                        be = new BoxParamter {
                            Parameter = pe,
                            Type = node.Type,
                            Expression = YExpression.Field(pe, "Value" ),
                            Create = true
                        };
                        PendingReplacements.Variables[node] = be;
                    }
                    return be?.Expression ?? node;
                }

                var pn = Parent.Access(node, true );
                var n = YExpression.Parameter(typeof(Box<>).MakeGenericType(node.Type));
                var bp = new BoxParamter
                {
                    Parent = pn,
                    Index = index++,
                    Parameter = n,
                    Expression = YExpression.Field(n, "Value")
                };
                PendingReplacements.Variables[node] = bp;
                return bp.Expression;
            }

        }

        internal YExpression Access(YParameterExpression node)
        {
            return Top.Access(node);
        }
    }
}
