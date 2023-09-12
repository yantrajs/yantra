#nullable enable
using System;
namespace YantraJS.Core.FastParser
{
    internal static class ExpressionPatternExtensions
    {

        public static AstExpression ToPattern(this AstExpression exp)
        {
            switch (exp.Type)
            {
                case FastNodeType.SpreadElement: 
                    var spe = exp as AstSpreadElement;
                    if (spe!.Argument.Type  != FastNodeType.Identifier)
                    {
                        throw new FastParseException(exp.Start, $"Invalid spread pattern at {exp.Start.Start}");
                    }
                    return spe;
                case FastNodeType.ObjectLiteral: 
                    var l = exp as AstObjectLiteral;
                    return LiteralToPattern(l!);
                case FastNodeType.ArrayExpression: 
                    var ae = exp as AstArrayExpression;
                    return ArrayToPattern(ae!);
                case FastNodeType.Identifier:
                case FastNodeType.ArrayPattern:
                case FastNodeType.ObjectPattern:
                    return exp;
                case FastNodeType.BinaryExpression:
                    var be = (exp as AstBinaryExpression)!;
                    if (be.Operator == TokenTypes.Assign)
                    {
                        return new AstBinaryExpression(be.Left.ToPattern(), be.Operator, be.Right);
                    }
                    break;

            }
            throw new FastParseException(exp.Start,
                $"Invalid pattern of {exp.Type} at {exp.Start.Start}");

            static AstExpression ArrayToPattern(AstArrayExpression array)
            {
                var pl = new Sequence<AstExpression>(array.Elements.Count);
                var e = array.Elements.GetFastEnumerator();
                while(e.MoveNext(out var item))
                {
                    pl.Add(item.ToPattern());
                }
                return new AstArrayPattern(array.Start, array.End, pl);
            }

            static AstExpression LiteralToPattern(AstObjectLiteral literal)
            {

                var pl = new Sequence<ObjectProperty>(literal.Properties.Count);
                var e = literal.Properties.GetFastEnumerator();
                while (e.MoveNext(out var px))
                {
                    ref var property = ref pl.AddGetRef();
                    switch (px.Type)
                    {
                        case FastNodeType.SpreadElement:
                            var spe = (px as AstSpreadElement)!;
                            property = new ObjectProperty(null, spe.Argument, null, true);
                            break;
                        case FastNodeType.ClassProperty:
                            var p = (px as AstClassProperty)!;
                            if (p.Kind == AstPropertyKind.Data)
                            {
                                property = new ObjectProperty(p.Key, p.Key, p.Init, false, p.Computed);
                                break;
                            }
                            var init = p.Init.ToPattern();
                            if (init.Type == FastNodeType.BinaryExpression && init is AstBinaryExpression be)
                            {
                                property = new ObjectProperty(p.Key, be.Left, be.Right, false, p.Computed);
                            }
                            else
                            {
                                property = new ObjectProperty(p.Key, init, null, false, p.Computed);
                            }
                            break;
                        default:
                            throw new NotSupportedException();
                    }
                    // pl.Add(property);
                }
                return new AstObjectPattern(literal.Start, literal.End, pl);
            }
        }

    }

    public readonly struct VariableDeclarator
    {
        public readonly AstExpression Identifier;
        public readonly AstExpression? Init;

        public VariableDeclarator(AstExpression identifier, AstExpression? init = null)
        {
            if (identifier.Type == FastNodeType.BinaryExpression && identifier is AstBinaryExpression be)
            {
                Identifier = be.Left;
                Init = be.Right;
            }
            else
            {
                Identifier = identifier;
                Init = init;
            }
        }

        public bool Equals(VariableDeclarator d)
        {
            return this.Identifier == d.Identifier && this.Init == d.Init;
        }

        public override string ToString()
        {
            if (Init == null)
                return Identifier.ToString();
            return $"{Identifier} = {Init}";
        }

        private static void Fill(Sequence<VariableDeclarator> args, AstExpression exp) {
            if(exp.Type == FastNodeType.SequenceExpression && exp is AstSequenceExpression se) {
                var e = se.Expressions.GetFastEnumerator();
                while(e.MoveNext(out var item)) {
                    args.Add(new VariableDeclarator(item.ToPattern()));
                }
            } else {
                args.Add(new VariableDeclarator(exp.ToPattern()));
            }
        }

        public static IFastEnumerable<VariableDeclarator> From(AstExpression node)
        {
            if (node.Type == FastNodeType.EmptyExpression)
                return Sequence<VariableDeclarator>.Empty;
            var list = new Sequence<VariableDeclarator>();
            Fill(list, node);
            return list;
        }


        public static Sequence<VariableDeclarator> From(in ArraySpan<AstExpression> nodes)
        {
            var r = new Sequence<VariableDeclarator>(nodes.Length);
            for (int i = 0; i < nodes.Length; i++)
            {
                r.Add(new VariableDeclarator(nodes[i].ToPattern()));
            }
            return r;
        }
    }
}