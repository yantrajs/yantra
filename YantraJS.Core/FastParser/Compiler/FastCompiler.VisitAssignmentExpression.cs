using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using YantraJS.Core.LinqExpressions;
using YantraJS.ExpHelper;
using YantraJS.Utils;
using Exp = YantraJS.Expressions.YExpression;
using Expression = YantraJS.Expressions.YExpression;
using ParameterExpression = YantraJS.Expressions.YParameterExpression;

namespace YantraJS.Core.FastParser.Compiler
{
    partial class FastCompiler
    {
        private Expression VisitAssignmentExpression(
            AstExpression left, 
            TokenTypes assignmentOperator, 
            AstExpression right)
        {
            switch (left.Type)
            {
                case FastNodeType.ArrayPattern:
                case FastNodeType.ObjectPattern:
                    return CreateAssignment(left, Visit(right));
            }


            // we need to rewrite left side if it is computed expression with member assignment...
            if (assignmentOperator != TokenTypes.Assign 
                && left.Type == FastNodeType.MemberExpression 
                && left is AstMemberExpression mem)
            {
                if (mem.Object.Type != FastNodeType.Identifier)
                {
                    // this needs to be computed...
                    var tmp = this.scope.Top.GetTempVariable();
                    var leftExp = CreateMemberExpression(tmp.Expression, mem.Property, mem.Computed);
                    return Expression.Block(
                        Expression.Assign(tmp.Expression, Visit(mem.Object)),
                        BinaryOperation.Assign(leftExp, Visit(right), assignmentOperator),
                        tmp.Expression
                        );

                }
            }
            return BinaryOperation.Assign(Visit(left), Visit(right), assignmentOperator);
        }

        private Exp CreateAssignment(
            AstExpression pattern,
            Exp init,
            bool createVariable = false,
            bool newScope = false)
        {
            var inits = pool.AllocateList<Exp>();
            try {
                CreateAssignment(inits, pattern, init, createVariable, newScope);
                return Exp.Block(inits);
            } finally
            {
                inits.Clear();
            }
        }

        private void CreateAssignment(
            FastList<Exp> inits,
            AstExpression pattern,
            Exp init,
            bool createVariable = false,
            bool newScope = false)
        {
            Exp target;
            switch (pattern.Type)
            {
                case FastNodeType.Identifier:
                    {
                        var id = pattern as AstIdentifier;
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
                    }
                    return;
                case FastNodeType.ObjectPattern:
                    var objectPattern = pattern as AstObjectPattern;
                    foreach (var property in objectPattern.Properties)
                    {
                        Exp start = null;
                        switch (property.Key.Type)
                        {
                            case FastNodeType.Identifier:
                                var id = property.Key as AstIdentifier;
                                var propertyInit = property.Init;
                                if (propertyInit != null)
                                {
                                    var piTemp = scope.Top.GetTempVariable(typeof(JSValue));
                                    inits.Add(Exp.Assign(piTemp.Variable, 
                                        JSValueBuilder.Coalesce(
                                        CreateMemberExpression(init, id, property.Computed),
                                        Visit(propertyInit))));
                                    start = piTemp.Variable;
                                }
                                else
                                {
                                    start = CreateMemberExpression(init, id, property.Computed);
                                }
                                break;
                            default:
                                throw new NotImplementedException();
                        }
                        switch (property.Value.Type)
                        {
                            case FastNodeType.Identifier:
                            case FastNodeType.ArrayPattern:
                            case FastNodeType.ObjectPattern:
                                CreateAssignment(inits, property.Value, start, true, newScope);
                                break;
                            // TODO
                            case FastNodeType.BinaryExpression:
                                var ap = property.Value as AstBinaryExpression;
                                CreateAssignment(inits, ap.Left,
                                    Exp.Coalesce(
                                        JSValueExtensionsBuilder.NullIfUndefined(start),
                                        Visit(ap.Right))
                                );
                                break;
                            default:
                                throw new NotImplementedException();
                        }
                    }
                    return;
                case FastNodeType.ArrayPattern:
                    var arrayPattern = pattern as AstArrayPattern;
                    using (var enVar = this.scope.Top.GetTempVariable(typeof(IElementEnumerator)))
                    {
                        var en = enVar.Expression;
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
                                        this.scope.Top.CreateVariable(id.Name.Value, null, newScope);
                                        // inits.Add(Exp.Assign(v.Variable, JSVariableBuilder.New(id.Name.Value)));
                                    }
                                    var assignee = VisitIdentifier(id);
                                    inits.Add(IElementEnumeratorBuilder.AssignMoveNext(assignee, en));
                                    break;
                                case FastNodeType.BinaryExpression:
                                    var be = element as AstBinaryExpression;
                                    if (be.Left.Type != FastNodeType.Identifier)
                                        throw new FastParseException(be.Left.Start, "Invalid left hand side in assignment");
                                    id = be.Left as AstIdentifier;
                                    if (createVariable)
                                    {
                                        this.scope.Top.CreateVariable(id.Name.Value, null, newScope);
                                    }
                                    assignee = VisitIdentifier(id);
                                    inits.Add(IElementEnumeratorBuilder.AssignMoveNext(assignee, en));
                                    inits.Add(JSValueExtensionsBuilder.AssignCoalesce(assignee, Visit(be.Right)));
                                    break;
                                case FastNodeType.SpreadElement:
                                    var spe = element as AstSpreadElement;
                                    // loop...
                                    if (createVariable && spe.Argument is AstIdentifier id2)
                                    {
                                        this.scope.Top.CreateVariable(id2.Name.Value, null, newScope);
                                        // inits.Add(Exp.Assign(v.Variable, JSVariableBuilder.New(id2.Name.Value)));
                                    }

                                    var spid = Visit(spe.Argument);

                                    // using (var arrayVar = this.scope.Top.GetTempVariable(typeof(JSArray)))
                                    // {
                                        //inits.Add(Exp.Assign(arrayVar.Expression, JSArrayBuilder.New()));
                                        //var @break = Exp.Label();
                                        //var add = JSArrayBuilder.Add(arrayVar.Expression, item.Expression);
                                        //var @breakStmt = Exp.Goto(@break);
                                        //var loop = Exp.Loop(Exp.Block(
                                        //    Exp.IfThenElse(
                                        //        IElementEnumeratorBuilder.MoveNext(en, item.Expression),
                                        //        add,
                                        //        breakStmt)
                                        //    ), @break);
                                        //inits.Add(loop);
                                        inits.Add(Exp.Assign(spid, JSArrayBuilder.NewFromElementEnumerator(en)));
                                    // }
                                    break;
                                case FastNodeType.ObjectPattern:
                                case FastNodeType.ArrayPattern:
                                    var ape = element;
                                    // nested array ...
                                    // nested object ...
                                    using (var te = scope.Top.GetTempVariable(typeof(JSValue)))
                                    {
                                        var check = IElementEnumeratorBuilder.MoveNext(en, te.Expression);
                                        inits.Add(check);
                                        CreateAssignment(inits, ape, te.Expression, true, newScope);
                                    }
                                    break;
                                default:
                                    // inits.Add(IElementEnumeratorBuilder.MoveNext(en, item.Expression));
                                    // break;
                                    throw new NotSupportedException($"{element.Type}");
                            }
                        }
                    }
                    return;
            }
            throw new NotImplementedException();
        }
    }
}
