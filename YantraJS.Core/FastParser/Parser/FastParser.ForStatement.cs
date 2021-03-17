using System;
using System.Collections.Generic;
using System.Text;

namespace YantraJS.Core.FastParser
{

    partial class FastParser
    {



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

            AstNode beginNode = null;

            var current = stream.Current;
            if (current.IsKeyword)
            {
                switch (current.Keyword)
                {
                    case FastKeywords.let:
                        if (!VariableDeclaration(out var declaration, isLet: true))
                            throw stream.Unexpected();
                        beginNode = declaration;
                        break;
                    case FastKeywords.@const:
                        if (!VariableDeclaration(out declaration, isConst: true))
                            throw stream.Unexpected();
                        beginNode = declaration;
                        break;
                    case FastKeywords.var:
                        if (!VariableDeclaration(out declaration))
                            throw stream.Unexpected();
                        beginNode = declaration;
                        break;

                }
            }
            else if (ExpressionSequence(out var expressions))
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
            }
            else if (stream.CheckAndConsumeContextualKeyword(FastKeywords.of))
            {
                of = true;
                if (!Expression(out ofTarget))
                    throw stream.Unexpected();
            }
            else if (stream.CheckAndConsume(TokenTypes.SemiColon))
            {
                if (!Expression(out test))
                    throw stream.Unexpected();
                stream.Expect(TokenTypes.SemiColon);
                if (!Expression(out preTest))
                    throw stream.Unexpected();
            }
            else stream.Unexpected();

            stream.Expect(TokenTypes.BracketEnd);

            if (!Statement(out var statement))
                throw stream.Unexpected();
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
        }


    }

}
