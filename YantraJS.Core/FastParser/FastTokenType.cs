using System.Threading;

namespace YantraJS.Core.FastParser
{
    public readonly struct FastTokenType
    {
        public readonly int Id;

        private static int nextId = 0;

        public static bool operator == (in FastTokenType left, in FastTokenType right)
        {
            return left.Id == right.Id;
        }
        public static bool operator !=(in FastTokenType left, in FastTokenType right)
        {
            return left.Id != right.Id;
        }

        public override bool Equals(object obj)
        {
            return obj is  FastTokenType ft ? Id == ft.Id : false;
        }

        public override int GetHashCode()
        {
            return Id;
        }

    }
}
