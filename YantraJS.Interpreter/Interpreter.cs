using System;
using System.Collections.Generic;
using System.Linq;
using YantraJS.Core;
using YantraJS.Core.FastParser;
using YantraJS.Utils;

namespace YantraJS.Interpreter;

public class Interpreter
{

}

public enum ExecutionState { 
    None,
    Executing,
    Yield
};

public class FunctionScope
{
    public string Calle { get; set; }

    public BlockScope Variables = new BlockScope();

    public Arguments Arguments;

    public FunctionScope Caller { get; set; }

}

public class BlockScope
{
    SAUint32Map<JSValue> Variables;

    public BlockScope Parent;
}

public delegate JSValue ExecuteStep(FunctionScope scope);

public class AstToStepVisitor : AstMapVisitor<ExecuteStep>
{
    protected override ExecuteStep VisitArrayExpression(AstArrayExpression arrayExpression)
    {
        var fe = arrayExpression.Elements.GetFastEnumerator();
        List<ExecuteStep> sequence = new();
        while(fe.MoveNext(out var item))
        {
            sequence.Add(this.Visit(item));
        }
        return (fc) => new JSArray(sequence.Select(x => x(fc)));
    }

    protected override ExecuteStep VisitArrayPattern(AstArrayPattern arrayPattern)
    {
        throw new NotImplementedException();
    }

    protected override ExecuteStep VisitAwaitExpression(AstAwaitExpression node)
    {
        throw new NotImplementedException();
    }

    protected override ExecuteStep VisitBinaryExpression(AstBinaryExpression binaryExpression)
    {
        var left = this.Visit(binaryExpression.Left);
        var right = this.Visit(binaryExpression.Right);
        switch (binaryExpression.Operator)
        {
            case TokenTypes.BooleanAnd:
                return (fc) => (left(fc).BooleanValue && right(fc).BooleanValue) ? JSBoolean.True : JSBoolean.False;
            case TokenTypes.BooleanOr:
                return (fc) => (left(fc).BooleanValue || right(fc).BooleanValue) ? JSBoolean.True : JSBoolean.False;
            case TokenTypes.BitwiseAnd:
                return (fc) => (left(fc).BitwiseAnd(right(fc)));
            case TokenTypes.BitwiseOr:
                return (fc) => (left(fc).BitwiseOr(right(fc)));
            case TokenTypes.Plus:
                return (fc) => (left(fc).AddValue(right(fc)));
            case TokenTypes.Minus:
                return (fc) => (left(fc).Subtract(right(fc)));
            case TokenTypes.Mod:
                return (fc) => (left(fc).Modulo(right(fc)));
            case TokenTypes.Multiply:
                return (fc) => (left(fc).Multiply(right(fc)));
            case TokenTypes.NotEqual:
                return (fc) => !left(fc).Equals(right(fc)) ? JSBoolean.True : JSBoolean.False ;
            case TokenTypes.Equal:
                return (fc) => left(fc).Equals(right(fc)) ? JSBoolean.True : JSBoolean.False;
            case TokenTypes.StrictlyNotEqual:
                return (fc) => !left(fc).StrictEquals(right(fc)) ? JSBoolean.True : JSBoolean.False;
            case TokenTypes.StrictlyEqual:
                return (fc) => left(fc).StrictEquals(right(fc)) ? JSBoolean.True : JSBoolean.False;
            case TokenTypes.Assign:
                return "=";
        }
        throw new NotImplementedException();
    }

    protected override ExecuteStep VisitBlock(AstBlock block)
    {
        var statements = block.Statements.Select(this.Visit);
        return (fc) => {
            fc.Enter();
        };
    }

    protected override ExecuteStep VisitBreakStatement(AstBreakStatement breakStatement)
    {
        throw new NotImplementedException();
    }

    protected override ExecuteStep VisitCallExpression(AstCallExpression callExpression)
    {
        throw new NotImplementedException();
    }

    protected override ExecuteStep VisitClassStatement(AstClassExpression classStatement)
    {
        throw new NotImplementedException();
    }

    protected override ExecuteStep VisitConditionalExpression(AstConditionalExpression conditionalExpression)
    {
        throw new NotImplementedException();
    }

    protected override ExecuteStep VisitContinueStatement(AstContinueStatement continueStatement)
    {
        throw new NotImplementedException();
    }

    protected override ExecuteStep VisitDebuggerStatement(AstDebuggerStatement debuggerStatement)
    {
        throw new NotImplementedException();
    }

    protected override ExecuteStep VisitDoWhileStatement(AstDoWhileStatement doWhileStatement, string label = null)
    {
        throw new NotImplementedException();
    }

    protected override ExecuteStep VisitEmptyExpression(AstEmptyExpression emptyExpression)
    {
        throw new NotImplementedException();
    }

    protected override ExecuteStep VisitExportStatement(AstExportStatement astExportStatement)
    {
        throw new NotImplementedException();
    }

    protected override ExecuteStep VisitExpressionStatement(AstExpressionStatement expressionStatement)
    {
        throw new NotImplementedException();
    }

    protected override ExecuteStep VisitForInStatement(AstForInStatement forInStatement, string label = null)
    {
        throw new NotImplementedException();
    }

    protected override ExecuteStep VisitForOfStatement(AstForOfStatement forOfStatement, string label = null)
    {
        throw new NotImplementedException();
    }

    protected override ExecuteStep VisitForStatement(AstForStatement forStatement, string label = null)
    {
        throw new NotImplementedException();
    }

    protected override ExecuteStep VisitFunctionExpression(AstFunctionExpression functionExpression)
    {
        throw new NotImplementedException();
    }

    protected override ExecuteStep VisitIdentifier(AstIdentifier identifier)
    {
        throw new NotImplementedException();
    }

    protected override ExecuteStep VisitIfStatement(AstIfStatement ifStatement)
    {
        throw new NotImplementedException();
    }

    protected override ExecuteStep VisitImportStatement(AstImportStatement astImportStatement)
    {
        throw new NotImplementedException();
    }

    protected override ExecuteStep VisitLabeledStatement(AstLabeledStatement labeledStatement)
    {
        throw new NotImplementedException();
    }

    protected override ExecuteStep VisitLiteral(AstLiteral literal)
    {
        throw new NotImplementedException();
    }

    protected override ExecuteStep VisitMemberExpression(AstMemberExpression memberExpression)
    {
        throw new NotImplementedException();
    }

    protected override ExecuteStep VisitMeta(AstMeta astMeta)
    {
        throw new NotImplementedException();
    }

    protected override ExecuteStep VisitNewExpression(AstNewExpression newExpression)
    {
        throw new NotImplementedException();
    }

    protected override ExecuteStep VisitObjectLiteral(AstObjectLiteral objectLiteral)
    {
        throw new NotImplementedException();
    }

    protected override ExecuteStep VisitObjectPattern(AstObjectPattern objectPattern)
    {
        throw new NotImplementedException();
    }

    protected override ExecuteStep VisitProgram(AstProgram program)
    {
        throw new NotImplementedException();
    }

    protected override ExecuteStep VisitReturnStatement(AstReturnStatement returnStatement)
    {
        throw new NotImplementedException();
    }

    protected override ExecuteStep VisitSequenceExpression(AstSequenceExpression sequenceExpression)
    {
        throw new NotImplementedException();
    }

    protected override ExecuteStep VisitSpreadElement(AstSpreadElement spreadElement)
    {
        throw new NotImplementedException();
    }

    protected override ExecuteStep VisitSwitchStatement(AstSwitchStatement switchStatement)
    {
        throw new NotImplementedException();
    }

    protected override ExecuteStep VisitTaggedTemplateExpression(AstTaggedTemplateExpression astTaggedTemplateExpression)
    {
        throw new NotImplementedException();
    }

    protected override ExecuteStep VisitTemplateExpression(AstTemplateExpression templateExpression)
    {
        throw new NotImplementedException();
    }

    protected override ExecuteStep VisitThrowStatement(AstThrowStatement throwStatement)
    {
        throw new NotImplementedException();
    }

    protected override ExecuteStep VisitTryStatement(AstTryStatement tryStatement)
    {
        throw new NotImplementedException();
    }

    protected override ExecuteStep VisitUnaryExpression(AstUnaryExpression unaryExpression)
    {
        throw new NotImplementedException();
    }

    protected override ExecuteStep VisitVariableDeclaration(AstVariableDeclaration variableDeclaration)
    {
        throw new NotImplementedException();
    }

    protected override ExecuteStep VisitWhileStatement(AstWhileStatement whileStatement, string label = null)
    {
        throw new NotImplementedException();
    }

    protected override ExecuteStep VisitYieldExpression(AstYieldExpression yieldExpression)
    {
        throw new NotImplementedException();
    }
}
