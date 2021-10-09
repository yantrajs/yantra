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
                var pl = new AstExpression?[array.Elements.Count];
                var e = array.Elements.GetEnumerator();
                int i = 0;
                while(e.MoveNext(out var item))
                {
                    pl[i++] = item.ToPattern();
                }
                return new AstArrayPattern(array.Start, array.End, ArraySpan<AstExpression?>.From(pl));
            }

            static AstExpression LiteralToPattern(AstObjectLiteral literal)
            {

                var pl = new ObjectProperty[literal.Properties.Count];
                var e = literal.Properties.GetEnumerator();
                int i = 0;
                while (e.MoveNext(out var px))
                {
                    ObjectProperty property;
                    switch (px.Type)
                    {
                        case FastNodeType.SpreadElement:
                            var spe = (px as AstSpreadElement)!;
                            property = new ObjectProperty(null, spe.Argument, null, true);
                            break;
                        case FastNodeType.ClassProperty:
                            var p = (px as AstClassProperty)!;
                            var init = p.Init.ToPattern();
                            if (init.Type == FastNodeType.BinaryExpression && init is AstBinaryExpression be)
                            {
                                property = new ObjectProperty(p.Key, be.Left, be.Right, false, p.Computed);
                            }
                            else
                            {
                                property = new ObjectProperty(p.Key, p.Init.ToPattern(), null, false, p.Computed);
                            }
                            break;
                        default:
                            throw new NotSupportedException();
                    }
                    pl[i++] = property;
                }
                return new AstObjectPattern(literal.Start, literal.End, ArraySpan<ObjectProperty>.From(pl));
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

        public override string ToString()
        {
            if (Init == null)
                return Identifier.ToString();
            return $"{Identifier} = {Init}";
        }

        private static void Fill(FastList<VariableDeclarator> args, AstExpression exp) {
            if(exp.Type == FastNodeType.SequenceExpression && exp is AstSequenceExpression se) {
                var e = se.Expressions.GetEnumerator();
                while(e.MoveNext(out var item)) {
                    args.Add(new VariableDeclarator(item.ToPattern()));
                }
            } else {
                args.Add(new VariableDeclarator(exp.ToPattern()));
            }
        }

        public static ArraySpan<VariableDeclarator> From(FastPool pool, AstExpression node)
        {
            if (node.Type == FastNodeType.EmptyExpression)
                return ArraySpan<VariableDeclarator>.Empty;
            var list = pool.AllocateList<VariableDeclarator>();
            Fill(list, node);
            var result = list.ToSpan();
            list.Clear();
            return result;
        }


        public static ArraySpan<VariableDeclarator> From(FastPool pool, in ArraySpan<AstExpression> nodes)
        {
            var r = new VariableDeclarator[nodes.Length];
            for (int i = 0; i < nodes.Length; i++)
            {
                r[i] = new VariableDeclarator(nodes[i].ToPattern());
            }
            return r.ToArraySpan();
        }
    }
}