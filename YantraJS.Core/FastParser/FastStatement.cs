using System;

namespace YantraJS.Core.FastParser
{

    public abstract class FastStatement: FastNode
    {
        protected FastStatement(FastNode parent, FastNodeType type, FastTokenStream stream): base(parent, type, stream ,  true)
        {

        }

        private static FastStatement ReadKeyword(FastKeywords keyword, FastNode parent, FastTokenStream stream)
        {
            switch (keyword)
            {

                /**
                    * Variable Declarations
                    */
                case FastKeywords.let:
                    return new FastVariableDeclaration(parent, stream, isLet: true);
                case FastKeywords.var:
                    return new FastVariableDeclaration(parent, stream);
                case FastKeywords.@const:
                    return new FastVariableDeclaration(parent, stream, isConst: true);

                case FastKeywords.function:
                    return new FastExpressionStatement(parent, stream, keyword);

                default:
                    throw stream.Unexpected();
            }

        }

        private static FastStatement Read(FastNode parent, FastTokenStream stream)
        {
            var token = stream.Current;
            switch (token.Type)
            {
                case TokenTypes.EOF:
                    return null;

                case TokenTypes.Identifier:
                    if (stream.Keywords.IsKeyword(in token.Span, out var keyword))
                        return ReadKeyword(keyword, parent, stream);
                    break;

                    // IIFE
                case TokenTypes.BracketStart:
                    return new FastExpressionStatement(parent, stream);

                case TokenTypes.CurlyBracketEnd:
                    stream.Consume();
                    return null;

                    // nested bracket added...
                    // new block scope...
                case TokenTypes.CurlyBracketStart:
                    stream.Consume();
                    return FastBlock.Read(parent, stream);
            }
            return null;

        }

        internal static bool ParseStatement(FastNode parent, FastTokenStream stream, out FastStatement statement)
        {
            statement = Read(parent, stream);
            return statement != null;
        }
    }
}
