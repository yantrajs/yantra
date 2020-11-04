using Esprima.Ast;
using Esprima.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using YantraJS.Core;

namespace YantraJS.Utils
{

    public class ScopeAnaylzerNode : LinkedStackItem<ScopeAnaylzerNode>
    {

        private Dictionary<string, (string name, VariableDeclarationKind kind)> Variables
            = new Dictionary<string, (string name, VariableDeclarationKind kind)>();

        public ScopeAnaylzerNode(Node node)
        {
            Node = node;
        }

        public Node Node { get;}
        public void CreateVariable(Expression d, VariableDeclarationKind kind)
        {
            switch (d)
            {
                case Identifier id:
                    AddVariable(id.Name, kind);
                    break;
                case SpreadElement spe:
                    CreateVariable(spe.Argument, kind);
                    break;
                case ArrayPattern ap:
                    foreach(var e in ap.Elements)
                    {
                        CreateVariable(e, kind);
                    }
                    break;
                case ObjectPattern op:
                    foreach(Property p in op.Properties.OfType<Property>())
                    {
                        CreateVariable(p.Value, kind);
                    }
                    break;
            }
        }
        public void AddVariable(string name, VariableDeclarationKind kind = VariableDeclarationKind.Var)
        {
            if (string.IsNullOrWhiteSpace(name))
                return;

            var n = this;
            while(true)
            {
                if(n.Variables.TryGetValue(name, out var pn)) {
                    if (pn.kind != VariableDeclarationKind.Var)
                    {
                        throw new AccessViolationException($"{name} is already defined in current scope");
                    }
                }
                n = n.Parent;
                if (n == null)
                    break;
                if (n.Node is BlockStatement)
                    continue;
                break;
            }

            Variables[name] = (name, VariableDeclarationKind.Var);

        }

        public override void Dispose()
        {
            // hoist here..
            var stmt = this.Node as Statement;
            if (stmt == null)
            {
                var fx = (this.Node as IFunction);
                stmt = fx.Body as Statement;
            }
            List<string> list = new List<string>();
            foreach (var node in Variables)
            {
                if(node.Value.kind == VariableDeclarationKind.Var)
                {
                    list.Add(node.Value.name);
                }
            }
            if (list.Count > 0)
            {
                stmt.HoistingScope = list;
            }
            base.Dispose();
        }
        public void VerifyAssignment(Expression left)
        {
            switch (left)
            {
                case Identifier id:
                    Assign(id.Name);
                    break;
            }
        }

        public void Assign(string name)
        {
            var d = GetVariable(name);
            if (d == VariableDeclarationKind.Const)
            {
                throw new AccessViolationException($"constant {name} cannot be assigned");
            }
        }

        public VariableDeclarationKind? GetVariable(string name)
        {
            if (this.Variables.TryGetValue(name, out var node)){
                return node.kind;
            }
            return this.Parent?.GetVariable(name);
        }
    }

    public class ScopeAnalyzer: AstVisitor
    {

        public ScopeAnalyzer()
        {
            
        }

        public void Process(Program node)
        {
            using (stack.Push(new ScopeAnaylzerNode(node)))
            {
                this.Visit(node);
            }
        }

        private LinkedStack<ScopeAnaylzerNode> stack = new LinkedStack<ScopeAnaylzerNode>();

        protected override void VisitVariableDeclaration(VariableDeclaration variableDeclaration)
        {
            foreach(var d in variableDeclaration.Declarations)
            {
                stack.Top.CreateVariable(d.Id, variableDeclaration.Kind);
            }
        }

        protected override void VisitAssignmentExpression(AssignmentExpression assignmentExpression)
        {
            stack.Top.VerifyAssignment(assignmentExpression.Left);
            base.VisitAssignmentExpression(assignmentExpression);
        }

        protected override void VisitAssignmentPattern(AssignmentPattern assignmentPattern)
        {
            stack.Top.VerifyAssignment(assignmentPattern.Left);
            base.VisitAssignmentPattern(assignmentPattern);
        }

        protected override void VisitBlockStatement(BlockStatement blockStatement)
        {
            using (stack.Push(new ScopeAnaylzerNode(blockStatement)))
            {
                base.VisitBlockStatement(blockStatement);
            }
        }

        protected override void VisitForStatement(ForStatement forStatement)
        {
            if (forStatement.Init is VariableDeclaration vd)
                VisitVariableDeclaration(vd);
            base.VisitForStatement(forStatement);
        }

        protected override void VisitFunctionDeclaration(FunctionDeclaration functionDeclaration)
        {
            stack.Top.AddVariable(functionDeclaration.Id?.Name);
            using (stack.Push(new ScopeAnaylzerNode(functionDeclaration)))
            {
                base.VisitFunctionDeclaration(functionDeclaration);
            }
        }

        protected override void VisitFunctionExpression(IFunction function)
        {
            stack.Top.AddVariable(function.Id?.Name);
            using (stack.Push(new ScopeAnaylzerNode(function as Node)))
            {
                base.VisitFunctionExpression(function);
            }
        }

        protected override void VisitArrowFunctionExpression(ArrowFunctionExpression arrowFunctionExpression)
        {
            stack.Top.AddVariable(arrowFunctionExpression.Id?.Name);
            using (stack.Push(new ScopeAnaylzerNode(arrowFunctionExpression)))
            {
                base.VisitArrowFunctionExpression(arrowFunctionExpression);
            }
        }

        protected override void VisitExportAllDeclaration(ExportAllDeclaration exportAllDeclaration)
        {
            // foreach(var e in exportAllDeclaration.)
        }

        protected override void VisitExportDefaultDeclaration(ExportDefaultDeclaration exportDefaultDeclaration)
        {
            this.Visit(exportDefaultDeclaration.Declaration);
        }

        protected override void VisitExportNamedDeclaration(ExportNamedDeclaration exportNamedDeclaration)
        {
            this.Visit(exportNamedDeclaration.Declaration);
        }

        protected override void VisitExportSpecifier(ExportSpecifier exportSpecifier)
        {
            base.VisitExportSpecifier(exportSpecifier);
        }

        protected override void VisitProgram(Program program)
        {
            using (stack.Push(new ScopeAnaylzerNode(program)))
            {
                base.VisitProgram(program);
            }
        }

        protected override void VisitImportDeclaration(ImportDeclaration importDeclaration)
        {
            foreach(var d in importDeclaration.Specifiers)
            {

                VisitStatement(d);
            }
            base.VisitImportDeclaration(importDeclaration);
        }

        protected override void VisitImportDefaultSpecifier(ImportDefaultSpecifier importDefaultSpecifier)
        {
            stack.Top.AddVariable(importDefaultSpecifier.Local.Name);
        }

        protected override void VisitImportSpecifier(ImportSpecifier importSpecifier)
        {
            stack.Top.AddVariable(importSpecifier.Imported.Name);
        }

        protected override void VisitImportNamespaceSpecifier(ImportNamespaceSpecifier importNamespaceSpecifier)
        {
            stack.Top.AddVariable(importNamespaceSpecifier.Local.Name);
        }

        protected override void VisitStatement(Statement statement)
        {
            switch(statement.Type)
            {
                case Nodes.ClassDeclaration:
                    VisitClassDeclaration(statement.As<ClassDeclaration>());
                    return;
                case Nodes.ExportSpecifier:
                    VisitExportSpecifier(statement.As<ExportSpecifier>());
                    return;
                case Nodes.ExportNamedDeclaration:
                    VisitExportNamedDeclaration(statement.As<ExportNamedDeclaration>());
                    return;
                case Nodes.ExportAllDeclaration:
                    VisitExportAllDeclaration(statement.As<ExportAllDeclaration>());
                    return;
                case Nodes.ExportDefaultDeclaration:
                    VisitExportDefaultDeclaration(statement.As<ExportDefaultDeclaration>());
                    return;
                case Nodes.ImportDeclaration:
                    VisitImportDeclaration(statement.As<ImportDeclaration>());
                    return;
                case Nodes.ImportDefaultSpecifier:
                    VisitImportDefaultSpecifier(statement.As<ImportDefaultSpecifier>());
                    return;
                case Nodes.ImportSpecifier:
                    VisitImportSpecifier(statement.As<ImportSpecifier>());
                    return;
                case Nodes.ImportNamespaceSpecifier:
                    VisitImportNamespaceSpecifier(statement.As<ImportNamespaceSpecifier>());
                    return;

            }
            base.VisitStatement(statement);
        }

        protected override void VisitExpression(Expression node)
        {
            if (node == null)
                return;
            switch (node.Type)
            {
                case Nodes.YieldExpression:
                    VisitYieldExpression(node.As<YieldExpression>());
                    return;
                case Nodes.AwaitExpression:
                    VisitAwaitExpression(node.As<AwaitExpression>());
                    return;
                case Nodes.ExportSpecifier:
                    VisitExportSpecifier(node.As<ExportSpecifier>());
                    return;
                case Nodes.ExportNamedDeclaration:
                    VisitExportNamedDeclaration(node.As<ExportNamedDeclaration>());
                    return;
                case Nodes.ExportAllDeclaration:
                    VisitExportAllDeclaration(node.As<ExportAllDeclaration>());
                    return;
                case Nodes.ExportDefaultDeclaration:
                    VisitExportDefaultDeclaration(node.As<ExportDefaultDeclaration>());
                    return;
                case Nodes.TemplateLiteral:
                    VisitTemplateLiteral(node.As<TemplateLiteral>());
                    return;
                case Nodes.Import:
                    VisitImport(node.As<Import>());
                    return;
                case Nodes.ImportDeclaration:
                    VisitImportDeclaration(node.As<ImportDeclaration>());
                    return;
                case Nodes.ImportDefaultSpecifier:
                    VisitImportDefaultSpecifier(node.As<ImportDefaultSpecifier>());
                    return;
                case Nodes.ImportNamespaceSpecifier:
                    VisitImportNamespaceSpecifier(node.As<ImportNamespaceSpecifier>());
                    return;
            }
            base.VisitExpression(node);
        }

    }
}
