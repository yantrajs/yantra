#nullable enable
using System;
namespace YantraJS.Core.FastParser
{
    internal static class ExpressionPatternExtensions
    {

        public static AstExpression ToPattern(this AstExpression? exp)
        {
            if (exp == null)
                throw new FastParseException(null, "Invalid Pattern");
            switch ((exp.Type, exp))
            {
                case (FastNodeType.SpreadElement, AstSpreadElement spe):
                    if(spe.Argument.Type  != FastNodeType.Identifier)
                    {
                        throw new FastParseException(exp.Start, "Invalid spread pattern");
                    }
                    return spe;
                case (FastNodeType.Identifier, AstIdentifier id):
                    return id;
                case (FastNodeType.ObjectLiteral, AstObjectLiteral l):
                    return LiteralToPattern(l);
                case (FastNodeType.ArrayExpression, AstArrayExpression ae):
                    return ArrayToPattern(ae);
                default:
                    throw new FastParseException(exp.Start, "Invalid pattern");
            }
            
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
                    switch ((px.Type, px))
                    {
                        case (FastNodeType.SpreadElement, AstSpreadElement spe):
                            property = new ObjectProperty(null, spe.Argument, true);
                            break;
                        case (FastNodeType.ClassProperty, AstClassProperty p):
                            property = new ObjectProperty(p.Key, p.Init.ToPattern(), false, p.Computed);
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
            Identifier = identifier;
            Init = init;
        }

        public override string ToString()
        {
            if (Init == null)
                return Identifier.ToString();
            return $"{Identifier} = {Init}";
        }

        public static ArraySpan<VariableDeclarator> From(AstExpression node)
        {
            if (node.Type == FastNodeType.EmptyExpression)
                return ArraySpan<VariableDeclarator>.Empty;
            var r = new VariableDeclarator[4];
            r[0] = new VariableDeclarator(node.ToPattern());
            return r.ToArraySpan(1);
        }


        public static ArraySpan<VariableDeclarator> From(in ArraySpan<AstExpression> nodes)
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