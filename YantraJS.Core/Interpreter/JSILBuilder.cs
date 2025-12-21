using System;
using System.Collections.Generic;
using System.Text;
using YantraJS.Core.FastParser;
using YantraJS.Utils;

namespace YantraJS.Core.Interpreter;


internal class JSILBuilder
{
    public class JSILLabel
    {
        public int Id;
        public int Position = -1;
        public readonly string Label;

        public JSILLabel(string label, int id   )
        {
            this.Label = label;
            Id = id;
        }
    }


    Sequence<JSInstruction> instructions = new Sequence<JSInstruction>();

    int labels = 0;

    public void Apply(JSILLabel label)
    {
        label.Position = this.instructions.Count;
    }

    public void Add(JSIL il)
    {
        instructions.Add(new JSInstruction(il));
    }

    public void Add(JSIL il, JSILLabel label)
    {
        instructions.Add(new JSInstruction(il, label.Id));
    }

    internal JSILLabel Label(string v)
    {
        return new JSILLabel(v, this.labels++);
    }
}

internal class JSILAstVisitor : AstMapVisitor<JSILBuilder>
{
    private readonly JSILBuilder builder;

    public JSILAstVisitor(JSILBuilder builder)
    {
        this.builder = builder;
    }

    protected override JSILBuilder VisitArrayExpression(AstArrayExpression arrayExpression)
    {
        throw new NotImplementedException();
    }

    protected override JSILBuilder VisitArrayPattern(AstArrayPattern arrayPattern)
    {
        throw new NotImplementedException();
    }

    protected override JSILBuilder VisitAwaitExpression(AstAwaitExpression node)
    {
        throw new NotImplementedException();
    }

    protected override JSILBuilder VisitBinaryExpression(AstBinaryExpression binaryExpression)
    {
        throw new NotImplementedException();
    }

    protected override JSILBuilder VisitBlock(AstBlock block)
    {
        throw new NotImplementedException();
    }

    protected override JSILBuilder VisitBreakStatement(AstBreakStatement breakStatement)
    {
        throw new NotImplementedException();
    }

    protected override JSILBuilder VisitCallExpression(AstCallExpression callExpression)
    {
        throw new NotImplementedException();
    }

    protected override JSILBuilder VisitClassStatement(AstClassExpression classStatement)
    {
        throw new NotImplementedException();
    }

    protected override JSILBuilder VisitConditionalExpression(AstConditionalExpression conditionalExpression)
    {
        throw new NotImplementedException();
    }

    protected override JSILBuilder VisitContinueStatement(AstContinueStatement continueStatement)
    {
        throw new NotImplementedException();
    }

    protected override JSILBuilder VisitDebuggerStatement(AstDebuggerStatement debuggerStatement)
    {
        throw new NotImplementedException();
    }

    protected override JSILBuilder VisitDoWhileStatement(AstDoWhileStatement doWhileStatement, string label = null)
    {
        throw new NotImplementedException();
    }

    protected override JSILBuilder VisitEmptyExpression(AstEmptyExpression emptyExpression)
    {
        throw new NotImplementedException();
    }

    protected override JSILBuilder VisitExportStatement(AstExportStatement astExportStatement)
    {
        throw new NotImplementedException();
    }

    protected override JSILBuilder VisitExpressionStatement(AstExpressionStatement expressionStatement)
    {
        throw new NotImplementedException();
    }

    protected override JSILBuilder VisitForInStatement(AstForInStatement forInStatement, string label = null)
    {
        throw new NotImplementedException();
    }

    protected override JSILBuilder VisitForOfStatement(AstForOfStatement forOfStatement, string label = null)
    {
        throw new NotImplementedException();
    }

    protected override JSILBuilder VisitForStatement(AstForStatement forStatement, string label = null)
    {
        throw new NotImplementedException();
    }

    protected override JSILBuilder VisitFunctionExpression(AstFunctionExpression functionExpression)
    {
        throw new NotImplementedException();
    }

    protected override JSILBuilder VisitIdentifier(AstIdentifier identifier)
    {
        throw new NotImplementedException();
    }

    protected override JSILBuilder VisitIfStatement(AstIfStatement ifStatement)
    {
        throw new NotImplementedException();
    }

    protected override JSILBuilder VisitImportStatement(AstImportStatement astImportStatement)
    {
        throw new NotImplementedException();
    }

    protected override JSILBuilder VisitLabeledStatement(AstLabeledStatement labeledStatement)
    {
        throw new NotImplementedException();
    }

    protected override JSILBuilder VisitLiteral(AstLiteral literal)
    {
        throw new NotImplementedException();
    }

    protected override JSILBuilder VisitMemberExpression(AstMemberExpression memberExpression)
    {
        throw new NotImplementedException();
    }

    protected override JSILBuilder VisitMeta(AstMeta astMeta)
    {
        throw new NotImplementedException();
    }

    protected override JSILBuilder VisitNewExpression(AstNewExpression newExpression)
    {
        throw new NotImplementedException();
    }

    protected override JSILBuilder VisitObjectLiteral(AstObjectLiteral objectLiteral)
    {
        throw new NotImplementedException();
    }

    protected override JSILBuilder VisitObjectPattern(AstObjectPattern objectPattern)
    {
        throw new NotImplementedException();
    }

    protected override JSILBuilder VisitProgram(AstProgram program)
    {
        throw new NotImplementedException();
    }

    protected override JSILBuilder VisitReturnStatement(AstReturnStatement returnStatement)
    {
        throw new NotImplementedException();
    }

    protected override JSILBuilder VisitSequenceExpression(AstSequenceExpression sequenceExpression)
    {
        throw new NotImplementedException();
    }

    protected override JSILBuilder VisitSpreadElement(AstSpreadElement spreadElement)
    {
        throw new NotImplementedException();
    }

    protected override JSILBuilder VisitSwitchStatement(AstSwitchStatement switchStatement)
    {
        throw new NotImplementedException();
    }

    protected override JSILBuilder VisitTaggedTemplateExpression(AstTaggedTemplateExpression astTaggedTemplateExpression)
    {
        throw new NotImplementedException();
    }

    protected override JSILBuilder VisitTemplateExpression(AstTemplateExpression templateExpression)
    {
        throw new NotImplementedException();
    }

    protected override JSILBuilder VisitThrowStatement(AstThrowStatement throwStatement)
    {
        throw new NotImplementedException();
    }

    protected override JSILBuilder VisitTryStatement(AstTryStatement tryStatement)
    {
        throw new NotImplementedException();
    }

    protected override JSILBuilder VisitUnaryExpression(AstUnaryExpression unaryExpression)
    {
        throw new NotImplementedException();
    }

    protected override JSILBuilder VisitVariableDeclaration(AstVariableDeclaration variableDeclaration)
    {
        throw new NotImplementedException();
    }

    protected override JSILBuilder VisitWhileStatement(AstWhileStatement whileStatement, string label = null)
    {
        var start = builder.Label("while-start");
        var end = builder.Label("while-end");
        builder.Apply(start);

        this.Visit(whileStatement.Test);

        builder.Add(JSIL.JmpF, end);

        this.Visit(whileStatement.Body);

        builder.Apply(end);
        return builder;
    }

    protected override JSILBuilder VisitYieldExpression(AstYieldExpression yieldExpression)
    {
        var builder = this.Visit(yieldExpression.Argument);
        builder.Add(JSIL.Yild);
        return builder;
    }
}
