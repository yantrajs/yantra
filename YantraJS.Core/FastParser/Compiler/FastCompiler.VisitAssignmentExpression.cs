using System;
using System.Collections.Generic;
using System.Linq;
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
                case FastNodeType.Identifier:
                    var id = left as AstIdentifier;
                    id.VerifyIdentifierForUpdate();
                    break;
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
                        Assign(leftExp, right, assignmentOperator),
                        tmp.Expression
                        );

                }
            }
            return Assign(Visit(left), right, assignmentOperator);
        }

        private Exp Assign(Expression exp, AstExpression right, TokenTypes assignmentOperator)
        {
            if(assignmentOperator == TokenTypes.AssignAdd)
            {
                if(right.Type == FastNodeType.Literal && right is AstLiteral literal)
                {
                    if(literal.TokenType == TokenTypes.String)
                    {
                        return Expression.Assign(exp, JSValueBuilder.AddString(exp, Expression.Constant(literal.StringValue)));
                    }
                    if (literal.TokenType == TokenTypes.Number)
                    {
                        return Expression.Assign(exp, JSValueBuilder.AddDouble(exp, Expression.Constant(literal.NumericValue)));
                    }
                }
            }
            return BinaryOperation.Assign(exp, Visit(right), assignmentOperator);
        }

        private Exp CreateAssignment(
            AstExpression pattern,
            Exp init,
            bool createVariable = false,
            bool newScope = false)
        {
            var inits = new Sequence<Exp>();
            CreateAssignment(inits, pattern, init, createVariable, newScope);
            // var span = inits.ToArray();
            // inits.Clear();
            return Exp.Block(inits);
        }

        private void CreateAssignment(
            Sequence<Exp> inits,
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
                    {
                        var en = objectPattern.Properties.GetFastEnumerator();
                        while(en.MoveNext(out var property))
                        {
                            Exp start = null;
                            switch (property.Key.Type)
                            {
                                case FastNodeType.Identifier:
                                case FastNodeType.Literal:
                                    var id = property.Key;
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
                    }
                    return;
                case FastNodeType.ArrayPattern:
                    var arrayPattern = pattern as AstArrayPattern;
                    using (var enVar = this.scope.Top.GetTempVariable(typeof(IElementEnumerator)))
                    {
                        var destExp = enVar.Expression;
                        inits.Add(Exp.Assign(destExp, IElementEnumeratorBuilder.Get(init)));
                        var en = arrayPattern.Elements.GetFastEnumerator();
                        while(en.MoveNext(out var element))
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
                                    inits.Add(IElementEnumeratorBuilder.AssignMoveNext(assignee, destExp));
                                    break;
                                case FastNodeType.BinaryExpression:
                                    var be = element as AstBinaryExpression;
                                    if (be.Left.Type != FastNodeType.Identifier)
                                    {
                                        using (var te = scope.Top.GetTempVariable(typeof(JSValue)))
                                        {
                                            inits.Add(IElementEnumeratorBuilder.MoveNext(destExp, te.Expression));
                                            inits.Add(JSValueExtensionsBuilder.AssignCoalesce(te.Expression, Visit(be.Right)));
                                            CreateAssignment(inits, be.Left, te.Expression, true, newScope);
                                        }
                                        break;
                                    }
                                    id = be.Left as AstIdentifier;
                                    if (createVariable)
                                    {
                                        this.scope.Top.CreateVariable(id.Name.Value, null, newScope);
                                    }
                                    assignee = VisitIdentifier(id);
                                    inits.Add(IElementEnumeratorBuilder.AssignMoveNext(assignee, destExp));
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
                                    inits.Add(Exp.Assign(spid, JSArrayBuilder.NewFromElementEnumerator(destExp)));
                                    break;
                                case FastNodeType.ObjectPattern:
                                case FastNodeType.ArrayPattern:
                                    var ape = element;
                                    // nested array ...
                                    // nested object ...
                                    using (var te = scope.Top.GetTempVariable(typeof(JSValue)))
                                    {
                                        var check = IElementEnumeratorBuilder.MoveNext(destExp, te.Expression);
                                        inits.Add(check);
                                        CreateAssignment(inits, ape, te.Expression, true, newScope);
                                    }
                                    break;
                                default:
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
