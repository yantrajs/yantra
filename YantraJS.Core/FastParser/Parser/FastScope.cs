using System.Collections.Generic;
using YantraJS.Core.Core.Storage;

namespace YantraJS.Core.FastParser
{
    public partial class FastScope: LinkedStackItem<FastScope>
    {
        public readonly AstNode Node;

        private StringMap<(string name, FastVariableKind kind)> Variables;

        public FastScope(AstNode node)
        {
            this.Node = node;
        }

        public void CreateVariable(AstExpression d, FastVariableKind kind)
        {
            switch ((d.Type, d))
            {
                case (FastNodeType.Identifier, AstIdentifier id):
                    AddVariable(d.Start, id.Name, kind);
                    break;
                case (FastNodeType.SpreadElement, AstSpreadElement spe):
                    CreateVariable(spe.Argument, kind);
                    break;
                case (FastNodeType.ArrayPattern, AstArrayPattern ap):
                    foreach (var e in ap.Elements)
                    {
                        CreateVariable(e, kind);
                    }
                    break;
                case (FastNodeType.ObjectPattern, AstObjectPattern op):
                    foreach (ObjectProperty p in op.Properties)
                    {
                        CreateVariable(p.Value, kind);
                    }
                    break;
            }

        }

        public void AddVariable(FastToken token, in StringSpan name, FastVariableKind kind = FastVariableKind.Var)
        {
            if (name.IsNullOrWhiteSpace())
                return;

            var n = this;
            while (true)
            {
                if (n.Variables.TryGetValue(name, out var pn))
                {
                    if (pn.kind != FastVariableKind.Var)
                    {
                        throw new FastParseException(token, $"{name} is already defined in current scope");
                    }
                }
                n = n.Parent;
                if (n == null)
                    break;
                if (n.Node.Type == FastNodeType.Block)
                    continue;
                break;
            }

            Variables[name] = (name.Value, FastVariableKind.Var);

        }

        public override void Dispose()
        {
            var list = new List<string>();
            foreach(var node in Variables.AllValues())
            {
                if(node.Value.kind == FastVariableKind.Var)
                {
                    list.Add(node.Value.name);
                }
            }
            if (list.Count == 0)
                return;
            if (Node.IsStatement && Node is AstStatement stmt)
            {
                stmt.HoistingScope = list.ToArray();
                return;
            }
            if(Node.Type == FastNodeType.FunctionExpression && Node is  AstFunctionExpression fx)
            {
                fx.Body.HoistingScope = list.ToArray();
            }
            throw new FastParseException(Node.Start, $"Hoisting not supported in {Node.Type}");
        }
    }
}
