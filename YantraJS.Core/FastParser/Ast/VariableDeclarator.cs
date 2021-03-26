namespace YantraJS.Core.FastParser
{
    public readonly struct VariableDeclarator
    {
        public readonly AstExpression Identifier;
        public readonly AstExpression Init;

        public VariableDeclarator(AstExpression identifier, AstExpression init)
        {
            Identifier = identifier;
            Init = init;
        }

        public static VariableDeclarator FromNode(AstExpression node)
        {
            switch (node.Type)
            {
                case FastNodeType.BinaryExpression:
                    var b = node as AstBinaryExpression;
                    if (b.Operator != TokenTypes.Assign)
                    {
                        throw new FastParseException(b.Start, "Invalid parameter information");
                    }
                    return new VariableDeclarator(b.Left, b.Right);
                case FastNodeType.Identifier:
                    return new VariableDeclarator(node, null);
                default:
                    throw new FastParseException(node.Start, "Identifier expected");
            }
        }

        public static ArraySpan<VariableDeclarator> From(AstExpression node)
        {
            var r = new VariableDeclarator[4];
            r[0] = FromNode(node);
            return r.ToArraySpan(1);
        }


        public static ArraySpan<VariableDeclarator> From(ArraySpan<AstExpression> nodes)
        {
            var r = new VariableDeclarator[nodes.Length];
            for (int i = 0; i < nodes.Length; i++)
            {
                r[i] = FromNode(nodes[i]);
            }
            return r.ToArraySpan();
        }
    }
}