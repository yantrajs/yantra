namespace YantraJS.Core.FastParser
{
    public class FastBinaryExpression: FastExpression
    {
        public FastBinaryExpression(FastNode parent, FastTokenStream stream): base(parent, FastNodeType.BinaryExpression, stream)
        {

        }

        internal override void Read(FastTokenStream stream)
        {
            throw new System.NotImplementedException();
        }
    }
}
