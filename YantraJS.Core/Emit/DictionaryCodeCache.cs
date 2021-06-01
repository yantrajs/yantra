#if !WEBATOMS
// using FastExpressionCompiler;
#endif
using YantraJS.Core;
using YantraJS.Core.Core.Storage;
using YantraJS.Runtime;

namespace YantraJS.Emit
{
    public class DictionaryCodeCache : ICodeCache
    {
        private static ConcurrentStringMap<JSFunctionDelegate> cache
            = ConcurrentStringMap<JSFunctionDelegate>.Create();

        public static ICodeCache Current = new DictionaryCodeCache();

        public JSFunctionDelegate GetOrCreate(in JSCode code)
        {
            var compiler = code.Compiler;
            return cache.GetOrCreate(code.Key, (k) => {
                var  exp = compiler();
                return exp.CompileWithNestedLambdas();
                // return exp.CompileDynamic();
            });
        }

    }
}
