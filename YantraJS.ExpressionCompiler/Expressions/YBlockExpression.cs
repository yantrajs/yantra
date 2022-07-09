#nullable enable
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using YantraJS.Core;

namespace YantraJS.Expressions
{
    public class YBlockExpression: YExpression
    {
        public readonly IFastEnumerable<YParameterExpression> Variables;
        public readonly IFastEnumerable<YExpression> Expressions;

        public YBlockExpression(IFastEnumerable<YParameterExpression>? variables,
            IFastEnumerable<YExpression> expressions)
            :base(YExpressionType.Block, expressions.Last().Type)
        {
            this.Variables = variables ?? Sequence<YParameterExpression>.Empty;
            if (this.Variables.Any(v => v == null))
                throw new ArgumentNullException();
            this.Expressions = expressions;
        }

        public override void Print(IndentedTextWriter writer)
        {
            writer.WriteLine("{");
            writer.Indent++;
            {
                var en = Variables.GetFastEnumerator();
                while(en.MoveNext(out var v))
                    writer.WriteLine($"{v.Type.GetFriendlyName()} {v.Name};");
            }
            {
                var en = Expressions.GetFastEnumerator();
                while(en.MoveNext(out var exp))
                {
                    exp.Print(writer);
                    writer.WriteLine(";");
                }
            }
            writer.Indent--;
            writer.WriteLine("}");
        }

        public IEnumerable<YParameterExpression> FlattenVariables
        {
            get
            {
                var ve = Variables.GetFastEnumerator();
                while(ve.MoveNext(out var v))
                    yield return v;
                var ee = Expressions.GetFastEnumerator();
                while(ee.MoveNext(out var s))
                {
                    if(s.NodeType == YExpressionType.Block && s is YBlockExpression b)
                    {
                        foreach (var v in b.FlattenVariables)
                            yield return v;
                    }
                }
            }
        }

        public IEnumerable<(YExpression expression, bool isLast)> FlattenExpressions
        {
            get
            {
                var l = Expressions.Count - 1;
                var en = Expressions.GetFastEnumerator();
                while (en.MoveNext(out var e, out var i))
                {
                    bool last = i == l;
                    // var e = Expressions[i];
                    if (e.NodeType == YExpressionType.Block && e is YBlockExpression b) {
                        foreach (var (item, isLast) in b.FlattenExpressions)
                            yield return (item, isLast && last);
                        continue;
                    }

                    yield return (e, last);
                }
            }
        }

        public Enumerator Enumerate() => new Enumerator(this.Expressions);

        public ref struct Enumerator
        {
            private IFastEnumerator<YExpression> expressions;
            private int last;

            public Enumerator(IFastEnumerable<YExpression> expressions)
            {
                this.expressions = expressions.GetFastEnumerator();
                this.last = expressions.Count - 1;
            }

            public bool MoveNext(out YExpression? exp, out bool isLast)
            {
                if(expressions.MoveNext(out exp, out var index))
                {
                    isLast = index == last;
                    return true;
                }
                //if((this.index++)<= last)
                //{
                //    isLast = last == this.index;
                //    exp = expressions[index];
                //    return true;
                //}
                isLast = false;
                exp = default;
                return false;
            }
        }
    }
}