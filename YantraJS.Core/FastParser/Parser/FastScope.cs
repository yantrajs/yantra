namespace YantraJS.Core.FastParser
{
    public partial class FastScope: LinkedStack<FastScopeItem>
    {
        // private readonly FastPool pool;

        public FastScope()
        {
            // this.pool = pool;
        }

        public FastScopeItem Push(FastToken token, FastNodeType nodeType)
        {
            var n = new FastScopeItem(nodeType);
            return Push(n);
        }
    }
}
