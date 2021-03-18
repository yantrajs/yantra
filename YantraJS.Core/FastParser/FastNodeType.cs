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
        VariableDeclaration,
        ExpressionStatement,
        FunctionExpression,
        AssignmentPattern,
        VariableReference,
        Identifier,
        ObjectPattern,
        SpreadElement,
        IfStatement,
        WhileStatement,
        DoWhileStatement,
        SequenceExpression,
        ForStatement,
        ForInStatement,
        ForOfStatement,
        ContinueStatement,
        ThrowStatement,
        TryStatement,
        DebuggerStatement
    }
}
