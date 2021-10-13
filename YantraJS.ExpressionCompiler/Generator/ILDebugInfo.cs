#nullable enable
using YantraJS.Expressions;

namespace YantraJS.Generator
{
    public readonly struct ILDebugInfo
    {
        public readonly int ILOffset;
        public readonly Position Start;
        public readonly Position End;

        public ILDebugInfo(int ilOffset, in Position start, in Position end)
        {
            this.ILOffset = ilOffset;
            this.Start = start;
            this.End = end;
        }
    }
}
