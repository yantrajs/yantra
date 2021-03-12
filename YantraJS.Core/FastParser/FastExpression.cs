using System;
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
            throw new NotImplementedException();
        }

        private static FastExpression ReadKeyword(FastNode parent, FastTokenStream stream, FastKeywords hint)
        {
            switch (hint)
            {
                case FastKeywords.function:
                    return new FastFunctionExpression(parent, stream);
            }
            return null;
        }
    }
}
