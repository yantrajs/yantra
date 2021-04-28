using System.Reflection.Emit;
using System.Threading;

namespace YantraJS.Core
{
    public class ILWriterLabel
    {
        public readonly Label Value;

        public readonly int ID;

        public int Offset;

        private static int nextID = 1;

        public ILWriterLabel(Label value)
        {
            this.Value = value;
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
