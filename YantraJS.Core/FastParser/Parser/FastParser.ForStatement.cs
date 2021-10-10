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
            var begin = stream.Current;
            stream.Consume();

            if (stream.CheckAndConsume(FastKeywords.await))
                throw stream.Unexpected();

            stream.Expect(TokenTypes.BracketStart);

            AstNode? beginNode;

            // desugar let/const in following scope
            bool newScope = false;
            AstVariableDeclaration? declaration = null;
            var scope = variableScope.Push(begin, FastNodeType.ForStatement);
            try
            {

                var @in = false;
                var of = false;

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
                else if (ExpressionList(out var expressions))
                {
                    beginNode = expressions;
                }
                else throw stream.Unexpected();


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
                    // case of automatic semicolon insertion
                    if (test.End.Type == TokenTypes.BracketEnd)
                        throw stream.Unexpected();
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
                        (beginNode, statement, update, test) = Desugar(declaration, block.Statements, update, test);
                    }
                    else
                    {
                        statement = block;
                    }
                }
                else if (NonDeclarativeStatement(out statement))
                {
                    if (newScope && declaration != null)
                    {
                        (beginNode, statement, update, test) = Desugar(declaration, new Sequence<AstStatement>(1) { statement }, update, test);
                    }
                }
                else throw stream.Unexpected();

                if (@in)
                {
                    node = new AstForInStatement(begin, PreviousToken, beginNode, inTarget, statement);
                    scope.GetVariables();
                    return true;
                }
                if (of)
                {
                    node = new AstForOfStatement(begin, PreviousToken, beginNode, ofTarget, statement);
                    scope.GetVariables();
                    return true;
                }

                node = new AstForStatement(begin, PreviousToken, beginNode, test, update, statement);
                scope.GetVariables();
            } finally
            {
                scope.Dispose();
            }
            return true;

            bool ExpressionList(
                out AstExpression? node)
            {
                var list = new Sequence<AstExpression>();
                var token = stream.Current;
                node = null;
                considerInOfAsOperators = false;
                while (true)
                {
                    if (stream.CheckAndConsume(TokenTypes.SemiColon))
                        break;
                    if (!Expression(out node))
                        throw stream.Unexpected();

                    var c = stream.Current;
                    if (c.Type == TokenTypes.In || c.ContextualKeyword == FastKeywords.of)
                        break;
                    if (stream.CheckAndConsume(TokenTypes.SemiColon))
                        break;
                    if (stream.CheckAndConsume(TokenTypes.Comma))
                    {
                        list.Add(node);
                        continue;
                    }
                }

                if (list.Any())
                {
                    node = new AstSequenceExpression(token, list.Last().End, list);
                }
                considerInOfAsOperators = true;
                return true;
            }



            // modify the node as well...
            AstExpression AssignTempNames(
                Sequence<(string id, AstIdentifier temp)> list,
                Sequence<StringSpan> hoisted,
                AstExpression e)
            {
                switch (e.Type)
                {
                    case FastNodeType.Identifier:
                        var id = e as AstIdentifier;
                        var tempID = Interlocked.Increment(ref TempVarID).ToString();
                        var temp = new AstIdentifier(id!.Start, tempID);
                        hoisted.Add(id.Name);
                        list.Add((id.Name.Value!, temp));
                        return temp;
                    case FastNodeType.SpreadElement:
                        var spreadElement = e as AstSpreadElement;
                        return new AstSpreadElement(spreadElement!.Start,spreadElement.End, AssignTempNames(list, hoisted, spreadElement.Argument));
                    case FastNodeType.ObjectPattern: 
                        var pattern = e as AstObjectPattern;
                        var pat = (pattern!.Properties as Sequence<ObjectProperty>)!;
                        for (int i = 0; i < pat.Count; i++)
                        {
                            var property = pat[i];
                            pat[i] = 
                                new ObjectProperty(
                                    property.Key, 
                                    AssignTempNames(list, hoisted, property.Value), property.Init, property.Spread);
                        }
                        return pattern;
                    case FastNodeType.ArrayPattern:
                        var arrayPattern = e as AstArrayPattern;
                        var elements = (arrayPattern!.Elements as Sequence<AstExpression>)!;
                        for (int i = 0; i < elements.Count; i++)
                        {
                            var property = elements[i];
                            elements[i] = AssignTempNames(list, hoisted, property);
                        }
                        return arrayPattern;
                    default:
                        throw new FastParseException(e.Start, $"Unknown token");
                }
            }
            


            (AstNode beginNode, AstStatement statement, AstExpression? update, AstExpression? test) Desugar(
                AstVariableDeclaration declaration, 
                IFastEnumerable<AstStatement> body,
                AstExpression? update,
                AstExpression? test)
            {
                var statementList = new Sequence<AstStatement>(body.Count + 1);
                // body.Copy(statementList, 1);
                statementList.Add(null!);
                statementList.AddRange(body);

                // for-of and for-in does not require identifier replacement
                // instead they need single identifier as a temp variable

                // both test/update are null for for-of and for-in

                var requiresReplacement = update != null || test != null;

                var tempDeclarations = new Sequence<VariableDeclarator>();
                var scopedDeclarations = new Sequence<VariableDeclarator>();
                var list = new Sequence<(string id, AstIdentifier temp)>();
                var hoisted = new Sequence<StringSpan>();
                try {
                    var en = declaration.Declarators.GetFastEnumerator();
                    while(en.MoveNext(out var d))
                    {
                        // ref var d = ref declaration.Declarators[i];
                        if (requiresReplacement)
                        {
                            var id = AssignTempNames(list , hoisted, d.Identifier);
                            tempDeclarations.Add(new VariableDeclarator(id, d.Init));
                        } else
                        {
                            var tid = Interlocked.Increment(ref TempVarID).ToString();
                            var id = new AstIdentifier(d.Identifier.Start, tid);
                            tempDeclarations.Add(new VariableDeclarator(id));
                            scopedDeclarations.Add(new VariableDeclarator(d.Identifier, id));
                        }
                    }

                    var changes = list;

                    if (requiresReplacement)
                    {

                        foreach (var (id, temp) in changes)
                        {
                            scopedDeclarations.Add(new VariableDeclarator(new AstIdentifier(temp.Start, id), temp));
                        }

                        if (update != null)
                        {
                            update = AstIdentifierReplacer.Replace(update, changes)
                                as AstExpression;
                        }
                        if (test != null)
                        {
                            test = AstIdentifierReplacer.Replace(test, changes)
                                as AstExpression;
                        }
                    }


                    statementList[0] = new AstVariableDeclaration(declaration.Start, declaration.End, scopedDeclarations, FastVariableKind.Let);

                    var r = new AstVariableDeclaration(declaration.Start, declaration.End, tempDeclarations);

                    var last = body.Count == 0 ? declaration :  body.Last();
                    var block = new AstBlock(r.Start, last.End, statementList);
                    if (requiresReplacement)
                    {
                        block.HoistingScope = hoisted;
                    }
                    return (r, block, update, test);

                } finally {
                    // tempDeclarations.Clear();
                    // scopedDeclarations.Clear();
                    // list.Clear();
                    // hoisted.Clear();
                }
            }
        }


    }

}
