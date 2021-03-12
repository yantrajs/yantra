using System;

namespace YantraJS.Core.FastParser
{
    [Flags]
    public enum FastNodeType
    {
        Node,
        Block,
        Program,
        BinaryExpression,
        VariableDeclaration
    }
}
