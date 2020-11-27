using System.Collections.Generic;
using YantraJS.Core;
using YantraJS.Core.Core.Storage;

namespace YantraJS
{
    public class StringArray
    {
        private StringMap<uint> map;
        
        public List<StringSpan> List { get; } = new List<StringSpan>();
        
        public uint GetOrAdd(in StringSpan code)
        {
            if (map.TryGetValue(code, out var i))
                return i;
            i = (uint)List.Count;
            map[code] = i;
            List.Add(code);
            return i;
        }
    }

}
