using System;
using System.Collections.Generic;
using System.Text;

namespace YantraJS.Core.FastParser
{


    partial class FastParser
    {


        bool Class(out AstStatement statement)
        {
            statement = default;
            if(ClassExpression(out var node, isStatement: true))
            {
                statement = new AstExpressionStatement(node);
                return true;
            }
            return false;
                
        }


        bool ClassExpression(out AstExpression statement, bool isStatement = false)
        {
            var begin = stream.Current;
            statement = default;

            var next = stream.Consume();
            AstIdentifier identifier = null;
            AstExpression @base = null;

            if (next.Type != TokenTypes.CurlyBracketStart)
            {

                if (next.Keyword != FastKeywords.extends)
                {
                    if (!Identitifer(out identifier))
                        throw stream.Unexpected();
                }



                if (stream.CheckAndConsume(FastKeywords.extends))
                {
                    if (!Expression(out @base))
                        throw stream.Unexpected();
                }
            }

            stream.Expect(TokenTypes.CurlyBracketStart);
            

            var nodes = new Sequence<AstClassProperty>();
            try
            {

                while (!stream.CheckAndConsume(TokenTypes.CurlyBracketEnd))
                {
                    if(ObjectProperty(out var property, true, isClass: true))
                    {
                        nodes.Add(property);
                    }
                    stream.CheckAndConsumeWithLineTerminator(TokenTypes.SemiColon);
                }
                if(identifier != null) {
                    this.variableScope.Top.AddVariable(identifier.Start,
                        identifier.Name, isStatement ? FastVariableKind.Let : FastVariableKind.Var , throwError: false);
                }
                statement = new AstClassExpression(begin, PreviousToken, identifier, @base, nodes);
            }
            finally
            {
                //nodes.Clear();
            }


            return true;
        }

        //bool ClassExpression(out AstExpression statement)
        //{
        //    var begin = Location;
        //    statement = default;

        //    stream.Consume();
        //    if (!Identitifer(out var identifier))
        //        throw stream.Unexpected();

        //    AstExpression @base = null;

        //    if (stream.CheckAndConsume(FastKeywords.extends))
        //    {
        //        if (!Expression(out @base))
        //            throw stream.Unexpected();
        //    }

        //    stream.Expect(TokenTypes.CurlyBracketStart);

        //    var nodes = Pool.AllocateList<AstClassProperty>();
        //    try
        //    {

        //        while(!stream.CheckAndConsume(TokenTypes.CurlyBracketEnd))
        //        {
        //            var @static = false;
        //            var @async = false;
        //            var generator = false;
        //            var isPrivate = false;
        //            var isProperty = false;
        //            var propertyKind = AstPropertyKind.None;

        //            if (stream.CheckAndConsume(FastKeywords.@static))
        //            {
        //                @static = true;
        //            }

        //            if (stream.CheckAndConsume(TokenTypes.Multiply))
        //            {
        //                generator = true;
        //            }
        //            else if (stream.CheckAndConsume(FastKeywords.async))
        //            {
        //                @async = true;
        //            }
        //            if (stream.CheckAndConsume(TokenTypes.Hash))
        //            {
        //                isPrivate = true;
        //            }

        //            if (stream.CheckAndConsumeContextualKeyword(FastKeywords.get))
        //            {
        //                propertyKind = AstPropertyKind.Get;
        //                isProperty = true;
        //            }
        //            else if (stream.CheckAndConsumeContextualKeyword(FastKeywords.set))
        //            {
        //                propertyKind = AstPropertyKind.Set;
        //                isProperty = true;
        //            }

        //            if (!PropertyName(out var propertyName))
        //                throw stream.Unexpected();

        //            if (stream.CheckAndConsume(TokenTypes.Assign))
        //            {
        //                // member assignment... 
        //                // cannot be true with get/set
        //                if (isProperty)
        //                    throw stream.Unexpected();

        //                if (!SingleExpression(out var init))
        //                    throw stream.Unexpected();
        //                propertyKind = AstPropertyKind.Data;
        //                nodes.Add(new AstClassProperty(propertyKind, isPrivate, @static, propertyName, init));
        //            }
        //            else if (Parameters(out var declarators))
        //            {
        //                if(propertyName.Start.ContextualKeyword == FastKeywords.constructor)
        //                {
        //                    propertyKind = AstPropertyKind.Constructor;
        //                }

        //                if (!Statement(out var body))
        //                    throw stream.Unexpected();

        //                nodes.Add(new AstClassProperty(propertyKind, isPrivate, @static, async, generator, propertyName, declarators, body));
        //            }
        //            else stream.Unexpected();

        //            if (EndOfStatement())
        //                continue;

        //        }
        //        statement = new AstClassExpression(begin.Token, PreviousToken, identifier, @base, nodes);
        //    }
        //    finally {
        //        nodes.Clear();
        //    }


        //    return true;
        //}


    }

}
