using System.Collections.Generic;
using System.Linq;
using YantraJS.Core.Core.Storage;

namespace YantraJS.Core.FastParser
{
    public partial class FastScopeItem: LinkedStackItem<FastScopeItem>
    {
        private StringMap<(StringSpan name, FastVariableKind kind)> Variables;
        private readonly FastToken token;
        public readonly FastNodeType NodeType;
        private readonly FastPool pool;
        public FastScopeItem(FastToken token, FastNodeType nodeType, FastPool pool)
        {
            this.token = token;
            this.NodeType = nodeType;
            this.pool = pool;
        }

        //public void CreateVariable(AstExpression d, FastVariableKind kind)
        //{
        //    switch ((d.Type, d))
        //    {
        //        case (FastNodeType.Identifier, AstIdentifier id):
        //            AddVariable(d.Start, id.Name, kind);
        //            break;
        //        case (FastNodeType.SpreadElement, AstSpreadElement spe):
        //            CreateVariable(spe.Argument, kind);
        //            break;
        //        case (FastNodeType.ArrayPattern, AstArrayPattern ap):
        //            foreach (var e in ap.Elements)
        //            {
        //                CreateVariable(e, kind);
        //            }
        //            break;
        //        case (FastNodeType.ObjectPattern, AstObjectPattern op):
        //            foreach (ObjectProperty p in op.Properties)
        //            {
        //                CreateVariable(p.Value, kind);
        //            }
        //            break;
        //    }

        //}

        public void AddVariable(FastToken token, 
            in StringSpan name, 
            FastVariableKind kind = FastVariableKind.Var, 
            bool throwError = true)
        {
            if (name.IsNullOrWhiteSpace())
                return;

            var n = this;

            if (n.Variables.TryGetValue(name, out var pn)) {
                if (pn.kind != FastVariableKind.Var) {
                    if (throwError)
                    {
                        throw new FastParseException(token, $"{name} is already defined in current scope at {token.Start}");
                    }
                    return;
                }
            }

            // all `var` variables must be hoisted to
            // to top most scope
            if (kind == FastVariableKind.Var) {
                while (true) {
                    if (n.Parent == null)
                        break;
                    if (n.NodeType == FastNodeType.Block
                        && n.Parent.NodeType == FastNodeType.Block) {
                        n = n.Parent;
                        continue;
                    }
                    break;
                }
            }

            n.Variables.Put(name) = (name, kind);

        }

        public ArraySpan<StringSpan> GetVariables()
        {
            var list = pool.AllocateList<StringSpan>();
            try
            {
                foreach (var node in Variables.AllValues())
                {
                    list.Add(node.Value.name);
                }
                if (list.Count == 0)
                    return ArraySpan<StringSpan>.Empty;
                return list.ToSpan();
            } finally
            {
                list.Clear();
            }

        }
    }
}
