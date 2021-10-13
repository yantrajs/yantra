using System.Reflection.Emit;
using System.Threading;

namespace YantraJS.Core
{
    public class ILWriterLabel
    {
        public readonly Label Value;
        public readonly ILTryBlock TryBlock;
        public readonly string ID;

        public int Offset;

        private static int nextID = 1;

        public ILWriterLabel(Label value, string label, ILTryBlock tryBlock)
        {
            this.Value = value;
            this.TryBlock = tryBlock;
            this.ID = $"{label ?? "LABEL"}_{ Interlocked.Increment(ref nextID)}";
        }

        public override string ToString()
        {
            return ID;
        }
    }
}
