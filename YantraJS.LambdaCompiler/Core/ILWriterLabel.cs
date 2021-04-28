using System.Reflection.Emit;
using System.Threading;

namespace YantraJS.Core
{
    public class ILWriterLabel
    {
        public readonly Label Value;
        public readonly ILTryBlock TryBlock;
        public readonly int ID;

        public int Offset;

        private static int nextID = 1;

        public ILWriterLabel(Label value, ILTryBlock tryBlock)
        {
            this.Value = value;
            this.TryBlock = tryBlock;
            this.ID = Interlocked.Increment(ref nextID);
        }

        public override string ToString()
        {
            //if(Offset>0)
            //    return $"L_{ID}_{Offset}";
            return $"LABEL_{ID}";
        }
    }
}
