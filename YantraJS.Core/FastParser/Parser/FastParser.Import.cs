#nullable enable
using System;
using System.Collections.Generic;
using System.Text;

namespace YantraJS.Core.FastParser
{
    partial class FastParser
    {
        bool Import(FastToken token, out AstStatement statement)
        {
            stream.Consume();

            AstIdentifier id;

            if (stream.CheckAndConsume(TokenTypes.Multiply))
            {
                stream.ExpectContextualKeyword(FastKeywords.@as);
                if (!Identitifer(out id))
                    throw stream.Unexpected();

                stream.ExpectContextualKeyword(FastKeywords.from);

                var literal = ExpectStringLiteral();
                this.isAsync = true;
                statement = new AstImportStatement(token, null, id, null, literal);
                return true;

            }
            AstIdentifier? all = null;
            IFastEnumerable<(StringSpan, StringSpan)>? names = null;
            if(Identitifer(out id))
            {
                if(stream.CheckAndConsume(TokenTypes.Comma))
                {
                    if (stream.CheckAndConsume(TokenTypes.Multiply))
                    {
                        stream.ExpectContextualKeyword(FastKeywords.@as);
                        if (!Identitifer(out all))
                            throw stream.Unexpected();
                    }
                    else if (ImportNames(out var n)) {
                        names = n;
                    } else throw stream.Unexpected();
                }

                stream.ExpectContextualKeyword(FastKeywords.from);
                var literal = ExpectStringLiteral();
                this.isAsync = true;
                statement = new AstImportStatement(token, id, all, names, literal);
                return true;
            }

            if(ImportNames(out names))
            {
                if (stream.CheckAndConsume(TokenTypes.Comma))
                {
                    if (!Identitifer(out id))
                        throw stream.Unexpected();
                }
                stream.ExpectContextualKeyword(FastKeywords.from);
                var literal = ExpectStringLiteral();
                this.isAsync = true;
                statement = new AstImportStatement(token, id, all, names, literal);
                return true;
            }



            throw stream.Unexpected();

            bool ImportNames(out IFastEnumerable<(StringSpan,StringSpan)>? names)
            {
                if (!stream.CheckAndConsume(TokenTypes.CurlyBracketStart))
                {
                    names = null;
                    return false;
                }

                var list = new Sequence<(StringSpan, StringSpan)>();

                try {

                    while(!stream.CheckAndConsume(TokenTypes.CurlyBracketEnd))
                    {
                        if (!Identitifer(out var id))
                            throw stream.Unexpected();

                        if (stream.CheckAndConsumeContextualKeyword(FastKeywords.@as))
                        {
                            if (!Identitifer(out var asName))
                                throw stream.Unexpected();
                            list.Add((id.Name, asName.Name));
                        } else
                        {
                            list.Add((id.Name, id.Name));
                        }

                        if (stream.CheckAndConsume(TokenTypes.Comma))
                            continue;
                        if (stream.CheckAndConsume(TokenTypes.CurlyBracketEnd))
                            break;
                        throw stream.Unexpected();
                    }

                    names = list;
                    return true;
                } finally
                {
                    // list.Clear();
                }
            }
        }
    }

}
