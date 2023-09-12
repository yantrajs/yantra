using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using YantraJS.Utils;

namespace YantraJS.Core.FastParser.Ast
{

    public class AstIdentifierReplacer : AstReduce
    {
        private readonly IFastEnumerable<(string from, AstIdentifier temp)> changes;

        private AstIdentifierReplacer(IFastEnumerable<(string from, AstIdentifier temp)> changes)
        {
            this.changes = changes;
        }

        protected override AstNode VisitIdentifier(AstIdentifier identifier)
        {
            var e = changes.GetFastEnumerator();
            while(e.MoveNext(out var item))
            {
                if (identifier.Name.Equals(item.from))
                {
                    return item.temp;
                }
            }
            return identifier;
        }

        public static AstNode Replace(AstNode node, IFastEnumerable<(string from, AstIdentifier temp)> changes)
        {
            var ast = new AstIdentifierReplacer(changes);
            return ast.Visit(node);
        }
    }

    public abstract class AstReduce: AstMapVisitor<AstNode>
    {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool Modified<T>(T node, out T r)
            where T: AstNode
        {
            r = Visit(node) as T;
            return r != node;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool Modified<T1, T2>(T1 node1, T2 node2, out T1 r1, out T2 r2)
            where T1 : AstNode
            where T2 : AstNode
        {
            r1 = Visit(node1) as T1;
            r2 = Visit(node2) as T2;
            return r1 != node1 || r2 != node2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool Modified<T1, T2, T3>(T1 node1, T2 node2, T3 node3, out T1 r1, out T2 r2, out T3 r3)
            where T1 : AstNode
            where T2 : AstNode
            where T3 : AstNode
        {
            r1 = Visit(node1) as T1;
            r2 = Visit(node2) as T2;
            r3 = Visit(node3) as T3;
            return r1 != node1 || r2 != node2 || r3 != node3;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool Modified<T1, T2, T3, T4>(T1 node1, T2 node2, T3 node3, T4 node4, out T1 r1, out T2 r2, out T3 r3, out T4 r4)
            where T1 : AstNode
            where T2 : AstNode
            where T3 : AstNode
            where T4 : AstNode
        {
            r1 = Visit(node1) as T1;
            r2 = Visit(node2) as T2;
            r3 = Visit(node3) as T3;
            r4 = Visit(node4) as T4;
            return r1 != node1 || r2 != node2 || r3 != node3 || r4 != node4;
        }

        private bool Modified<T>(in ArraySpan<T> statements, out ArraySpan<T> list)
            where T: AstNode
        {
            list = statements;
            if(statements.Length == 0)
            {
                return false;
            }
            bool dirty = false;
            var r = new T[statements.Length];
            for (int i = 0; i < statements.Length; i++)
            {
                ref var item = ref statements[i];
                var visited = Visit(item);
                if (visited != item)
                    dirty = true;
                r[i] = visited as T;
            }
            if(!dirty)
            {
                return false;
            }
            list = new ArraySpan<T>(r, r.Length);
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool Modified<T>(IFastEnumerable<T> statements, out IFastEnumerable<T> list)
            where T : AstNode
        {
            list = statements;
            if (statements.Count == 0)
            {
                return false;
            }
            // we will create new sequence only if any expression has been modified
            // this will prevent allocations
            Sequence<T> r = null;
            var en = statements.GetFastEnumerator();
            while (en.MoveNext(out var item))
            {
                var visited = Visit(item) as T ?? throw new ArgumentNullException();
                if (visited == item)
                {
                    r?.Add(item);
                    continue;
                }
                if (r == null)
                {
                    r = new Sequence<T>(statements.Count);
                    var ec = statements.GetFastEnumerator();
                    while (ec.MoveNext(out var previous))
                    {
                        if (previous == item)
                            break;
                        r.Add(previous);
                    }
                    r.Add(visited);
                    continue;
                }
                r.Add(visited);
            }
            if (r == null)
            {
                return false;
            }
            list = r;
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool Modified<T>(IFastEnumerable<T> statements, Func<T, T> visitor, out IFastEnumerable<T> list)
        {
            list = statements;
            if (statements.Count == 0)
            {
                return false;
            }
            // we will create new sequence only if any expression has been modified
            // this will prevent allocations
            Sequence<T> r = null;
            var en = statements.GetFastEnumerator();
            while (en.MoveNext(out var item, out var index))
            {
                var visitedItem = visitor(item);
                if (visitedItem.Equals(item)) {
                    r?.Add(item);
                    continue;
                }
                if (r == null)
                {
                    r = new Sequence<T>(statements.Count);
                    var ec = statements.GetFastEnumerator();
                    while (ec.MoveNext(out var previous, out var i))
                    {
                        if (index == i)
                            break;
                        r.Add(previous);
                    }
                    r.Add(visitedItem);
                    continue;
                }
                r.Add(visitedItem);
            }
            if (r == null)
            {
                return false;
            }
            list = r;
            return true;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool Modified<T>(T[] statements, out T[] list)
            where T : AstExpression
        {
            list = statements;
            if (statements.Length == 0)
            {
                return false;
            }
            T[] r = null;
            for (int i = 0; i < statements.Length; i++)
            {
                ref var item = ref statements[i];
                var visited = Visit(item) as T ?? throw new ArgumentNullException();
                if (visited == item)
                {
                    if (r != null)
                        r[i] = item;
                    continue;
                }
                if (r == null)
                {
                    r = new T[statements.Length];
                    for (int j = 0; j < i; j++)
                    {
                        r[j] = statements[j];
                    }
                }
                r[i] = visited;
            }
            if (r == null)
            {
                return false;
            }
            list = r;
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool Modified<T>(T[] statements, Func<T, T> visitor, out T[] list)
        {
            list = statements;
            if (statements.Length == 0)
            {
                return false;
            }
            T[] r = null;
            for (int i = 0; i < statements.Length; i++)
            {
                ref var item = ref statements[i];
                var visited = visitor(item);
                if (visited.Equals(item))
                    continue;
                if (r == null)
                {
                    r = new T[statements.Length];
                    for (int j = 0; j < i; j++)
                    {
                        r[j] = statements[j];
                    }
                }
                r[i] = visited;
            }
            if (r == null)
            {
                return false;
            }
            list = r;
            return true;
        }

        private bool Modified<T>(in ArraySpan<T> statements, Func<T, T> visitor, out ArraySpan<T> list)
        {
            list = statements;
            if (statements.Length == 0)
            {
                return false;
            }
            bool dirty = false;
            var r = new T[statements.Length];
            for (int i = 0; i < statements.Length; i++)
            {
                ref var item = ref statements[i];
                var visited = visitor(item);
                if (!visited.Equals(item))
                    dirty = true;
                r[i] = visited;
            }
            if (!dirty)
            {
                return false;
            }
            list = new ArraySpan<T>(r, r.Length);
            return true;
        }

        protected override AstNode VisitArrayPattern(AstArrayPattern arrayPattern)
        {
            if (Modified(arrayPattern.Elements, out var elements))
                return new AstArrayPattern(arrayPattern.Start, arrayPattern.End, elements);
            return arrayPattern;
        }

        protected override AstNode VisitProgram(AstProgram program)
        {
            if (Modified(program.Statements, out var list))
                return new AstProgram(program.Start, program.End, list, program.IsAsync);
            return program;
        }

        protected override AstNode VisitBlock(AstBlock block)
        {
            if (Modified(block.Statements, out var list))
                return new AstBlock(block.Start, block.End, list);
            return block;
        }

        protected override AstNode VisitAwaitExpression(AstAwaitExpression node)
        {
            if (Modified(node.Argument, out var arg))
                return new AstAwaitExpression(node.Start, node.End, arg);
            return node;
        }

        protected override AstNode VisitArrayExpression(AstArrayExpression arrayExpression)
        {
            if (Modified(arrayExpression.Elements, out var list))
                return new AstArrayExpression(arrayExpression.Start, arrayExpression.End, list);
            return arrayExpression;
        }

        protected override AstNode VisitBreakStatement(AstBreakStatement breakStatement)
        {
            return breakStatement;
        }

        protected override AstNode VisitBinaryExpression(AstBinaryExpression binaryExpression)
        {
            
            if (Modified(binaryExpression.Left, binaryExpression.Right, out var left, out var right))
                return new AstBinaryExpression(left, binaryExpression.Operator, right);
            return binaryExpression;
        }

        protected override AstNode VisitCallExpression(AstCallExpression callExpression)
        {
            var t = Modified(callExpression.Callee, out var target);
            var a = Modified(callExpression.Arguments, out var arguments);
            if(t || a)
            {
                return new AstCallExpression(target, arguments, callExpression.Coalesce);
            }
            return callExpression;
        }

        protected override AstNode VisitClassStatement(AstClassExpression classStatement)
        {
            var pc = Modified(classStatement.Identifier, classStatement.Base, out var id, out var @base);
            if (Modified(classStatement.Members, out var members) || pc)
                return new AstClassExpression(classStatement.Start, classStatement.End, id, @base, members);
            return classStatement;
        }

        protected override AstNode VisitConditionalExpression(AstConditionalExpression conditionalExpression)
        {
            if (Modified(conditionalExpression.Test, conditionalExpression.True, conditionalExpression.False, out var test, out var @true, out var @false))
                return new AstConditionalExpression(test, @true, @false);
            return conditionalExpression;
        }

        protected override AstNode VisitExportStatement(AstExportStatement astExportStatement)
        {
            if (Modified(astExportStatement.Declaration, astExportStatement.Source, out var d, out var s))
                return new AstExportStatement(astExportStatement.Start, d, s);
            return astExportStatement;
        }

        protected override AstNode VisitImportStatement(AstImportStatement astImportStatement)
        {
            return astImportStatement;
        }

        protected override AstNode VisitContinueStatement(AstContinueStatement continueStatement)
        {
            return continueStatement;
        }

        protected override AstNode VisitDebuggerStatement(AstDebuggerStatement debuggerStatement)
        {
            return debuggerStatement;
        }

        protected override AstNode VisitDoWhileStatement(AstDoWhileStatement doWhileStatement, string label = null)
        {

            if (Modified(doWhileStatement.Body, doWhileStatement.Test, out var statement, out var test))
                return new AstDoWhileStatement(doWhileStatement.Start, doWhileStatement.End, test, statement);
            return doWhileStatement;
        }

        protected override AstNode VisitEmptyExpression(AstEmptyExpression emptyExpression)
        {
            return emptyExpression;
        }

        protected override AstNode VisitExpressionStatement(AstExpressionStatement expressionStatement)
        {
            if (Modified(expressionStatement.Expression, out var expression))
                return new AstExpressionStatement(expression);
            return expressionStatement;
        }

        protected override AstNode VisitForInStatement(AstForInStatement forInStatement, string label = null)
        {
            if(Modified(forInStatement.Init, forInStatement.Target, forInStatement.Body, 
                out var init, out var target, out var body))
            {
                return new AstForInStatement(forInStatement.Start, forInStatement.End, init, target, body);
            }
            return forInStatement;
        }

        protected override AstNode VisitForOfStatement(AstForOfStatement forOfStatement, string label = null)
        {
            if (Modified(forOfStatement.Init, forOfStatement.Target, forOfStatement.Body,
                out var init, out var target, out var body))
            {
                return new AstForInStatement(forOfStatement.Start, forOfStatement.End, init, target, body);
            }
            return forOfStatement;
        }

        protected override AstNode VisitForStatement(AstForStatement forStatement, string label = null)
        {
            if (Modified(forStatement.Init, forStatement.Test, forStatement.Update, forStatement.Body,
                out var init, out var test, out var update, out var body))
                return new AstForStatement(forStatement.Start, forStatement.End, init, test, update, body);
            return forStatement;
        }

        protected override AstNode VisitFunctionExpression(AstFunctionExpression functionExpression)
        {
            var argsModified = Modified(functionExpression.Params, VisitVariableDeclarator, out var parameters);
            if(Modified(functionExpression.Id, functionExpression.Body, out var id, out var body) || argsModified)
                return new AstFunctionExpression(functionExpression.Start, functionExpression.End, 
                    functionExpression.IsArrowFunction, 
                    functionExpression.Async, 
                    functionExpression.Generator, 
                    id, parameters, body);
            return functionExpression;
        }

        protected override AstNode VisitIdentifier(AstIdentifier identifier)
        {
            return identifier;
        }

        protected override AstNode VisitIfStatement(AstIfStatement ifStatement)
        {
            if (Modified(ifStatement.Test, ifStatement.True, ifStatement.False,
                out var test, out var @true, out var @false))
                return new AstIfStatement(ifStatement.Start, ifStatement.End, test, @true, @false);
            return ifStatement;
        }

        protected override AstNode VisitLabeledStatement(AstLabeledStatement labeledStatement)
        {
            if (Modified(labeledStatement.Body, out var statement))
                return new AstLabeledStatement(labeledStatement.Label, statement);
            return labeledStatement;
        }

        protected override AstNode VisitLiteral(AstLiteral literal)
        {
            return literal;
        }

        protected override AstNode VisitMeta(AstMeta astMeta)
        {
            return astMeta;
        }

        protected override AstNode VisitMemberExpression(AstMemberExpression memberExpression)
        {
            if (Modified(memberExpression.Object, memberExpression.Property, out var target, out var member))
                return new AstMemberExpression(target, member, memberExpression.Computed);
            return memberExpression;
        }

        protected override AstNode VisitNewExpression(AstNewExpression newExpression) {
            var cm = Modified(newExpression.Callee, out var callee);
            var am = Modified(newExpression.Arguments, out var args);
            if (cm || am)
                return new AstNewExpression(newExpression.Start, callee, args);
            return newExpression;
        }

        protected override AstNode VisitObjectLiteral(AstObjectLiteral objectLiteral)
        {
            if (Modified(objectLiteral.Properties, out var members))
                return new AstObjectLiteral(objectLiteral.Start, objectLiteral.End, members);
            return objectLiteral;
        }

        protected override AstNode VisitObjectPattern(AstObjectPattern objectPattern)
        {
            if(Modified(objectPattern.Properties, VisitObjectProperty, out var properties))
                return new AstObjectPattern(objectPattern.Start, objectPattern.End, properties);
            return objectPattern;
        }

        protected override AstNode VisitReturnStatement(AstReturnStatement returnStatement) {
            if (Modified(returnStatement.Argument, out var target))
                return new AstReturnStatement(returnStatement.Start, returnStatement.End, target);
            return returnStatement;
        }

        protected override AstNode VisitSequenceExpression(AstSequenceExpression sequenceExpression)
        {
            if (Modified(sequenceExpression.Expressions, out var expressions))
                return new AstSequenceExpression(sequenceExpression.Start, sequenceExpression.End, expressions);
            return sequenceExpression;
        }

        protected override AstNode VisitSpreadElement(AstSpreadElement spreadElement)
        {
            if (Modified(spreadElement.Argument, out var argument))
                return new AstSpreadElement(spreadElement.Start, spreadElement.End, argument);
            return argument;
        }

        protected override AstNode VisitSwitchStatement(AstSwitchStatement switchStatement)
        {
            var testModified = Modified(switchStatement.Target, out var target);
            if (Modified(switchStatement.Cases, VisitCase, out var cases) || testModified)
                return new AstSwitchStatement(switchStatement.Start, switchStatement.End, target, cases);
            return switchStatement;
        }

        protected override AstNode VisitTaggedTemplateExpression(AstTaggedTemplateExpression taggedTemplateExpression)
        {
            var tagModified = Modified(taggedTemplateExpression.Tag, out var tag);
            var argsModified = Modified(taggedTemplateExpression.Arguments, out var arguments);
            if (tagModified || argsModified)
                return new AstTaggedTemplateExpression(tag, arguments);
            return taggedTemplateExpression;
        }

        protected override AstNode VisitTemplateExpression(AstTemplateExpression templateExpression)
        {
            if (Modified(templateExpression.Parts, out var parts))
                return new AstTemplateExpression(templateExpression.Start, templateExpression.End, parts);
            return templateExpression;
        }

        protected override AstNode VisitThrowStatement(AstThrowStatement throwStatement)
        {
            if (Modified(throwStatement.Argument, out var argument))
                return new AstThrowStatement(throwStatement.Start, throwStatement.End, argument);
            return throwStatement;
        }

        protected override AstNode VisitTryStatement(AstTryStatement tryStatement)
        {
            if (Modified(tryStatement.Block, tryStatement.Catch, tryStatement.Finally,
                out var @try, out var @catch, out var @finally))
                return new AstTryStatement(tryStatement.Start, tryStatement.End, @try, tryStatement.Identifier, @catch, @finally);
            return tryStatement;
        }

        protected override AstNode VisitUnaryExpression(AstUnaryExpression unaryExpression)
        {
            if (Modified(unaryExpression.Argument, out var argument))
                return new AstUnaryExpression(unaryExpression.Start, argument, unaryExpression.Operator, unaryExpression.Prefix);
            return unaryExpression;
        }

        protected override AstNode VisitVariableDeclaration(AstVariableDeclaration variableDeclaration)
        {
            if (Modified(variableDeclaration.Declarators, VisitVariableDeclarator, out var declarators))
                return new AstVariableDeclaration(
                    variableDeclaration.Start,
                    variableDeclaration.End,
                    declarators,
                    variableDeclaration.Kind,
                    variableDeclaration.Using,
                    variableDeclaration.AwaitUsing);
            return variableDeclaration;
        }

        protected override AstNode VisitWhileStatement(AstWhileStatement whileStatement, string label = null)
        {
            if (Modified(whileStatement.Test, whileStatement.Body, out var test, out var body))
                return new AstWhileStatement(whileStatement.Start, whileStatement.End, test, body);
            return whileStatement;
        }

        protected override AstNode VisitYieldExpression(AstYieldExpression yieldExpression)
        {
            if (Modified(yieldExpression.Argument, out var argument))
                return new AstYieldExpression(yieldExpression.Start, yieldExpression.End, argument, yieldExpression.Delegate);
            return yieldExpression;
        }

        protected override AstNode VisitClassProperty(AstClassProperty property)
        {
            var km = Modified(property.Key, out var key);
            var im = Modified(property.Init, out var init);
            if (km || im)
                return property.Reduce(key, init);
            return property;
        }

#pragma warning disable EPS05 // Use in-modifier for a readonly struct

        protected virtual AstCase VisitCase(AstCase property)
        {
            return property;
        }

        protected virtual VariableDeclarator VisitVariableDeclarator(VariableDeclarator property)
        {
            return property;
        }
        protected virtual ObjectProperty VisitObjectProperty(ObjectProperty property)
        {
            return property;
        }

#pragma warning restore EPS05 // Use in-modifier for a readonly struct

    }

}
