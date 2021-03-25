using Esprima.Ast;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;
using YantraJS.Core.FastParser;
using YantraJS.Parser;

namespace YantraJS.Utils
{
    public abstract class AstVisitor<T>
    {
        public bool IsStrictMode { get; set; } = false;

        public bool Debug { get; set; } = true;

        public virtual T Visit(AstNode node)
        {
            switch ((node.Type,node))
            {
                case (FastNodeType.Block, AstBlock Block):
                    return VisitBlock(Block);
                case (FastNodeType.Program, AstProgram Program):
                    return VisitProgram(Program);
                case (FastNodeType.BinaryExpression, AstBinaryExpression BinaryExpression):
                    return VisitBinaryExpression(BinaryExpression);
                case (FastNodeType.VariableDeclaration, AstVariableDeclaration VariableDeclaration):
                    return VisitVariableDeclaration(VariableDeclaration);
                case (FastNodeType.ExpressionStatement, AstExpressionStatement ExpressionStatement):
                    return VisitExpressionStatement(ExpressionStatement);
                case (FastNodeType.FunctionExpression, AstFunctionExpression FunctionExpression):
                    return VisitFunctionExpression(FunctionExpression);
                case (FastNodeType.Identifier, AstIdentifier Identifier):
                    return VisitIdentifier(Identifier);
                case (FastNodeType.ObjectPattern, AstObjectPattern ObjectPattern):
                    return VisitObjectPattern(ObjectPattern);
                case (FastNodeType.SpreadElement, AstSpreadElement SpreadElement):
                    return VisitSpreadElement(SpreadElement);
                case (FastNodeType.IfStatement, AstIfStatement IfStatement):
                    return VisitIfStatement(IfStatement);
                case (FastNodeType.WhileStatement, AstWhileStatement WhileStatement):
                    return VisitWhileStatement(WhileStatement);
                case (FastNodeType.DoWhileStatement, AstDoWhileStatement DoWhileStatement):
                    return VisitDoWhileStatement(DoWhileStatement);
                case (FastNodeType.SequenceExpression, AstSequenceExpression SequenceExpression):
                    return VisitSequenceExpression(SequenceExpression);
                case (FastNodeType.ForStatement, AstForStatement ForStatement):
                    return VisitForStatement(ForStatement);
                case (FastNodeType.ForInStatement, AstForInStatement ForInStatement):
                    return VisitForInStatement(ForInStatement);
                case (FastNodeType.ForOfStatement, AstForOfStatement ForOfStatement):
                    return VisitForOfStatement(ForOfStatement);
                case (FastNodeType.ContinueStatement, AstContinueStatement ContinueStatement):
                    return VisitContinueStatement(ContinueStatement);
                case (FastNodeType.ThrowStatement, AstThrowStatement ThrowStatement):
                    return VisitThrowStatement(ThrowStatement);
                case (FastNodeType.TryStatement, AstTryStatement TryStatement):
                    return VisitTryStatement(TryStatement);
                case (FastNodeType.DebuggerStatement, AstDebuggerStatement DebuggerStatement):
                    return VisitDebuggerStatement(DebuggerStatement);
                case (FastNodeType.Literal, AstLiteral Literal):
                    return VisitLiteral(Literal);
                case (FastNodeType.MemberExpression, AstMemberExpression MemberExpression):
                    return VisitMemberExpression(MemberExpression);
                case (FastNodeType.ClassStatement, AstClassExpression ClassStatement):
                    return VisitClassStatement(ClassStatement);
                case (FastNodeType.SwitchStatement, AstSwitchStatement SwitchStatement):
                    return VisitSwitchStatement(SwitchStatement);
                case (FastNodeType.EmptyExpression, AstEmptyExpression EmptyExpression):
                    return VisitEmptyExpression(EmptyExpression);
                case (FastNodeType.ArrayExpression, AstArrayExpression ArrayExpression):
                    return VisitArrayExpression(ArrayExpression);
                case (FastNodeType.ObjectLiteral, AstObjectLiteral ObjectLiteral):
                    return VisitObjectLiteral(ObjectLiteral);
                case (FastNodeType.TemplateExpression, AstTemplateExpression TemplateExpression):
                    return VisitTemplateExpression(TemplateExpression);
                case (FastNodeType.UnaryExpression, AstUnaryExpression UnaryExpression):
                    return VisitUnaryExpression(UnaryExpression);
                case (FastNodeType.CallExpression, AstCallExpression CallExpression):
                    return VisitCallExpression(CallExpression);
                case (FastNodeType.ConditionalExpression, AstConditionalExpression ConditionalExpression):
                    return VisitConditionalExpression(ConditionalExpression);
                default:
                    throw new NotImplementedException();
            }
        }

        protected abstract T VisitCallExpression(AstCallExpression callExpression);
        protected abstract T VisitUnaryExpression(AstUnaryExpression unaryExpression);
        protected abstract T VisitTemplateExpression(AstTemplateExpression templateExpression);
        protected abstract T VisitObjectLiteral(AstObjectLiteral objectLiteral);
        protected abstract T VisitArrayExpression(AstArrayExpression arrayExpression);
        protected abstract T VisitEmptyExpression(AstEmptyExpression emptyExpression);
        protected abstract T VisitSwitchStatement(AstSwitchStatement switchStatement);
        protected abstract T VisitClassStatement(AstClassExpression classStatement);
        protected abstract T VisitMemberExpression(AstMemberExpression memberExpression);
        protected abstract T VisitLiteral(AstLiteral literal);
        protected abstract T VisitDebuggerStatement(AstDebuggerStatement debuggerStatement);
        protected abstract T VisitTryStatement(AstTryStatement tryStatement);
        protected abstract T VisitThrowStatement(AstThrowStatement throwStatement);
        protected abstract T VisitContinueStatement(AstContinueStatement continueStatement);
        protected abstract T VisitForOfStatement(AstForOfStatement forOfStatement);
        protected abstract T VisitForInStatement(AstForInStatement forInStatement);
        protected abstract T VisitForStatement(AstForStatement forStatement);
        protected abstract T VisitSequenceExpression(AstSequenceExpression sequenceExpression);
        protected abstract T VisitDoWhileStatement(AstDoWhileStatement doWhileStatement);
        protected abstract T VisitWhileStatement(AstWhileStatement whileStatement);
        protected abstract T VisitIfStatement(AstIfStatement ifStatement);
        protected abstract T VisitSpreadElement(AstSpreadElement spreadElement);
        protected abstract T VisitObjectPattern(AstObjectPattern objectPattern);
        protected abstract T VisitIdentifier(AstIdentifier identifier);
        protected abstract T VisitFunctionExpression(AstFunctionExpression functionExpression);
        protected abstract T VisitExpressionStatement(AstExpressionStatement expressionStatement);
        protected abstract T VisitVariableDeclaration(AstVariableDeclaration variableDeclaration);
        protected abstract T VisitBinaryExpression(AstBinaryExpression binaryExpression);
        protected abstract T VisitProgram(AstProgram program);
        protected abstract T VisitBlock(AstBlock block);
        protected abstract T VisitConditionalExpression(AstConditionalExpression conditionalExpression);
    }
}
