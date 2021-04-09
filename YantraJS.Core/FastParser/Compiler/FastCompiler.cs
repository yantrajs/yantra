using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using YantraJS.Core.LinqExpressions;
using YantraJS.ExpHelper;
using YantraJS.Utils;

namespace YantraJS.Core.FastParser.Compiler
{
    public partial class FastCompiler : AstMapVisitor<Expression>
    {

        private FastPool pool;

        readonly LinkedStack<FastFunctionScope> scope = new LinkedStack<FastFunctionScope>();

        public LoopScope LoopScope => this.scope.Top.Loop.Top;

        private StringArray _keyStrings = new StringArray();

        private SparseList<object> _innerFunctions = new SparseList<object>();

        public Expression<JSFunctionDelegate> Method { get; }

        public FastCompiler()
        {

        }





        protected override Expression VisitBlock(AstBlock block)
        {
            return VisitStatements(block.HoistingScope, in block.Statements);
        }

        protected override Expression VisitCallExpression(AstCallExpression callExpression)
        {
            throw new NotImplementedException();
        }

        protected override Expression VisitClassStatement(AstClassExpression classStatement)
        {
            throw new NotImplementedException();
        }

        protected override Expression VisitConditionalExpression(AstConditionalExpression conditionalExpression)
        {
            throw new NotImplementedException();
        }

        protected override Expression VisitContinueStatement(AstContinueStatement continueStatement)
        {
            throw new NotImplementedException();
        }

        protected override Expression VisitDebuggerStatement(AstDebuggerStatement debuggerStatement)
        {
            throw new NotImplementedException();
        }

        protected override Expression VisitDoWhileStatement(AstDoWhileStatement doWhileStatement)
        {
            throw new NotImplementedException();
        }

        protected override Expression VisitEmptyExpression(AstEmptyExpression emptyExpression)
        {
            throw new NotImplementedException();
        }

        protected override Expression VisitExpressionStatement(AstExpressionStatement expressionStatement)
        {
            throw new NotImplementedException();
        }

        protected override Expression VisitForInStatement(AstForInStatement forInStatement)
        {
            throw new NotImplementedException();
        }

        protected override Expression VisitForOfStatement(AstForOfStatement forOfStatement)
        {
            throw new NotImplementedException();
        }

        protected override Expression VisitForStatement(AstForStatement forStatement)
        {
            throw new NotImplementedException();
        }

        protected override Expression VisitFunctionExpression(AstFunctionExpression functionExpression)
        {
            throw new NotImplementedException();
        }

        protected override Expression VisitIdentifier(AstIdentifier identifier)
        {
            throw new NotImplementedException();
        }

        protected override Expression VisitIfStatement(AstIfStatement ifStatement)
        {
            throw new NotImplementedException();
        }

        protected override Expression VisitLabeledStatement(AstLabeledStatement labeledStatement)
        {
            throw new NotImplementedException();
        }

        protected override Expression VisitLiteral(AstLiteral literal)
        {
            throw new NotImplementedException();
        }

        protected override Expression VisitMemberExpression(AstMemberExpression memberExpression)
        {
            throw new NotImplementedException();
        }

        protected override Expression VisitObjectLiteral(AstObjectLiteral objectLiteral)
        {
            throw new NotImplementedException();
        }

        protected override Expression VisitObjectPattern(AstObjectPattern objectPattern)
        {
            throw new NotImplementedException();
        }

        protected override Expression VisitProgram(AstProgram program)
        {
            return VisitStatements(program.HoistingScope, in program.Statements);
        }

        protected override Expression VisitSequenceExpression(AstSequenceExpression sequenceExpression)
        {
            throw new NotImplementedException();
        }

        protected override Expression VisitSpreadElement(AstSpreadElement spreadElement)
        {
            throw new NotImplementedException();
        }

        protected override Expression VisitSwitchStatement(AstSwitchStatement switchStatement)
        {
            throw new NotImplementedException();
        }

        protected override Expression VisitTemplateExpression(AstTemplateExpression templateExpression)
        {
            throw new NotImplementedException();
        }

        protected override Expression VisitThrowStatement(AstThrowStatement throwStatement)
        {
            throw new NotImplementedException();
        }

        protected override Expression VisitTryStatement(AstTryStatement tryStatement)
        {
            throw new NotImplementedException();
        }

        protected override Expression VisitUnaryExpression(AstUnaryExpression unaryExpression)
        {
            throw new NotImplementedException();
        }

        protected override Expression VisitVariableDeclaration(AstVariableDeclaration variableDeclaration)
        {
            throw new NotImplementedException();
        }

        protected override Expression VisitWhileStatement(AstWhileStatement whileStatement)
        {
            throw new NotImplementedException();
        }

        protected override Expression VisitYieldExpression(AstYieldExpression yieldExpression)
        {
            throw new NotImplementedException();
        }
    }
}
