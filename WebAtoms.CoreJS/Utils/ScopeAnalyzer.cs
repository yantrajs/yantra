using Esprima.Ast;
using Esprima.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using WebAtoms.CoreJS.Core;

namespace WebAtoms.CoreJS.Utils
{

    public class ScopeAnaylzerNode : LinkedStackItem<ScopeAnaylzerNode>
    {
        public ScopeAnaylzerNode(Node node)
        {
            Node = node;
        }

        public Node Node { get;}
    }

    public class ScopeAnalyzer: AstVisitor
    {

        public ScopeAnalyzer()
        {

        }

        private LinkedStack<ScopeAnaylzerNode> stack = new LinkedStack<ScopeAnaylzerNode>();

        protected override void VisitFunctionDeclaration(FunctionDeclaration functionDeclaration)
        {
            base.VisitFunctionDeclaration(functionDeclaration);
        }

        protected override void VisitFunctionExpression(IFunction function)
        {
            base.VisitFunctionExpression(function);
        }

        protected override void VisitArrowFunctionExpression(ArrowFunctionExpression arrowFunctionExpression)
        {
            base.VisitArrowFunctionExpression(arrowFunctionExpression);
        }

        protected override void VisitProgram(Program program)
        {
            base.VisitProgram(program);
        }

    }
}
