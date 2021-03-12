using System;
namespace YantraJS.Core.FastParser
{
    public abstract class FastExpression : FastNode
    {
        protected FastExpression(FastNode parent, FastNodeType nodeType) : base(parent, nodeType)
        {
        }

        public static FastExpression Read(FastNode parent, FastTokenStream stream)
        {
            throw new NotImplementedException();
        }
    }
}
