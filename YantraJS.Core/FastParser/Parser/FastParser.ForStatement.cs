using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

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
            AstVariableDeclaration declaration = null;

            var current = stream.Current;
            if (current.IsKeyword)
            {
                switch (current.Keyword)
                {
                    case FastKeywords.let:
                        if (!VariableDeclarationStatement(out declaration, isLet: true))
                            throw stream.Unexpected();
                        beginNode = declaration;
                        newScope = true;
                        break;
                    case FastKeywords.@const:
                        if (!VariableDeclarationStatement(out declaration, isConst: true))
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

            AstExpression inTarget = null;
            AstExpression ofTarget = null;
            AstExpression test = null;
            AstExpression preTest = null;

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
                if (!ExpressionSequence(out preTest, TokenTypes.BracketEnd, true))
                    throw stream.Unexpected();
            }
            else stream.Unexpected();

            
            AstStatement statement;
            if (stream.CheckAndConsume(TokenTypes.CurlyBracketStart))
            {
                if (!Block(out var block))
                    throw stream.Unexpected();
                if (newScope)
                {
                    (beginNode, statement) = Desugar(declaration, in block.Statements);
                }
                else
                {
                    statement = block;
                }
            } else if (Statement(out statement))
            {
                if(newScope)
                {
                    (beginNode, statement) = Desugar(declaration, ArraySpan<AstStatement>.From(statement));
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

            node = new AstForStatement(begin.Token, PreviousToken, beginNode, test, preTest, statement);
            return true;

            (AstNode beginNode, AstStatement statement) Desugar(
                AstVariableDeclaration declaration, 
                in ArraySpan<AstStatement> body)
            {
                var statementList = new AstStatement[body.Length + 1];
                body.Copy(statementList, 1);

                var tempDeclarations = Pool.AllocateList<VariableDeclarator>();
                var scopedDeclarations = Pool.AllocateList<VariableDeclarator>();
                try {

                    for (int i = 0; i < declaration.Declarators.Length; i++)
                    {
                        ref var d = ref declaration.Declarators[i];
                        var tempID = Interlocked.Increment(ref TempVarID);
                        var id = new AstIdentifier(d.Identifier.Start, tempID.ToString());
                        tempDeclarations.Add(new VariableDeclarator(id, d.Init));
                        scopedDeclarations.Add(new VariableDeclarator(d.Identifier, id));
                    }

                    statementList[0] = new AstVariableDeclaration(declaration.Start, declaration.End, scopedDeclarations, declaration.IsLet, declaration.IsConst);

                    var r = new AstVariableDeclaration(declaration.Start, declaration.End, tempDeclarations, false, false);

                    var last = body.Length == 0 ? declaration :  body[body.Length - 1];
                    return (r, new AstBlock(r.Start, last.End, ArraySpan<AstStatement>.From(statementList)));

                } finally {
                    tempDeclarations.Clear();
                    scopedDeclarations.Clear();
                }
            }
        }


    }

}
