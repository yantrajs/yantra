#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using YantraJS.Core.FastParser.Ast;

namespace YantraJS.Core.FastParser
{

    partial class FastParser
    {

        private static int TempVarID = 1;

        /// <summary>
        /// For ( in
        /// For ( of
        /// For await ( // not supported yet...
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        bool ForStatement(out AstStatement node)
        {
            var begin = Location;
            stream.Consume();

            if (stream.CheckAndConsume(FastKeywords.await))
                throw stream.Unexpected();

            stream.Expect(TokenTypes.BracketStart);

            AstNode beginNode;

            // desugar let/const in following scope
            bool newScope = false;
            AstVariableDeclaration? declaration = null;
            var scope = variableScope.Push(begin.Token, FastNodeType.ForStatement);
            try
            {

                var current = stream.Current;
                if (current.IsKeyword)
                {
                    switch (current.Keyword)
                    {
                        case FastKeywords.let:
                            if (!VariableDeclarationStatement(out declaration, FastVariableKind.Let))
                                throw stream.Unexpected();
                            beginNode = declaration;
                            newScope = true;
                            break;
                        case FastKeywords.@const:
                            if (!VariableDeclarationStatement(out declaration, FastVariableKind.Const))
                                throw stream.Unexpected();
                            beginNode = declaration;
                            newScope = true;
                            break;
                        case FastKeywords.var:
                            if (!VariableDeclarationStatement(out declaration))
                                throw stream.Unexpected();
                            beginNode = declaration;
                            break;
                        default:
                            throw stream.Unexpected();
                    }
                }
                else if (ExpressionSequence(out var expressions, TokenTypes.SemiColon, true))
                {
                    beginNode = expressions;
                }
                else throw stream.Unexpected();

                var @in = false;
                var of = false;

                AstExpression? inTarget = null;
                AstExpression? ofTarget = null;
                AstExpression? test = null;
                AstExpression? update = null;

                if (stream.CheckAndConsume(TokenTypes.In))
                {
                    @in = true;
                    if (!Expression(out inTarget))
                        throw stream.Unexpected();
                    stream.Expect(TokenTypes.BracketEnd);
                }
                else if (stream.CheckAndConsumeContextualKeyword(FastKeywords.of))
                {
                    of = true;
                    if (!Expression(out ofTarget))
                        throw stream.Unexpected();
                    stream.Expect(TokenTypes.BracketEnd);
                }
                else if (ExpressionSequence(out test, TokenTypes.SemiColon, true))
                {
                    if (test.Type == FastNodeType.EmptyExpression)
                        test = null;
                    if (!ExpressionSequence(out update, TokenTypes.BracketEnd, true))
                        throw stream.Unexpected();
                    if (update.Type == FastNodeType.EmptyExpression)
                        update = null;
                }
                else stream.Unexpected();


                AstStatement statement;
                if (stream.CheckAndConsume(TokenTypes.CurlyBracketStart))
                {
                    if (!Block(out var block))
                        throw stream.Unexpected();
                    if (newScope && declaration != null)
                    {
                        (beginNode, statement, update, test) = Desugar(declaration, in block.Statements, update, test);
                    }
                    else
                    {
                        statement = block;
                    }
                }
                else if (Statement(out statement))
                {
                    if (newScope && declaration != null)
                    {
                        (beginNode, statement, update, test) = Desugar(declaration, ArraySpan<AstStatement>.From(statement), update, test);
                    }
                }
                else throw stream.Unexpected();

                if (@in)
                {
                    node = new AstForInStatement(begin.Token, PreviousToken, beginNode, inTarget, statement);
                    scope.GetVariables();
                    return true;
                }
                if (of)
                {
                    node = new AstForOfStatement(begin.Token, PreviousToken, beginNode, ofTarget, statement);
                    scope.GetVariables();
                    return true;
                }

                node = new AstForStatement(begin.Token, PreviousToken, beginNode, test, update, statement);
                scope.GetVariables();
            } finally
            {
                scope.Dispose();
            }
            return true;



            // modify the node as well...
            AstExpression AssignTempNames(FastList<(string id, AstIdentifier temp)> list, AstExpression e)
            {
                switch ((e.Type,e))
                {
                    case (FastNodeType.Identifier, AstIdentifier id):
                        var tempID = Interlocked.Increment(ref TempVarID).ToString();
                        var temp = new AstIdentifier(id.Start, tempID);
                        list.Add((id.Name.Value!, temp));
                        return temp;
                    case (FastNodeType.SpreadElement, AstSpreadElement spreadElement):
                        return new AstSpreadElement(spreadElement.Start,spreadElement.End, AssignTempNames(list, spreadElement.Argument));
                    case (FastNodeType.ObjectPattern, AstObjectPattern pattern):
                        for (int i = 0; i < pattern.Properties.Length; i++)
                        {
                            ref var property = ref pattern.Properties[i];
                            pattern.Properties[i] = new ObjectProperty(property.Key, AssignTempNames(list, property.Value), property.Spread);
                        }
                        return pattern;
                    case (FastNodeType.ArrayPattern, AstArrayPattern pattern):
                        for (int i = 0; i < pattern.Elements.Length; i++)
                        {
                            ref var property = ref pattern.Elements[i];
                            pattern.Elements[i] = AssignTempNames(list, property);
                        }
                        return pattern;
                    default:
                        throw new FastParseException(e.Start, $"Unknown token");
                }
            }
            


            (AstNode beginNode, AstStatement statement, AstExpression? update, AstExpression? test) Desugar(
                AstVariableDeclaration declaration, 
                in ArraySpan<AstStatement> body,
                AstExpression? update,
                AstExpression? test)
            {
                var statementList = new AstStatement[body.Length + 1];
                body.Copy(statementList, 1);

                // for-of and for-in does not require identifier replacement
                // instead they need single identifier as a temp variable

                // both test/update are null for for-of and for-in

                var requiresReplacement = update != null || test != null;

                var tempDeclarations = Pool.AllocateList<VariableDeclarator>();
                var scopedDeclarations = Pool.AllocateList<VariableDeclarator>();
                var list = Pool.AllocateList<(string id, AstIdentifier temp)>();
                try {

                    for (int i = 0; i < declaration.Declarators.Length; i++)
                    {
                        ref var d = ref declaration.Declarators[i];
                        if (requiresReplacement)
                        {
                            var id = AssignTempNames(list, d.Identifier);
                            tempDeclarations.Add(new VariableDeclarator(id, d.Init));
                        } else
                        {
                            var tid = Interlocked.Increment(ref TempVarID).ToString();
                            var id = new AstIdentifier(d.Identifier.Start, tid);
                            tempDeclarations.Add(new VariableDeclarator(id));
                            scopedDeclarations.Add(new VariableDeclarator(d.Identifier, id));
                        }
                    }

                    var changes = list.ToSpan();

                    if (requiresReplacement)
                    {

                        foreach (var (id, temp) in changes)
                        {
                            scopedDeclarations.Add(new VariableDeclarator(new AstIdentifier(temp.Start, id), temp));
                        }

                        if (update != null)
                        {
                            update = AstIdentifierReplacer.Replace(update, in changes)
                                as AstExpression;
                        }
                        if (test != null)
                        {
                            test = AstIdentifierReplacer.Replace(test, in changes)
                                as AstExpression;
                        }
                    }


                    statementList[0] = new AstVariableDeclaration(declaration.Start, declaration.End, scopedDeclarations, FastVariableKind.Let);

                    var r = new AstVariableDeclaration(declaration.Start, declaration.End, tempDeclarations);

                    var last = body.Length == 0 ? declaration :  body[body.Length - 1];
                    var block = new AstBlock(r.Start, last.End, ArraySpan<AstStatement>.From(statementList));
                    if (requiresReplacement)
                    {
                        block.HoistingScope = changes.Select(x => x.id).ToList().ToArraySpan();
                    }
                    return (r, block, update, test);

                } finally {
                    tempDeclarations.Clear();
                    scopedDeclarations.Clear();
                }
            }
        }


    }

}
