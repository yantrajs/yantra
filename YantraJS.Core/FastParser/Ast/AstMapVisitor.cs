using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;
using YantraJS.Core.FastParser;
using YantraJS.Core.FastParser.Ast;
using YantraJS.Parser;

namespace YantraJS.Utils
{
    public abstract class AstMapVisitor<T>
    {
        public bool IsStrictMode { get; set; } = false;

        public bool Debug { get; set; } = true;

        public T Visit(AstNode node) { 
        //    if(node.IsStatement)
        //    {
        //        return VisitStatement(node as AstStatement);
        //    }
        //    return VisitExpression(node as AstExpression);
        //}

        //protected virtual T VisitStatement(AstStatement node)
        //{
        //    return InternalVisit(node);
        //}

        //protected virtual T VisitExpression(AstExpression node)
        //{
        //    return InternalVisit(node);
        //}

        //private T InternalVisit(AstNode node)
        //{
            if (node == null)
                return default;
            switch (node.Type)
            {
                case FastNodeType.ArrayPattern:
					return VisitArrayPattern(node as AstArrayPattern);
                case FastNodeType.Block:
					return VisitBlock(node as AstBlock);
                case FastNodeType.Program:
					return VisitProgram(node as AstProgram);
                case FastNodeType.BreakStatement:
					return VisitBreakStatement(node as AstBreakStatement);
                case FastNodeType.BinaryExpression:
					return VisitBinaryExpression(node as AstBinaryExpression);
                case FastNodeType.VariableDeclaration:
					return VisitVariableDeclaration(node as AstVariableDeclaration);
                case FastNodeType.ExpressionStatement:
					return VisitExpressionStatement(node as AstExpressionStatement);
                case FastNodeType.FunctionExpression:
					return VisitFunctionExpression(node as AstFunctionExpression);
                case FastNodeType.Identifier:
					return VisitIdentifier(node as AstIdentifier);
                case FastNodeType.ObjectPattern:
					return VisitObjectPattern(node as AstObjectPattern);
                case FastNodeType.SpreadElement:
					return VisitSpreadElement(node as AstSpreadElement);
                case FastNodeType.IfStatement:
					return VisitIfStatement(node as AstIfStatement);
                case FastNodeType.WhileStatement:
					return VisitWhileStatement(node as AstWhileStatement);
                case FastNodeType.DoWhileStatement:
					return VisitDoWhileStatement(node as AstDoWhileStatement);
                case FastNodeType.SequenceExpression:
					return VisitSequenceExpression(node as AstSequenceExpression);
                case FastNodeType.ForStatement:
					return VisitForStatement(node as AstForStatement);
                case FastNodeType.ForInStatement:
					return VisitForInStatement(node as AstForInStatement);
                case FastNodeType.ForOfStatement:
					return VisitForOfStatement(node as AstForOfStatement);
                case FastNodeType.ContinueStatement:
					return VisitContinueStatement(node as AstContinueStatement);
                case FastNodeType.ThrowStatement:
					return VisitThrowStatement(node as AstThrowStatement);
                case FastNodeType.TryStatement:
					return VisitTryStatement(node as AstTryStatement);
                case FastNodeType.DebuggerStatement:
					return VisitDebuggerStatement(node as AstDebuggerStatement);
                case FastNodeType.LabeledStatement:
					return VisitLabeledStatement(node as AstLabeledStatement);
                case FastNodeType.Literal:
					return VisitLiteral(node as AstLiteral);
                case FastNodeType.MemberExpression:
					return VisitMemberExpression(node as AstMemberExpression);
                case FastNodeType.ClassStatement:
					return VisitClassStatement(node as AstClassExpression);
                case FastNodeType.SwitchStatement:
					return VisitSwitchStatement(node as AstSwitchStatement);
                case FastNodeType.EmptyExpression:
					return VisitEmptyExpression(node as AstEmptyExpression);
                case FastNodeType.ArrayExpression:
					return VisitArrayExpression(node as AstArrayExpression);
                case FastNodeType.ObjectLiteral:
					return VisitObjectLiteral(node as AstObjectLiteral);
                case FastNodeType.TemplateExpression:
					return VisitTemplateExpression(node as AstTemplateExpression);
                case FastNodeType.UnaryExpression:
					return VisitUnaryExpression(node as AstUnaryExpression);
                case FastNodeType.CallExpression:
					return VisitCallExpression(node as AstCallExpression);
                case FastNodeType.ConditionalExpression:
					return VisitConditionalExpression(node as AstConditionalExpression);
                case FastNodeType.YieldExpression:
					return VisitYieldExpression(node as AstYieldExpression);
                case FastNodeType.ClassProperty:
					return VisitClassProperty(node as AstClassProperty);
                case FastNodeType.ReturnStatement:
					return VisitReturnStatement(node as AstReturnStatement);
                case FastNodeType.NewExpression:
					return VisitNewExpression(node as AstNewExpression);
                case FastNodeType.ImportStatement:
                    return VisitImportStatement(node as AstImportStatement);
                case FastNodeType.ExportStatement:
                    return VisitExportStatement(node as AstExportStatement);
                case FastNodeType.Meta:
                    return VisitMeta(node as AstMeta);
                case FastNodeType.TaggedTemplateExpression:
                    return VisitTaggedTemplateExpression(node as AstTaggedTemplateExpression);
                case FastNodeType.AwaitExpression:
                    return VisitAwaitExpression(node as AstAwaitExpression);
                default:
                    throw new NotImplementedException($"No implementation for {node.Type}");
            }
        }

        protected abstract T VisitAwaitExpression(AstAwaitExpression node);
        protected abstract T VisitTaggedTemplateExpression(AstTaggedTemplateExpression astTaggedTemplateExpression);
        protected abstract T VisitMeta(AstMeta astMeta);
        protected abstract T VisitExportStatement(AstExportStatement astExportStatement);
        protected abstract T VisitImportStatement(AstImportStatement astImportStatement);
        protected abstract T VisitArrayPattern(AstArrayPattern arrayPattern);
        protected abstract T VisitNewExpression(AstNewExpression newExpression);
        protected abstract T VisitReturnStatement(AstReturnStatement returnStatement);
        protected virtual T VisitClassProperty(AstClassProperty property) => default;
        protected abstract T VisitBreakStatement(AstBreakStatement breakStatement);
        protected abstract T VisitLabeledStatement(AstLabeledStatement labeledStatement);
        protected abstract T VisitYieldExpression(AstYieldExpression yieldExpression);
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
        protected abstract T VisitForOfStatement(AstForOfStatement forOfStatement, string label = null);
        protected abstract T VisitForInStatement(AstForInStatement forInStatement, string label = null);
        protected abstract T VisitForStatement(AstForStatement forStatement, string label = null);
        protected abstract T VisitSequenceExpression(AstSequenceExpression sequenceExpression);
        protected abstract T VisitDoWhileStatement(AstDoWhileStatement doWhileStatement, string label = null);
        protected abstract T VisitWhileStatement(AstWhileStatement whileStatement, string label = null);
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
