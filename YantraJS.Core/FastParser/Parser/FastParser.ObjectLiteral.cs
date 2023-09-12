using System;
using System.Collections.Generic;
using System.Text;

namespace YantraJS.Core.FastParser
{

   
    partial class FastParser
    {

        FastToken lastObjectPropertyIndex;

        bool ObjectProperty(
            out AstClassProperty property, 
            bool checkContextualKeyword = true,
            bool isAsync = false,
            bool isClass = false)
        {

            PreventStackoverFlow(ref lastObjectPropertyIndex);

            var begin = BeginUndo();
            var current = begin.Token;

            var isStatic = isClass ? stream.CheckAndConsume(FastKeywords.@static) : false;


            // check for async method.. async getter/setter are not supported yet...
            if (stream.CheckAndConsume(FastKeywords.async))
            {
                if(ObjectProperty(out property, true, isClass: isClass, isAsync: true))
                {
                    if (property.Kind == AstPropertyKind.Get || property.Kind == AstPropertyKind.Set)
                        throw stream.Unexpected();
                    property = new AstClassProperty(
                        current,
                        property.End,
                        AstPropertyKind.Method,
                        isAsync,
                        isStatic,
                        property.Key,
                        property.Computed,
                        property.Init);
                    return true;
                }
                begin.Reset();
            }

            stream.SkipNewLines();

            var sc = stream.Current;
            var isGet = sc.ContextualKeyword == FastKeywords.get;
            var isSet = sc.ContextualKeyword == FastKeywords.set;

            bool isGenerator = stream.CheckAndConsume(TokenTypes.Multiply);
            if(PropertyName(out var key, out var computed, acceptKeywords: true))
            {
                if(checkContextualKeyword && ( isSet || isGet))
                {
                    if (ObjectProperty(out property, isClass: isClass, isAsync: isAsync)) {
                        property = new AstClassProperty(
                            current,
                            property.End,
                            isSet ? AstPropertyKind.Set : AstPropertyKind.Get,
                            false,
                            isStatic,
                            property.Key,
                            property.Computed,
                            property.Init);
                        return true;
                    }
                }

                stream.SkipNewLines();
                if (stream.CheckAndConsume(TokenTypes.Assign))
                {
                    if (!checkContextualKeyword)
                        throw stream.Unexpected();
                    if (!Expression(out var value))
                        throw stream.Unexpected();
                    property = new AstClassProperty(
                        current,
                        PreviousToken,
                        AstPropertyKind.Data,
                        false,
                        false,
                        key,
                        computed,
                        value);
                    stream.CheckAndConsume(TokenTypes.SemiColon);
                    return true;
                }

                if (stream.CheckAndConsume(TokenTypes.Colon))
                {
                    if (!checkContextualKeyword)
                        throw stream.Unexpected();
                    if (!Expression(out var value))
                        throw stream.Unexpected();

                    property = new AstClassProperty(
                        current,
                        PreviousToken,
                        AstPropertyKind.Data,
                        false,
                        false,
                        key,
                        computed,
                        value);
                    return true;
                } else if (stream.CheckAndConsume(TokenTypes.BracketStart))
                {
                    // add the scope...
                    var scope = this.variableScope.Push(PreviousToken, FastNodeType.FunctionExpression);
                    try {

                        if (!Parameters(out var parameters, checkForBracketStart: false))
                            throw stream.Unexpected();
                        if (!Statement(out var body))
                            throw stream.Unexpected();

                        var fx = new AstFunctionExpression(current, PreviousToken, false, isAsync, isGenerator, null, parameters, body);

                        property = new AstClassProperty(
                            current,
                            PreviousToken,
                            key.Start.ContextualKeyword == FastKeywords.constructor
                                ? AstPropertyKind.Constructor
                                : AstPropertyKind.Method,
                            false,
                            isStatic,
                            key,
                            computed, fx);
                        return true;
                    } finally {
                        scope.Dispose();
                    }
                } else if (stream.Current.Type == TokenTypes.Comma
                            || stream.Current.Type == TokenTypes.CurlyBracketEnd
                            || stream.Current.Type == TokenTypes.EOF) {
                    property = new AstClassProperty(current, PreviousToken, AstPropertyKind.Data, false, isStatic, 
                        key, computed,
                        key);
                    return true;
                } else throw stream.Unexpected();

            }
            property = default;
            return begin.Reset();
        }


        bool ObjectLiteral(out AstExpression node)
        {
            var begin = stream.Current;
            node = default;
            stream.Consume();
            var nodes = new Sequence<AstNode>();
            SkipNewLines();

            while (!stream.CheckAndConsumeAny(TokenTypes.CurlyBracketEnd, TokenTypes.EOF))
            {
                SkipNewLines();
                var current = this.stream.Current;
                if (stream.CheckAndConsume(TokenTypes.TripleDots))
                {
                    if (!Expression(out var exp))
                        throw stream.Unexpected();
                    nodes.Add(new AstSpreadElement(current, exp.End, exp));
                    continue;
                }
                if (ObjectProperty(out var property))
                    nodes.Add(property);
                if (stream.CheckAndConsume(TokenTypes.Comma))
                    continue;
            }

            node = new AstObjectLiteral(begin, PreviousToken, nodes);
            return true;
        }


    }

}
