using Esprima.Ast;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;
using YantraJS.Parser;

namespace YantraJS.Utils
{
    public abstract class JSAstVisitor<T>
    {
        public bool IsStrictMode { get; set; } = false;

        public bool Debug { get; set; } = true;

        public virtual T Visit(Node node)
        {
            switch (node.Type)
            {
                case Nodes.AssignmentExpression:
                    return VisitAssignmentExpression(node as AssignmentExpression);
                case Nodes.ArrayExpression:
                    return VisitArrayExpression(node as ArrayExpression);
                    
                case Nodes.AwaitExpression:
                    return VisitAwaitExpression(node as AwaitExpression);
                    
                case Nodes.BlockStatement:
                    return VisitBlockStatement(node as BlockStatement);
                    
                case Nodes.BinaryExpression:
                    return VisitBinaryExpression(node as BinaryExpression);
                    
                case Nodes.BreakStatement:
                    return VisitBreakStatement(node as BreakStatement);
                    
                case Nodes.CallExpression:
                    return VisitCallExpression(node as CallExpression);
                    
                case Nodes.CatchClause:
                    return VisitCatchClause(node as CatchClause);
                    
                case Nodes.ConditionalExpression:
                    return VisitConditionalExpression(node as ConditionalExpression);
                    
                case Nodes.ContinueStatement:
                    return VisitContinueStatement(node as ContinueStatement);
                    
                case Nodes.DoWhileStatement:
                    return VisitDoWhileStatement(node as DoWhileStatement);
                    
                case Nodes.DebuggerStatement:
                    return VisitDebuggerStatement(node as DebuggerStatement);
                    
                case Nodes.EmptyStatement:
                    return VisitEmptyStatement(node as EmptyStatement);
                    
                case Nodes.ExpressionStatement:
                    return VisitExpressionStatement(node as ExpressionStatement);
                    
                case Nodes.ForStatement:
                    return VisitForStatement(node as ForStatement);
                    
                case Nodes.ForInStatement:
                    return VisitForInStatement(node as ForInStatement);
                    
                case Nodes.FunctionDeclaration:
                    return VisitFunctionDeclaration(node as FunctionDeclaration);
                    
                case Nodes.FunctionExpression:
                    return VisitFunctionExpression(node as FunctionExpression);
                    
                case Nodes.Identifier:
                    return VisitIdentifier(node as Identifier);
                    
                case Nodes.IfStatement:
                    return VisitIfStatement(node as IfStatement);
                    
                case Nodes.Literal:
                    return VisitLiteral(node as Literal);
                    
                case Nodes.LabeledStatement:
                    return VisitLabeledStatement(node as LabeledStatement);
                    
                case Nodes.LogicalExpression:
                    return VisitLogicalExpression(node as BinaryExpression);
                    
                case Nodes.MemberExpression:
                    return VisitMemberExpression(node as MemberExpression);
                    
                case Nodes.NewExpression:
                    return VisitNewExpression(node as NewExpression);
                    
                case Nodes.ObjectExpression:
                    return VisitObjectExpression(node as ObjectExpression);
                    
                case Nodes.Program:
                    return VisitProgram(node as Program);
                    
                case Nodes.Property:
                    return VisitProperty(node as Property);
                    
                case Nodes.RestElement:
                    return VisitRestElement(node as RestElement);
                    
                case Nodes.ReturnStatement:
                    return VisitReturnStatement(node as ReturnStatement);
                    
                case Nodes.SequenceExpression:
                    return VisitSequenceExpression(node as SequenceExpression);
                    
                case Nodes.SwitchStatement:
                    return VisitSwitchStatement(node as SwitchStatement);
                    
                case Nodes.SwitchCase:
                    return VisitSwitchCase(node as SwitchCase);

                case Nodes.TemplateElement:
                    return VisitTemplateElement(node as TemplateElement);
                    
                case Nodes.TemplateLiteral:
                    return VisitTemplateLiteral(node as TemplateLiteral);
                    
                case Nodes.ThisExpression:
                    return VisitThisExpression(node as ThisExpression);
                    
                case Nodes.ThrowStatement:
                    return VisitThrowStatement(node as ThrowStatement);
                    
                case Nodes.TryStatement:
                    return VisitTryStatement(node as TryStatement);
                    
                case Nodes.UnaryExpression:
                    return VisitUnaryExpression(node as UnaryExpression);
                    
                case Nodes.UpdateExpression:
                    return VisitUpdateExpression(node as UpdateExpression);
                    
                case Nodes.VariableDeclaration:
                    return VisitVariableDeclaration(node as VariableDeclaration);
                    
                case Nodes.VariableDeclarator:
                    return VisitVariableDeclarator(node as VariableDeclarator);
                    
                case Nodes.WhileStatement:
                    return VisitWhileStatement(node as WhileStatement);
                    
                case Nodes.WithStatement:
                    return VisitWithStatement(node as WithStatement);
                    
                case Nodes.ArrayPattern:
                    return VisitArrayPattern(node as ArrayPattern);
                    
                case Nodes.AssignmentPattern:
                    return VisitAssignmentPattern(node as AssignmentPattern);
                    
                case Nodes.SpreadElement:
                    return VisitSpreadElement(node as SpreadElement);
                    
                case Nodes.ObjectPattern:
                    return VisitObjectPattern(node as ObjectPattern);
                    
                case Nodes.ArrowParameterPlaceHolder:
                    return VisitArrowParameterPlaceHolder(node as ArrowParameterPlaceHolder);
                    
                case Nodes.MetaProperty:
                    return VisitMetaProperty(node as MetaProperty);
                    
                case Nodes.Super:
                    return VisitSuper(node as Super);
                    
                case Nodes.TaggedTemplateExpression:
                    return VisitTaggedTemplateExpression(node as TaggedTemplateExpression);
                    
                case Nodes.YieldExpression:
                    return VisitYieldExpression(node as YieldExpression);
                    
                case Nodes.ArrowFunctionExpression:
                    return VisitArrowFunctionExpression(node as ArrowFunctionExpression);
                    
                case Nodes.ClassBody:
                    return VisitClassBody(node as ClassBody);
                    
                case Nodes.ClassDeclaration:
                    return VisitClassDeclaration(node as ClassDeclaration);
                    
                case Nodes.ForOfStatement:
                    return VisitForOfStatement(node as ForOfStatement);
                    
                case Nodes.MethodDefinition:
                    return VisitMethodDefinition(node as MethodDefinition);
                    
                case Nodes.ImportSpecifier:
                    return VisitImportSpecifier(node as ImportSpecifier);
                    
                case Nodes.ImportDefaultSpecifier:
                    return VisitImportDefaultSpecifier(node as ImportDefaultSpecifier);
                    
                case Nodes.ImportNamespaceSpecifier:
                    return VisitImportNamespaceSpecifier(node as ImportNamespaceSpecifier);
                    
                case Nodes.Import:
                    return VisitImport(node as Import);
                    
                case Nodes.ImportDeclaration:
                    return VisitImportDeclaration(node as ImportDeclaration);
                case Nodes.ExportSpecifier:
                    return VisitExportSpecifier(node as ExportSpecifier);
                case Nodes.ExportNamedDeclaration:
                    return VisitExportNamedDeclaration(node as ExportNamedDeclaration);
                case Nodes.ExportAllDeclaration:
                    return VisitExportAllDeclaration(node as ExportAllDeclaration);
                case Nodes.ExportDefaultDeclaration:
                    return VisitExportDefaultDeclaration(node as ExportDefaultDeclaration);
                case Nodes.ClassExpression:
                    return VisitClassExpression(node as ClassExpression);
                default:
                    return VisitUnknownNode(node);
            }
        }

        public virtual T DebugNode(Node node, T result)
        {
            return result;
        }

        public List<T> Visit(in NodeList<Expression> list)
        {
            var r = new List<T>(list.Count);
            foreach(var exp in list)
            {
                r.Add(VisitExpression(exp));
            }
            return r;
        }
        //public List<T> Visit(in NodeList<Statement> list)
        //{
        //    var r = new List<T>(list.Count);
        //    foreach (var exp in list)
        //    {
        //        r.Add(VisitStatement(exp));
        //    }
        //    return r;
        //}


        protected virtual T VisitStatement(Statement statement)
        {
            switch (statement.Type)
            {
                case Nodes.BlockStatement:
                    return VisitBlockStatement(statement as BlockStatement);
                case Nodes.BreakStatement:
                    return VisitBreakStatement(statement as BreakStatement);
                    
                case Nodes.ContinueStatement:
                    return VisitContinueStatement(statement as ContinueStatement);
                    
                case Nodes.DoWhileStatement:
                    return VisitDoWhileStatement(statement as DoWhileStatement);
                    
                case Nodes.DebuggerStatement:
                    return VisitDebuggerStatement(statement as DebuggerStatement);
                    
                case Nodes.EmptyStatement:
                    return VisitEmptyStatement(statement as EmptyStatement);
                    
                case Nodes.ExpressionStatement:
                    return VisitExpressionStatement(statement as ExpressionStatement);
                    
                case Nodes.ForStatement:
                    return VisitForStatement(statement as ForStatement);
                    
                case Nodes.ForInStatement:
                    return VisitForInStatement(statement as ForInStatement);
                    
                case Nodes.ForOfStatement:
                    return VisitForOfStatement(statement as ForOfStatement);
                    
                case Nodes.FunctionDeclaration:
                    return VisitFunctionDeclaration(statement as FunctionDeclaration);
                    
                case Nodes.IfStatement:
                    return VisitIfStatement(statement as IfStatement);
                    
                case Nodes.LabeledStatement:
                    return VisitLabeledStatement(statement as LabeledStatement);
                    
                case Nodes.ReturnStatement:
                    return VisitReturnStatement(statement as ReturnStatement);
                    
                case Nodes.SwitchStatement:
                    return VisitSwitchStatement(statement as SwitchStatement);
                    
                case Nodes.ThrowStatement:
                    return VisitThrowStatement(statement as ThrowStatement);
                    
                case Nodes.TryStatement:
                    return VisitTryStatement(statement as TryStatement);
                    
                case Nodes.VariableDeclaration:
                    return VisitVariableDeclaration(statement as VariableDeclaration);
                    
                case Nodes.WhileStatement:
                    return VisitWhileStatement(statement as WhileStatement);
                    
                case Nodes.WithStatement:
                    return VisitWithStatement(statement as WithStatement);
                    
                case Nodes.Program:
                    return VisitProgram(statement as Program);
                    
                case Nodes.CatchClause:
                    return VisitCatchClause(statement as CatchClause);

                case Nodes.ClassDeclaration:
                    return VisitClassDeclaration(statement as ClassDeclaration);
                case Nodes.ExportSpecifier:
                    return VisitExportSpecifier(statement as ExportSpecifier);
                case Nodes.ExportNamedDeclaration:
                    return VisitExportNamedDeclaration(statement as ExportNamedDeclaration);
                case Nodes.ExportAllDeclaration:
                    return VisitExportAllDeclaration(statement as ExportAllDeclaration);
                case Nodes.ExportDefaultDeclaration:
                    return VisitExportDefaultDeclaration(statement as ExportDefaultDeclaration);
                case Nodes.ImportDeclaration:
                    return VisitImportDeclaration(statement as ImportDeclaration);
                case Nodes.ImportDefaultSpecifier:
                    return VisitImportDefaultSpecifier(statement as ImportDefaultSpecifier);
                case Nodes.ImportSpecifier:
                    return VisitImportSpecifier(statement as ImportSpecifier);
                case Nodes.ImportNamespaceSpecifier:
                    return VisitImportNamespaceSpecifier(statement as ImportNamespaceSpecifier);
                default:
                    return VisitUnknownNode(statement);
                    
            }
        }

        protected abstract T VisitProgram(Program program);

        protected virtual T VisitUnknownNode(Node node)
        {
            throw new NotImplementedException($"AST visitor doesn't support nodes of type {node.Type}, you can override VisitUnknownNode to handle this case.");
        }

        protected abstract T VisitCatchClause(CatchClause catchClause);

        protected abstract T VisitFunctionDeclaration(FunctionDeclaration functionDeclaration);

        protected abstract T VisitWithStatement(WithStatement withStatement);

        protected abstract T VisitWhileStatement(WhileStatement whileStatement, string label = null);

        protected abstract T VisitVariableDeclaration(VariableDeclaration variableDeclaration);

        protected abstract T VisitTryStatement(TryStatement tryStatement);

        protected abstract T VisitThrowStatement(ThrowStatement throwStatement);

        protected abstract T VisitSwitchStatement(SwitchStatement switchStatement);

        protected abstract T VisitSwitchCase(SwitchCase switchCase);

        protected abstract T VisitReturnStatement(ReturnStatement returnStatement);

        protected abstract T VisitLabeledStatement(LabeledStatement labeledStatement);

        protected abstract T VisitIfStatement(IfStatement ifStatement);

        protected abstract T VisitEmptyStatement(EmptyStatement emptyStatement);

        protected abstract T VisitDebuggerStatement(DebuggerStatement debuggerStatement);

        protected abstract T VisitExpressionStatement(ExpressionStatement expressionStatement);

        protected abstract T VisitForStatement(ForStatement forStatement, string label = null);

        protected abstract T VisitForInStatement(ForInStatement forInStatement, string label = null);

        protected abstract T VisitDoWhileStatement(DoWhileStatement doWhileStatement, string label = null);

        protected virtual T VisitExpression(Expression expression)
        {
            try
            {
                switch (expression.Type)
                {
                    case Nodes.AssignmentExpression:
                        return VisitAssignmentExpression(expression as AssignmentExpression);
                    case Nodes.ArrayExpression:
                        return VisitArrayExpression(expression as ArrayExpression);
                    case Nodes.BinaryExpression:
                        return VisitBinaryExpression(expression as BinaryExpression);
                    case Nodes.CallExpression:
                        return VisitCallExpression(expression as CallExpression);
                    case Nodes.ConditionalExpression:
                        return VisitConditionalExpression(expression as ConditionalExpression);

                    case Nodes.FunctionExpression:
                        return VisitFunctionExpression(expression as FunctionExpression);

                    case Nodes.Identifier:
                        return VisitIdentifier(expression as Identifier);

                    case Nodes.Literal:
                        return VisitLiteral(expression as Literal);

                    case Nodes.LogicalExpression:
                        return VisitLogicalExpression(expression as BinaryExpression);

                    case Nodes.MemberExpression:
                        return VisitMemberExpression(expression as MemberExpression);

                    case Nodes.NewExpression:
                        return VisitNewExpression(expression as NewExpression);

                    case Nodes.ObjectExpression:
                        return VisitObjectExpression(expression as ObjectExpression);

                    case Nodes.SequenceExpression:
                        return VisitSequenceExpression(expression as SequenceExpression);

                    case Nodes.ThisExpression:
                        return VisitThisExpression(expression as ThisExpression);

                    case Nodes.UpdateExpression:
                        return VisitUpdateExpression(expression as UpdateExpression);

                    case Nodes.UnaryExpression:
                        return VisitUnaryExpression(expression as UnaryExpression);

                    case Nodes.ArrowFunctionExpression:
                        return VisitArrowFunctionExpression(expression as ArrowFunctionExpression);
                    case Nodes.YieldExpression:
                        return VisitYieldExpression(expression as YieldExpression);
                    case Nodes.AwaitExpression:
                        return VisitAwaitExpression(expression as AwaitExpression);
                    case Nodes.TemplateLiteral:
                        return VisitTemplateLiteral(expression as TemplateLiteral);
                    case Nodes.ClassExpression:
                        return VisitClassExpression(expression as ClassExpression);
                    case Nodes.TaggedTemplateExpression:
                        return VisitTaggedTemplateExpression(expression as TaggedTemplateExpression);
                    case Nodes.MetaProperty:
                        return VisitMetaProperty(expression as MetaProperty);
                    default:
                        // return VisitUnknownNode(expression);
                        throw new NotImplementedException($"{expression.GetType().FullName}");

                }
            }catch (Exception ex) when (!(ex is CompilerException))
            {
                var p = expression.Location.Start;
                throw new CompilerException($"Failed to parse at {p.Line},{p.Column}\r\n{ex}", ex);
            }
        }

        protected abstract T VisitArrowFunctionExpression(ArrowFunctionExpression arrowFunctionExpression);

        protected abstract T VisitUnaryExpression(UnaryExpression unaryExpression);

        protected abstract T VisitUpdateExpression(UpdateExpression updateExpression);

        protected abstract T VisitThisExpression(ThisExpression thisExpression);

        protected abstract T VisitSequenceExpression(SequenceExpression sequenceExpression);

        protected abstract T VisitObjectExpression(ObjectExpression objectExpression);

        protected abstract T VisitNewExpression(NewExpression newExpression);

        protected abstract T VisitMemberExpression(MemberExpression memberExpression);

        protected abstract T VisitLogicalExpression(BinaryExpression binaryExpression);

        protected abstract T VisitLiteral(Literal literal);
        protected abstract T VisitIdentifier(Identifier identifier);

        protected abstract T VisitFunctionExpression(IFunction function);

        protected abstract T VisitClassExpression(ClassExpression classExpression);

        protected abstract T VisitExportDefaultDeclaration(ExportDefaultDeclaration exportDefaultDeclaration);

        protected abstract T VisitExportAllDeclaration(ExportAllDeclaration exportAllDeclaration);

        protected abstract T VisitExportNamedDeclaration(ExportNamedDeclaration exportNamedDeclaration);

        protected abstract T VisitExportSpecifier(ExportSpecifier exportSpecifier);

        protected abstract T VisitImport(Import import);

        protected abstract T VisitImportDeclaration(ImportDeclaration importDeclaration);

        protected abstract T VisitImportNamespaceSpecifier(ImportNamespaceSpecifier importNamespaceSpecifier);

        protected abstract T VisitImportDefaultSpecifier(ImportDefaultSpecifier importDefaultSpecifier);

        protected abstract T VisitImportSpecifier(ImportSpecifier importSpecifier);

        protected abstract T VisitMethodDefinition(MethodDefinition methodDefinitions);

        protected abstract T VisitForOfStatement(ForOfStatement forOfStatement, string  label = null);

        protected abstract T VisitClassDeclaration(ClassDeclaration classDeclaration);

        protected abstract T VisitClassBody(ClassBody classBody);

        protected abstract T VisitYieldExpression(YieldExpression yieldExpression);

        protected abstract T VisitTaggedTemplateExpression(TaggedTemplateExpression taggedTemplateExpression);

        protected abstract T VisitSuper(Super super);
        protected abstract T VisitMetaProperty(MetaProperty metaProperty);

        protected abstract T VisitArrowParameterPlaceHolder(ArrowParameterPlaceHolder arrowParameterPlaceHolder);

        protected abstract T VisitObjectPattern(ObjectPattern objectPattern);

        protected abstract T VisitSpreadElement(SpreadElement spreadElement);

        protected abstract T VisitAssignmentPattern(AssignmentPattern assignmentPattern);

        protected abstract T VisitArrayPattern(ArrayPattern arrayPattern);

        protected abstract T VisitVariableDeclarator(VariableDeclarator variableDeclarator);

        protected abstract T VisitTemplateLiteral(TemplateLiteral templateLiteral);

        protected abstract T VisitTemplateElement(TemplateElement templateElement);

        protected abstract T VisitRestElement(RestElement restElement);

        protected abstract T VisitProperty(Property property);

        protected abstract T VisitAwaitExpression(AwaitExpression awaitExpression);

        protected abstract T VisitConditionalExpression(ConditionalExpression conditionalExpression);

        protected abstract T VisitCallExpression(CallExpression callExpression);

        protected abstract T VisitBinaryExpression(BinaryExpression binaryExpression);

        protected abstract T VisitArrayExpression(ArrayExpression arrayExpression);

        protected abstract T VisitAssignmentExpression(AssignmentExpression assignmentExpression);

        protected abstract T VisitContinueStatement(ContinueStatement continueStatement);
       
        protected abstract T VisitBreakStatement(BreakStatement breakStatement);

        protected abstract T VisitBlockStatement(BlockStatement blockStatement);
    }
}
