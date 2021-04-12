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

        private Exp CreateAssignment(
            AstExpression pattern,
            Exp init,
            bool createVariable = false,
            bool newScope = false)
        {
            Exp target;
            SparseList<Exp> inits;
            switch ((pattern.Type,pattern))
            {
                case (FastNodeType.Identifier, AstIdentifier id):
                    inits = new SparseList<Exp>();
                    if (createVariable)
                    {
                        var v = this.scope.Top.CreateVariable(id.Name.Value, JSVariableBuilder.New(id.Name.Value), newScope);
                        // inits.Add(Exp.Assign(v.Variable, JSVariableBuilder.New(id.Name.Value)));
                        target = v.Expression;
                    }
                    else
                    {
                        target = this.VisitIdentifier(id);
                    }
                    inits.Add(Exp.Assign(target, init));
                    return Exp.Block(inits);
                case (FastNodeType.ObjectPattern, AstObjectPattern objectPattern):
                    inits = new SparseList<Exp>();
                    foreach (var property in objectPattern.Properties)
                    {
                        Exp start = null;
                        switch ((property.Key.Type, property.Key))
                        {
                            case (FastNodeType.Identifier, AstIdentifier id):
                                start = CreateMemberExpression(init, id, property.Computed);
                                break;
                            default:
                                throw new NotImplementedException();
                        }
                        switch (property.Value.Type)
                        {
                            case FastNodeType.Identifier:
                            case FastNodeType.ArrayPattern:
                            case FastNodeType.ObjectPattern:
                                inits.Add(CreateAssignment(property.Value, start, true, newScope));
                                break;
                            // TODO
                            case FastNodeType.BinaryExpression:
                                var ap = property.Value as AstBinaryExpression;
                                inits.Add(CreateAssignment(ap.Left,
                                    Exp.Coalesce(
                                        JSValueExtensionsBuilder.NullIfUndefined(start),
                                        Visit(ap.Right))
                                ));
                                break;
                            default:
                                throw new NotImplementedException();
                        }
                    }
                    return Exp.Block(inits);
                case (FastNodeType.ArrayPattern, AstArrayPattern arrayPattern):
                    inits = new SparseList<Exp>();
                    using (var enVar = this.scope.Top.GetTempVariable(typeof(IElementEnumerator)))
                    {
                        var en = enVar.Expression;
                        using (var item = this.scope.Top.GetTempVariable())
                        {
                            inits.Add(Exp.Assign(en, IElementEnumeratorBuilder.Get(init)));
                            foreach (var element in arrayPattern.Elements)
                            {
                                switch (element.Type)
                                {
                                    case FastNodeType.Identifier:
                                        var id = element as AstIdentifier;
                                        // inits.Add(CreateAssignment(id, start));
                                        if (createVariable)
                                        {
                                            var v = this.scope.Top.CreateVariable(id.Name.Value, null, newScope);
                                            inits.Add(Exp.Assign(v.Variable, JSVariableBuilder.New(id.Name.Value)));
                                        }
                                        var assignee = VisitIdentifier(id);
                                        inits.Add(IElementEnumeratorBuilder.AssignMoveNext(assignee, en,
                                            item.Expression));
                                        break;
                                    case FastNodeType.SpreadElement:
                                        var spe = element as AstSpreadElement;
                                        // loop...
                                        if (createVariable && spe.Argument is AstIdentifier id2)
                                        {
                                            var v = this.scope.Top.CreateVariable(id2.Name.Value, null, newScope);
                                            inits.Add(Exp.Assign(v.Variable, JSVariableBuilder.New(id2.Name.Value)));
                                        }

                                        var spid = Visit(spe.Argument);

                                        using (var arrayVar = this.scope.Top.GetTempVariable(typeof(JSArray)))
                                        {
                                            inits.Add(Exp.Assign(arrayVar.Expression, JSArrayBuilder.New()));
                                            var @break = Exp.Label();
                                            var add = JSArrayBuilder.Add(arrayVar.Expression, item.Expression);
                                            var @breakStmt = Exp.Goto(@break);
                                            var loop = Exp.Loop(Exp.Block(
                                                Exp.IfThenElse(
                                                    IElementEnumeratorBuilder.MoveNext(en, item.Expression),
                                                    add,
                                                    breakStmt)
                                                ), @break);
                                            inits.Add(loop);
                                            inits.Add(Exp.Assign(spid, arrayVar.Expression));
                                        }
                                        break;
                                    case FastNodeType.ObjectPattern:
                                    case FastNodeType.ArrayPattern:
                                        var ape = element;
                                        // nested array ...
                                        // nested object ...
                                        var check = IElementEnumeratorBuilder.MoveNext(en, item.Expression);
                                        inits.Add(check);
                                        inits.Add(CreateAssignment(ape, item.Expression, true, newScope));
                                        break;
                                    default:
                                        inits.Add(IElementEnumeratorBuilder.MoveNext(en, item.Expression));
                                        break;
                                }
                            }
                        }
                    }
                    return Exp.Block(inits);
            }
            throw new NotImplementedException();
        }
    }
}
