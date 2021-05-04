using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using YantraJS.Expressions;

namespace YantraJS
{
    public class ClosureScopeStack : LinkedStack<ClosureScopeStack.ClosureScopeItem> {

        public void Register(YParameterExpression pe)
        {
            Top.PendingReplacements.Variables.Add(pe, null);
        }


        public ClosureScopeItem Push(YLambdaExpression exp)
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

            public void Register(YLambdaExpression exp)
            {
                foreach (var item in exp.Parameters)
                {
                    if (!PendingReplacements.Variables.ContainsKey(item))
                        PendingReplacements.Variables.Add(item, null);
                }
                if (exp.This != null)
                {
                    if (!PendingReplacements.Variables.ContainsKey(exp.This))
                        PendingReplacements.Variables.Add(exp.This, null);
                }
            }


            public ClosureScopeItem(YLambdaExpression exp)
            {
                this.Expression = exp;
                PendingReplacements = exp.PendingReplacements;
            }

            private int index => 
                PendingReplacements.Variables.Count(x => x.Value?.ParentParameter != null);

            public int Length => index;

            internal YExpression Access(YParameterExpression node, bool box = false)
            {
                return AccessInternal(node, box).expression;
            }

            internal (YExpression expression, YParameterExpression parameter) AccessInternal(YParameterExpression node, bool box = false)
            {
                var boxType = node.Type;
                if (boxType.IsByRef)
                {
                    boxType = boxType.Assembly.GetType(boxType.FullName.Replace("&", ""));
                }

                if (PendingReplacements.Variables.TryGetValue(node, out var be))
                {
                    if(box && be == null)
                    {

                        var pe = YExpression.Parameter(typeof(Box<>).MakeGenericType(boxType));
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
                var n = YExpression.Parameter(typeof(Box<>).MakeGenericType(boxType));
                var bp = new BoxParamter
                {
                    Parent = pn,
                    ParentParameter = pp,
                    Index = index,
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
