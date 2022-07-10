using System;
using YantraJS.Core.Core.Storage;

namespace YantraJS.Core.FastParser
{
    public class FastKeywordMap
    {

        public static FastKeywordMap Instance = new FastKeywordMap();

        private static ConcurrentStringMap<FastKeywords> list = ConcurrentStringMap<FastKeywords>.Create();

        static FastKeywordMap()
        {
            foreach (var name in Enum.GetNames(typeof(FastKeywords)))
            {
                var value = (FastKeywords)Enum.Parse(typeof(FastKeywords), name);
                list[name] = value;
            }
        }

        protected FastKeywordMap() { }

        public virtual bool IsKeyword(in StringSpan k, out FastKeywords keyword)
        {
            return list.TryGetValue(k, out keyword);
        }
    }
}
