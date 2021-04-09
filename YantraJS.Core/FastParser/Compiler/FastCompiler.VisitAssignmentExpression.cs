using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using YantraJS.Core.LinqExpressions;
using YantraJS.ExpHelper;
using YantraJS.Utils;

namespace YantraJS.Core.FastParser.Compiler
{
    partial class FastCompiler
    {
        private Expression VisitAssignmentExpression(
            AstExpression left, 
            Esprima.Ast.AssignmentOperator assignmentOperator, 
            AstExpression right)
        {
            switch (left.Type)
            {
                case FastNodeType.ArrayPattern:
                case FastNodeType.ObjectPattern:
                    return CreateAssignment(left, Visit(right));
            }
            return BinaryOperation.Assign(Visit(left), Visit(right), assignmentOperator);
        }

        private Expression CreateAssignment(AstExpression left, Expression expression)
        {
            throw new NotImplementedException();
        }
    }
}
