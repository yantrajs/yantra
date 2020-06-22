using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace WebAtoms.CoreJS.Core
{
    internal class KeyStrings
    {

        public static KeyStrings Instance = new KeyStrings();

        private BinaryCharMap<uint> map = new BinaryCharMap<uint>();

        private static int NextID = 3;

        private KeyStrings()
        {
            map.Save("toString", 1);
            map.Save("name", 2);
        }

        public uint GetValue(string key)
        {
            lock(this) return map.GetOrCreate(key, () => (uint)Interlocked.Increment(ref NextID));
        }

    }
}
