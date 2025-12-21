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

    public void Add(JSIL il, uint uVal)
    {
        instructions.Add(new JSInstruction(il, uVal));
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

/// <summary>
/// 
/// 1. The reason we cannot evaluate expression directly is because we want to implement generators.
/// And to do that, we need to save the execution stack, which is not possible in C# unless we
/// 
/// 2. It is possible to save IL builder output and skip this step next time. 
/// </summary>
internal class JSILAstVisitor : AstMapVisitor<JSILBuilder>
{
    private readonly JSILBuilder builder;

    public JSILAstVisitor(JSILBuilder builder)
    {
        this.builder = builder;
    }

    protected override JSILBuilder VisitArrayExpression(AstArrayExpression arrayExpression)
    {
        // [ a, b, ... c, ... d, e];
        // need to add spread to an array...
        // lets count min lenght
        var total = arrayExpression.Elements.Count;
        builder.Add(JSIL.NAry, (uint)total);
        foreach (var e in arrayExpression.Elements) {
            if (e.IsSpreadElement(out var s))
            {
                this.Visit(s.Argument);
                builder.Add(JSIL.ApdS);
            }
            else
            {
                this.Visit(e);
                builder.Add(JSIL.Apd);
            }
        }
        return builder;
    }

    protected override JSILBuilder VisitArrayPattern(AstArrayPattern arrayPattern)
    {
        throw new NotImplementedException();
    }

    protected override JSILBuilder VisitAwaitExpression(AstAwaitExpression node)
    {
        this.Visit(node.Argument);
        builder.Add(JSIL.Awit);
        return builder;
    }

    protected override JSILBuilder VisitBinaryExpression(AstBinaryExpression binaryExpression)
    {
        switch (binaryExpression.Operator)
        {
            case TokenTypes.Assign:
                break;
            case TokenTypes.AssignMultiply:
                break;
            case TokenTypes.AssignDivide:
                break;
            case TokenTypes.AssignMod:
                break;
            case TokenTypes.AssignAdd:
                break;
            case TokenTypes.AssignXor:
                break;
            case TokenTypes.AssignSubtract:
                break;
            case TokenTypes.AssignUnsignedRightShift:
                break;
            case TokenTypes.AssignBitwideAnd:
                break;
            case TokenTypes.AssignBitwideOr:
                break;
            case TokenTypes.AssignRightShift:
                break;
            case TokenTypes.AssignLeftShift:
                break;
            case TokenTypes.AssignCoalesce:
                break;
            case TokenTypes.AssignPower:
                break;
            case TokenTypes.Negate:
                break;
            case TokenTypes.Power:
                break;
            case TokenTypes.Multiply:
                break;
            case TokenTypes.Divide:
                break;
            case TokenTypes.Mod:
                break;
            case TokenTypes.Plus:
                break;
            case TokenTypes.Minus:
                break;
            case TokenTypes.LeftShift:
                break;
            case TokenTypes.RightShift:
                break;
            case TokenTypes.UnsignedRightShift:
                break;
            case TokenTypes.Less:
                break;
            case TokenTypes.LessOrEqual:
                break;
            case TokenTypes.Greater:
                break;
            case TokenTypes.GreaterOrEqual:
                break;
            case TokenTypes.In:
                break;
            case TokenTypes.InstanceOf:
                break;
            case TokenTypes.Equal:
                break;
            case TokenTypes.NotEqual:
                break;
            case TokenTypes.StrictlyEqual:
                break;
            case TokenTypes.StrictlyNotEqual:
                break;
            case TokenTypes.Coalesce:
                break;
            case TokenTypes.BitwiseAnd:
                break;
            case TokenTypes.Xor:
                break;
            case TokenTypes.BitwiseOr:
                break;
            case TokenTypes.BooleanAnd:
                break;
            case TokenTypes.BooleanOr:
                break;
            case TokenTypes.QuestionMark:
                break;
            case TokenTypes.BitwiseNot:
                break;
            case TokenTypes.QuestionDot:
                break;
            case TokenTypes.Dot:
                break;
            case TokenTypes.Increment:
                break;
            case TokenTypes.Decrement:
                break;
            case TokenTypes.OptionalCall:
                break;
            case TokenTypes.OptionalIndex:
                break;
        }
        return builder;
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
        var falseLabel = builder.Label("false");
        var endLabel = builder.Label("end");
        this.Visit(conditionalExpression.Test);
        builder.Add(JSIL.JmpF, falseLabel);
        this.Visit(conditionalExpression.True);
        builder.Add(JSIL.Jump, endLabel);
        builder.Apply(falseLabel);
        this.Visit(conditionalExpression.False);
        builder.Apply(endLabel);
        return builder;
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
        var start = builder.Label("while-start");
        builder.Apply(start);
        this.Visit(doWhileStatement.Body);
        this.Visit(doWhileStatement.Test);
        builder.Add(JSIL.JmpT, start);
        return builder;
    }

    protected override JSILBuilder VisitEmptyExpression(AstEmptyExpression emptyExpression)
    {
        return builder;
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
        var falseLabel = builder.Label("false");
        this.Visit(ifStatement.Test);
        builder.Add(JSIL.JmpF, falseLabel);
        this.Visit(ifStatement.True);
        if (ifStatement.False != null)
        {
            var endLabel = builder.Label("end");
            builder.Add(JSIL.Jump, endLabel);
            this.Visit(ifStatement.False);
            builder.Apply(endLabel);
        } else
        {
            builder.Apply(falseLabel);
        }
        return builder;
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
        if (returnStatement.Argument != null)
        {
            this.Visit(returnStatement.Argument);
            builder.Add(JSIL.RetV);
            return builder;
        }
        builder.Add(JSIL.RetU);
        return builder;
    }

    protected override JSILBuilder VisitSequenceExpression(AstSequenceExpression sequenceExpression)
    {
        // we should pop till last expression..
        var fe = sequenceExpression.Expressions.GetFastEnumerator();
        if (fe.MoveNext(out var e))
        {
            this.Visit(e);
        }
        while(fe.MoveNext(out e))
        {
            builder.Add(JSIL.Pop);
            this.Visit(e);
        }
        return builder;
    }

    protected override JSILBuilder VisitSpreadElement(AstSpreadElement spreadElement)
    {
        // program should not reach here...
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
        this.Visit(throwStatement.Argument);
        builder.Add(JSIL.Thro);
        return builder;
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

        this.Visit(whileStatement.Test);
        builder.Add(JSIL.JmpF, end);
        builder.Apply(start);
        this.Visit(whileStatement.Body);
        builder.Add(JSIL.Jump, start);
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
