using Esprima.Ast;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace WebAtoms.CoreJS.Utils
{
    public abstract class JSAstVisitor<T>
    {
        public bool IsStrictMode { get; set; } = false;

        public virtual T Visit(Node node)
        {
            switch (node.Type)
            {
                case Nodes.AssignmentExpression:
                    return VisitAssignmentExpression(node.As<AssignmentExpression>());
                case Nodes.ArrayExpression:
                    return VisitArrayExpression(node.As<ArrayExpression>());
                    
                case Nodes.AwaitExpression:
                    return VisitAwaitExpression(node.As<AwaitExpression>());
                    
                case Nodes.BlockStatement:
                    return VisitBlockStatement(node.As<BlockStatement>());
                    
                case Nodes.BinaryExpression:
                    return VisitBinaryExpression(node.As<BinaryExpression>());
                    
                case Nodes.BreakStatement:
                    return VisitBreakStatement(node.As<BreakStatement>());
                    
                case Nodes.CallExpression:
                    return VisitCallExpression(node.As<CallExpression>());
                    
                case Nodes.CatchClause:
                    return VisitCatchClause(node.As<CatchClause>());
                    
                case Nodes.ConditionalExpression:
                    return VisitConditionalExpression(node.As<ConditionalExpression>());
                    
                case Nodes.ContinueStatement:
                    return VisitContinueStatement(node.As<ContinueStatement>());
                    
                case Nodes.DoWhileStatement:
                    return VisitDoWhileStatement(node.As<DoWhileStatement>());
                    
                case Nodes.DebuggerStatement:
                    return VisitDebuggerStatement(node.As<DebuggerStatement>());
                    
                case Nodes.EmptyStatement:
                    return VisitEmptyStatement(node.As<EmptyStatement>());
                    
                case Nodes.ExpressionStatement:
                    return VisitExpressionStatement(node.As<ExpressionStatement>());
                    
                case Nodes.ForStatement:
                    return VisitForStatement(node.As<ForStatement>());
                    
                case Nodes.ForInStatement:
                    return VisitForInStatement(node.As<ForInStatement>());
                    
                case Nodes.FunctionDeclaration:
                    return VisitFunctionDeclaration(node.As<FunctionDeclaration>());
                    
                case Nodes.FunctionExpression:
                    return VisitFunctionExpression(node.As<FunctionExpression>());
                    
                case Nodes.Identifier:
                    return VisitIdentifier(node.As<Identifier>());
                    
                case Nodes.IfStatement:
                    return VisitIfStatement(node.As<IfStatement>());
                    
                case Nodes.Literal:
                    return VisitLiteral(node.As<Literal>());
                    
                case Nodes.LabeledStatement:
                    return VisitLabeledStatement(node.As<LabeledStatement>());
                    
                case Nodes.LogicalExpression:
                    return VisitLogicalExpression(node.As<BinaryExpression>());
                    
                case Nodes.MemberExpression:
                    return VisitMemberExpression(node.As<MemberExpression>());
                    
                case Nodes.NewExpression:
                    return VisitNewExpression(node.As<NewExpression>());
                    
                case Nodes.ObjectExpression:
                    return VisitObjectExpression(node.As<ObjectExpression>());
                    
                case Nodes.Program:
                    return VisitProgram(node.As<Program>());
                    
                case Nodes.Property:
                    return VisitProperty(node.As<Property>());
                    
                case Nodes.RestElement:
                    return VisitRestElement(node.As<RestElement>());
                    
                case Nodes.ReturnStatement:
                    return VisitReturnStatement(node.As<ReturnStatement>());
                    
                case Nodes.SequenceExpression:
                    return VisitSequenceExpression(node.As<SequenceExpression>());
                    
                case Nodes.SwitchStatement:
                    return VisitSwitchStatement(node.As<SwitchStatement>());
                    
                case Nodes.SwitchCase:
                    return VisitSwitchCase(node.As<SwitchCase>());
                    
                case Nodes.TemplateElement:
                    return VisitTemplateElement(node.As<TemplateElement>());
                    
                case Nodes.TemplateLiteral:
                    return VisitTemplateLiteral(node.As<TemplateLiteral>());
                    
                case Nodes.ThisExpression:
                    return VisitThisExpression(node.As<ThisExpression>());
                    
                case Nodes.ThrowStatement:
                    return VisitThrowStatement(node.As<ThrowStatement>());
                    
                case Nodes.TryStatement:
                    return VisitTryStatement(node.As<TryStatement>());
                    
                case Nodes.UnaryExpression:
                    return VisitUnaryExpression(node.As<UnaryExpression>());
                    
                case Nodes.UpdateExpression:
                    return VisitUpdateExpression(node.As<UpdateExpression>());
                    
                case Nodes.VariableDeclaration:
                    return VisitVariableDeclaration(node.As<VariableDeclaration>());
                    
                case Nodes.VariableDeclarator:
                    return VisitVariableDeclarator(node.As<VariableDeclarator>());
                    
                case Nodes.WhileStatement:
                    return VisitWhileStatement(node.As<WhileStatement>());
                    
                case Nodes.WithStatement:
                    return VisitWithStatement(node.As<WithStatement>());
                    
                case Nodes.ArrayPattern:
                    return VisitArrayPattern(node.As<ArrayPattern>());
                    
                case Nodes.AssignmentPattern:
                    return VisitAssignmentPattern(node.As<AssignmentPattern>());
                    
                case Nodes.SpreadElement:
                    return VisitSpreadElement(node.As<SpreadElement>());
                    
                case Nodes.ObjectPattern:
                    return VisitObjectPattern(node.As<ObjectPattern>());
                    
                case Nodes.ArrowParameterPlaceHolder:
                    return VisitArrowParameterPlaceHolder(node.As<ArrowParameterPlaceHolder>());
                    
                case Nodes.MetaProperty:
                    return VisitMetaProperty(node.As<MetaProperty>());
                    
                case Nodes.Super:
                    return VisitSuper(node.As<Super>());
                    
                case Nodes.TaggedTemplateExpression:
                    return VisitTaggedTemplateExpression(node.As<TaggedTemplateExpression>());
                    
                case Nodes.YieldExpression:
                    return VisitYieldExpression(node.As<YieldExpression>());
                    
                case Nodes.ArrowFunctionExpression:
                    return VisitArrowFunctionExpression(node.As<ArrowFunctionExpression>());
                    
                case Nodes.ClassBody:
                    return VisitClassBody(node.As<ClassBody>());
                    
                case Nodes.ClassDeclaration:
                    return VisitClassDeclaration(node.As<ClassDeclaration>());
                    
                case Nodes.ForOfStatement:
                    return VisitForOfStatement(node.As<ForOfStatement>());
                    
                case Nodes.MethodDefinition:
                    return VisitMethodDefinition(node.As<MethodDefinition>());
                    
                case Nodes.ImportSpecifier:
                    return VisitImportSpecifier(node.As<ImportSpecifier>());
                    
                case Nodes.ImportDefaultSpecifier:
                    return VisitImportDefaultSpecifier(node.As<ImportDefaultSpecifier>());
                    
                case Nodes.ImportNamespaceSpecifier:
                    return VisitImportNamespaceSpecifier(node.As<ImportNamespaceSpecifier>());
                    
                case Nodes.Import:
                    return VisitImport(node.As<Import>());
                    
                case Nodes.ImportDeclaration:
                    return VisitImportDeclaration(node.As<ImportDeclaration>());
                case Nodes.ExportSpecifier:
                    return VisitExportSpecifier(node.As<ExportSpecifier>());
                case Nodes.ExportNamedDeclaration:
                    return VisitExportNamedDeclaration(node.As<ExportNamedDeclaration>());
                case Nodes.ExportAllDeclaration:
                    return VisitExportAllDeclaration(node.As<ExportAllDeclaration>());
                case Nodes.ExportDefaultDeclaration:
                    return VisitExportDefaultDeclaration(node.As<ExportDefaultDeclaration>());
                case Nodes.ClassExpression:
                    return VisitClassExpression(node.As<ClassExpression>());
                default:
                    return VisitUnknownNode(node);
            }
        }

        protected virtual T VisitStatement(Statement statement)
        {
            switch (statement.Type)
            {
                case Nodes.BlockStatement:
                    return VisitBlockStatement(statement.As<BlockStatement>());
                case Nodes.BreakStatement:
                    return VisitBreakStatement(statement.As<BreakStatement>());
                    
                case Nodes.ContinueStatement:
                    return VisitContinueStatement(statement.As<ContinueStatement>());
                    
                case Nodes.DoWhileStatement:
                    return VisitDoWhileStatement(statement.As<DoWhileStatement>());
                    
                case Nodes.DebuggerStatement:
                    return VisitDebuggerStatement(statement.As<DebuggerStatement>());
                    
                case Nodes.EmptyStatement:
                    return VisitEmptyStatement(statement.As<EmptyStatement>());
                    
                case Nodes.ExpressionStatement:
                    return VisitExpressionStatement(statement.As<ExpressionStatement>());
                    
                case Nodes.ForStatement:
                    return VisitForStatement(statement.As<ForStatement>());
                    
                case Nodes.ForInStatement:
                    return VisitForInStatement(statement.As<ForInStatement>());
                    
                case Nodes.ForOfStatement:
                    return VisitForOfStatement(statement.As<ForOfStatement>());
                    
                case Nodes.FunctionDeclaration:
                    return VisitFunctionDeclaration(statement.As<FunctionDeclaration>());
                    
                case Nodes.IfStatement:
                    return VisitIfStatement(statement.As<IfStatement>());
                    
                case Nodes.LabeledStatement:
                    return VisitLabeledStatement(statement.As<LabeledStatement>());
                    
                case Nodes.ReturnStatement:
                    return VisitReturnStatement(statement.As<ReturnStatement>());
                    
                case Nodes.SwitchStatement:
                    return VisitSwitchStatement(statement.As<SwitchStatement>());
                    
                case Nodes.ThrowStatement:
                    return VisitThrowStatement(statement.As<ThrowStatement>());
                    
                case Nodes.TryStatement:
                    return VisitTryStatement(statement.As<TryStatement>());
                    
                case Nodes.VariableDeclaration:
                    return VisitVariableDeclaration(statement.As<VariableDeclaration>());
                    
                case Nodes.WhileStatement:
                    return VisitWhileStatement(statement.As<WhileStatement>());
                    
                case Nodes.WithStatement:
                    return VisitWithStatement(statement.As<WithStatement>());
                    
                case Nodes.Program:
                    return VisitProgram(statement.As<Program>());
                    
                case Nodes.CatchClause:
                    return VisitCatchClause(statement.As<CatchClause>());
                    
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

        protected abstract T VisitWhileStatement(WhileStatement whileStatement);

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

        protected abstract T VisitForStatement(ForStatement forStatement);

        protected abstract T VisitForInStatement(ForInStatement forInStatement);

        protected abstract T VisitDoWhileStatement(DoWhileStatement doWhileStatement);

        protected virtual T VisitExpression(Expression expression)
        {
            switch (expression.Type)
            {
                case Nodes.AssignmentExpression:
                    return VisitAssignmentExpression(expression.As<AssignmentExpression>());
                case Nodes.ArrayExpression:
                    return VisitArrayExpression(expression.As<ArrayExpression>());
                case Nodes.BinaryExpression:
                    return VisitBinaryExpression(expression.As<BinaryExpression>());
                case Nodes.CallExpression:
                    return VisitCallExpression(expression.As<CallExpression>());
                case Nodes.ConditionalExpression:
                    return VisitConditionalExpression(expression.As<ConditionalExpression>());
                    
                case Nodes.FunctionExpression:
                    return VisitFunctionExpression(expression.As<FunctionExpression>());
                    
                case Nodes.Identifier:
                    return VisitIdentifier(expression.As<Identifier>());
                    
                case Nodes.Literal:
                    return VisitLiteral(expression.As<Literal>());
                    
                case Nodes.LogicalExpression:
                    return VisitLogicalExpression(expression.As<BinaryExpression>());
                    
                case Nodes.MemberExpression:
                    return VisitMemberExpression(expression.As<MemberExpression>());
                    
                case Nodes.NewExpression:
                    return VisitNewExpression(expression.As<NewExpression>());
                    
                case Nodes.ObjectExpression:
                    return VisitObjectExpression(expression.As<ObjectExpression>());
                    
                case Nodes.SequenceExpression:
                    return VisitSequenceExpression(expression.As<SequenceExpression>());
                    
                case Nodes.ThisExpression:
                    return VisitThisExpression(expression.As<ThisExpression>());
                    
                case Nodes.UpdateExpression:
                    return VisitUpdateExpression(expression.As<UpdateExpression>());
                    
                case Nodes.UnaryExpression:
                    return VisitUnaryExpression(expression.As<UnaryExpression>());
                    
                case Nodes.ArrowFunctionExpression:
                    return VisitArrowFunctionExpression(expression.As<ArrowFunctionExpression>());
                    
                default:
                    // return VisitUnknownNode(expression);
                    throw new NotImplementedException();
                    
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

        protected abstract T VisitForOfStatement(ForOfStatement forOfStatement);

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
