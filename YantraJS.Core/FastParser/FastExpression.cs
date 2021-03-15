using System;
using System.Collections.Generic;

namespace YantraJS.Core.FastParser
{
    public abstract class FastExpression : FastNode
    {
        protected FastExpression(FastNode parent, FastNodeType nodeType, FastTokenStream stream) 
            : base(parent, nodeType, stream)
        {
        }

        public static FastExpression Read(FastNode parent, FastTokenStream stream, FastKeywords hint = FastKeywords.none)
        {
            if(hint != FastKeywords.none)
            {
                // when hint is not none, token is already consumed...
                return ReadKeyword(parent, stream, hint);
            }

            var current = stream.Current;
            if (current.IsKeyword)
            {
                return ReadKeyword(parent, stream, current.Keyword);
            }

            switch (current.Type)
            {
                // (
                case TokenTypes.BracketStart:
                    stream.Consume();
                    List<FastNode> nodes = new List<FastNode>();
                    do
                    {
                        nodes.Add(Read(parent, stream));
                        if (stream.CheckAndConsume(TokenTypes.BracketEnd))
                            break;
                        if (stream.CheckAndConsume(TokenTypes.Comma))
                            continue;
                    }
                    while (true);

                    if (stream.CheckAndConsume(TokenTypes.Lambda))
                    {
                        // lambda function..
                    }

                    break;

            }


            throw new NotImplementedException();
        }

        private static FastExpression ReadKeyword(FastNode parent, FastTokenStream stream, FastKeywords hint)
        {
            switch (hint)
            {
                case FastKeywords.function:
                    return new FastFunctionExpression(parent, stream);
                case FastKeywords.async:
                    stream.Consume();
                    return new FastFunctionExpression(parent, stream, true);
            }
            return null;
        }
    }
}
