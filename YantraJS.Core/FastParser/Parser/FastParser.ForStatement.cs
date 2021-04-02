#nullable enable
using System;
using System.Collections.Generic;
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
            } else throw stream.Unexpected();

            var @in = false;
            var of = false;

            AstExpression? inTarget = null;
            AstExpression? ofTarget = null;
            AstExpression? test = null;
            AstExpression? update = null;

            if (stream.CheckAndConsume(FastKeywords.@in))
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
                if (!ExpressionSequence(out update, TokenTypes.BracketEnd, true))
                    throw stream.Unexpected();
            }
            else stream.Unexpected();

            
            AstStatement statement;
            if (stream.CheckAndConsume(TokenTypes.CurlyBracketStart))
            {
                if (!Block(out var block))
                    throw stream.Unexpected();
                if (newScope && declaration != null)
                {
                    (beginNode, statement, update) = Desugar(declaration, in block.Statements, update);
                }
                else
                {
                    statement = block;
                }
            } else if (Statement(out statement))
            {
                if(newScope && declaration != null)
                {
                    (beginNode, statement, update) = Desugar(declaration, ArraySpan<AstStatement>.From(statement), update);
                }
            } else throw stream.Unexpected();

            if(@in)
            {
                node = new AstForInStatement(begin.Token, PreviousToken, beginNode, inTarget, statement);
                return true;
            }
            if (of)
            {
                node = new AstForOfStatement(begin.Token, PreviousToken, beginNode, ofTarget, statement);
                return true;
            }

            node = new AstForStatement(begin.Token, PreviousToken, beginNode, test, update, statement);
            return true;



            // modify the node as well...
            AstExpression AssignTempNames(FastList<(string id, string temp)> list, AstExpression e)
            {
                switch ((e.Type,e))
                {
                    case (FastNodeType.Identifier, AstIdentifier id):
                        var tempID = Interlocked.Increment(ref TempVarID).ToString();
                        list.Add((id.Name.Value!, tempID));
                        return new AstIdentifier(id.Start, tempID);
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
                            pattern.Elements[i] = AssignTempNames(list, e);
                        }
                        return pattern;
                    default:
                        throw new FastParseException(e.Start, $"Unknown token");
                }
            }
            


            (AstNode beginNode, AstStatement statement, AstExpression? update) Desugar(
                AstVariableDeclaration declaration, 
                in ArraySpan<AstStatement> body,
                AstExpression? update)
            {
                var statementList = new AstStatement[body.Length + 1];
                body.Copy(statementList, 1);


                var tempDeclarations = Pool.AllocateList<VariableDeclarator>();
                var scopedDeclarations = Pool.AllocateList<VariableDeclarator>();
                var list = Pool.AllocateList<(string id, string temp)>();
                try {

                    for (int i = 0; i < declaration.Declarators.Length; i++)
                    {
                        ref var d = ref declaration.Declarators[i];
                        var id = AssignTempNames(list, d.Identifier);
                        tempDeclarations.Add(new VariableDeclarator(id, d.Init));
                        scopedDeclarations.Add(new VariableDeclarator(d.Identifier, id));
                    }

                    if (update != null)
                    {
                        update = AstIdentifierReplacer.Replace(update, list)
                            as AstExpression;
                    }


                    statementList[0] = new AstVariableDeclaration(declaration.Start, declaration.End, scopedDeclarations, FastVariableKind.Let);

                    var r = new AstVariableDeclaration(declaration.Start, declaration.End, tempDeclarations);

                    var last = body.Length == 0 ? declaration :  body[body.Length - 1];
                    return (r, new AstBlock(r.Start, last.End, ArraySpan<AstStatement>.From(statementList)), update);

                } finally {
                    tempDeclarations.Clear();
                    scopedDeclarations.Clear();
                }
            }
        }


    }

}
