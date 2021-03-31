using System;
using System.Collections.Generic;
using System.Text;
using YantraJS.Utils;

namespace YantraJS.Core.FastParser.Ast
{

    public abstract class AstReduce: AstMapVisitor<AstNode>
    {

        private T VisitNode<T>(T node)
            where T: AstNode
        {
            return Visit(node) as T;
        }

        private bool Modified<T>(T node, out T r)
            where T: AstNode
        {
            r = Visit(node) as T;
            return r != node;
        }

        private bool Modified<T1, T2>(T1 node1, T2 node2, out T1 r1, out T2 r2)
            where T1 : AstNode
            where T2 : AstNode
        {
            r1 = Visit(node1) as T1;
            r2 = Visit(node2) as T2;
            return r1 != node1 || r2 != node2;
        }

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

        private bool Modified<T>(in ArraySpan<T> statements, out ArraySpan<T> list)
            where T: AstNode
        {
            if(statements.Length == 0)
            {
                list = ArraySpan<T>.Empty;
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
                list = ArraySpan<T>.Empty;
                return false;
            }
            list = new ArraySpan<T>(r, r.Length);
            return true;
        }

        private bool Modified<T>(in ArraySpan<T> statements, Func<T, T> visitor, out ArraySpan<T> list)
        {
            if (statements.Length == 0)
            {
                list = ArraySpan<T>.Empty;
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
                list = ArraySpan<T>.Empty;
                return false;
            }
            list = new ArraySpan<T>(r, r.Length);
            return true;
        }

        protected override AstNode VisitProgram(AstProgram program)
        {
            if (Modified(program.Statements, out var list))
                return new AstProgram(program.Start, program.End, list);
            return program;
        }

        protected override AstNode VisitBlock(AstBlock block)
        {
            if (Modified(block.Statements, out var list))
                return new AstBlock(block.Start, block.End, list);
            return block;
        }

        protected override AstNode VisitArrayExpression(AstArrayExpression arrayExpression)
        {
            if (Modified(arrayExpression.Elements, out var list))
                return new AstArrayExpression(arrayExpression.Start, arrayExpression.End, list);
            return arrayExpression;
        }

        protected override AstNode VisitBinaryExpression(AstBinaryExpression binaryExpression)
        {
            
            if (Modified(binaryExpression.Left, binaryExpression.Right, out var left, out var right))
                return new AstBinaryExpression(left, binaryExpression.Operator, right);
            return binaryExpression;
        }

        protected override AstNode VisitCallExpression(AstCallExpression callExpression)
        {
            var t = Modified(callExpression.Target, out var target);
            var a = Modified(callExpression.Arguments, out var arguments);
            if(t || a)
            {
                return new AstCallExpression(target, arguments);
            }
            return callExpression;
        }

        protected override AstNode VisitClassStatement(AstClassExpression classStatement)
        {
            var pc = Modified(classStatement.Identifier, classStatement.Base, out var id, out var @base);
            if (Modified(classStatement.Members, VisitClassProperty, out var members) || pc)
                return new AstClassExpression(classStatement.Start, classStatement.End, id, @base, members);
            return classStatement;
        }

        protected override AstNode VisitConditionalExpression(AstConditionalExpression conditionalExpression)
        {
            if (Modified(conditionalExpression.Test, conditionalExpression.True, conditionalExpression.False, out var test, out var @true, out var @false))
                return new AstConditionalExpression(test, @true, @false);
            return conditionalExpression;
        }

        protected override AstNode VisitContinueStatement(AstContinueStatement continueStatement)
        {
            return continueStatement;
        }

        protected override AstNode VisitDebuggerStatement(AstDebuggerStatement debuggerStatement)
        {
            return debuggerStatement;
        }

        protected override AstNode VisitDoWhileStatement(AstDoWhileStatement doWhileStatement)
        {

            if (Modified(doWhileStatement.Statement, doWhileStatement.Test, out var statement, out var test))
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

        

#pragma warning disable EPS05 // Use in-modifier for a readonly struct
        protected virtual AstClassProperty VisitClassProperty(AstClassProperty property)
#pragma warning restore EPS05 // Use in-modifier for a readonly struct
        {
            return property;
        }


    }

}
