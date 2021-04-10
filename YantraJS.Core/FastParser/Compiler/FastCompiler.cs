using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using YantraJS.Core.LinqExpressions;
using YantraJS.ExpHelper;
using YantraJS.Utils;
using Exp = System.Linq.Expressions.Expression;

namespace YantraJS.Core.FastParser.Compiler
{
    public partial class FastCompiler : AstMapVisitor<Expression>
    {

        private readonly FastPool pool;

        readonly LinkedStack<FastFunctionScope> scope = new LinkedStack<FastFunctionScope>();

        public LoopScope LoopScope => this.scope.Top.Loop.Top;

        private StringArray _keyStrings = new StringArray();

        private SparseList<object> _innerFunctions = new SparseList<object>();

        public Expression<JSFunctionDelegate> Method { get; }

        public FastCompiler(FastPool pool)
        {
            this.pool = pool;
        }

        private Expression VisitExpression(AstExpression exp) => Visit(exp);

        private Expression VisitStatement(AstStatement exp) => Visit(exp);

        protected override Expression VisitBlock(AstBlock block)
        {
            return VisitStatements(block.HoistingScope, in block.Statements);
        }

        protected override Expression VisitClassStatement(AstClassExpression classStatement)
        {
            return CreateClass(classStatement.Identifier, classStatement.Base, classStatement);
        }

        protected override Expression VisitContinueStatement(AstContinueStatement continueStatement)
        {
            string name = continueStatement.Label?.Name.Value;
            if (name != null)
            {
                var target = this.LoopScope.Get(name);
                if (target == null)
                    throw JSContext.Current.NewSyntaxError($"No label found for {name}");
                return Exp.Continue(target.Break);
            }
            return Exp.Continue(this.scope.Top.Loop.Top.Continue);
        }

        protected override Expression VisitDebuggerStatement(AstDebuggerStatement debuggerStatement)
        {
            return ExpHelper.JSDebuggerBuilder.RaiseBreak();
        }



        protected override Expression VisitEmptyExpression(AstEmptyExpression emptyExpression)
        {
            return Exp.Empty();
        }

        protected override Expression VisitExpressionStatement(AstExpressionStatement expressionStatement)
        {
            return Visit(expressionStatement.Expression);
        }

        protected override Expression VisitFunctionExpression(AstFunctionExpression functionExpression)
        {
            return CreateFunction(functionExpression);
        }




        protected override Expression VisitProgram(AstProgram program)
        {
            return VisitStatements(program.HoistingScope, in program.Statements);
        }



        protected override Expression VisitSpreadElement(AstSpreadElement spreadElement)
        {
            throw new NotImplementedException();
        }

        protected override Expression VisitSwitchStatement(AstSwitchStatement switchStatement)
        {
            throw new NotImplementedException();
        }

        protected override Expression VisitThrowStatement(AstThrowStatement throwStatement)
        {
            return ExpHelper.JSExceptionBuilder.Throw(VisitExpression(throwStatement.Argument));
        }

        protected override Expression VisitVariableDeclaration(AstVariableDeclaration variableDeclaration)
        {
            throw new NotImplementedException();
        }


        protected override Expression VisitYieldExpression(AstYieldExpression yieldExpression)
        {
            var target = VisitExpression(yieldExpression.Argument);
            if (yieldExpression.Delegate)
            {
                throw new NotSupportedException();
                // return JSGeneratorBuilder.Delegate(this.scope.Top.Generator, VisitExpression(yieldExpression.Argument));
            }
            // return JSGeneratorBuilder.Yield(this.scope.Top.Generator, VisitExpression(yieldExpression.Argument));
            return YantraJS.Core.LinqExpressions.Generators.YieldExpression.New(target);

        }
    }
}
