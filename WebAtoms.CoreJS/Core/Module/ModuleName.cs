using System;
using System.Threading;

namespace WebAtoms.CoreJS.Core
{
    public struct ModuleName
    {

        public readonly string Name;
        public readonly uint Id;
        private ModuleName(string name, uint id)
        {
            this.Id = id;
            this.Name = name;
        }

        private static int uid = 0;

        private static ConcurrentStringTrie<ModuleName> names
            = new ConcurrentStringTrie<ModuleName>();

        public static implicit operator ModuleName(string value)
        {
            return names.GetOrCreate(value, () => new ModuleName(value, (uint)Interlocked.Increment(ref uid)));
        }


    }
}
