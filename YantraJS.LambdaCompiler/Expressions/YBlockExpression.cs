#nullable enable
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;

namespace YantraJS.Expressions
{
    public class YBlockExpression: YExpression
    {
        public readonly YParameterExpression[] Variables;
        public readonly YExpression[] Expressions;

        public YBlockExpression(IEnumerable<YParameterExpression>? variables, 
            YExpression[] expressions)
            :base(YExpressionType.Block, expressions.Last().Type)
        {
            this.Variables = variables?.ToArray() ?? (new YParameterExpression[] { });
            if (this.Variables.Any(v => v == null))
                throw new ArgumentNullException();
            this.Expressions = expressions;
        }

        public override void Print(IndentedTextWriter writer)
        {
            writer.WriteLine("{");
            writer.Indent++;
            foreach (var v in Variables)
            {
                writer.WriteLine($"{v.Type.GetFriendlyName()} {v.Name};");
            }
            foreach (var exp in Expressions)
            {
                exp.Print(writer);
                writer.WriteLine(";");
            }
            writer.Indent--;
            writer.WriteLine("}");
        }

        public IEnumerable<YParameterExpression> FlattenVariables
        {
            get
            {
                foreach (var v in Variables)
                    yield return v;
                foreach(var s in Expressions)
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
                var l = Expressions.Length;
                for (int i = 0; i < l; i++)
                {
                    bool last = i == l - 1;
                    var e = Expressions[i];
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
            private YExpression[] expressions;
            private int index;
            private int last;

            public Enumerator(YExpression[] expressions)
            {
                this.expressions = expressions;
                index = -1;
                this.last = expressions.Length - 1;
            }

            public bool MoveNext(out YExpression? exp, out bool isLast)
            {
                if((this.index++)<= last)
                {
                    isLast = last == this.index;
                    exp = expressions[index];
                    return true;
                }
                isLast = false;
                exp = default;
                return false;
            }
        }
    }
}