using System;
using System.Collections.Generic;
using System.Text;

namespace YantraJS.Core.FastParser
{
    partial class FastParser
    {
        bool Template(out AstExpression node)
        {
            var begin = stream.Current;
            stream.Consume();
            var nodes = Pool.AllocateList<AstExpression>();
            try
            {
                nodes.Add(new AstLiteral(TokenTypes.String, begin));
                while (!stream.CheckAndConsume(TokenTypes.EOF))
                {
                    if (stream.CheckAndConsume(TokenTypes.TemplateEnd, out var end))
                    {
                        nodes.Add(new AstLiteral(TokenTypes.StrictlyEqual, end));
                        break;
                    }
                    if (stream.CheckAndConsume(TokenTypes.TemplatePart, out var token))
                    {
                        nodes.Add(new AstLiteral(TokenTypes.String, token));
                        continue;
                    }
                    if (Expression(out var exp))
                    {
                        nodes.Add(exp);
                        continue;
                    }
                    throw stream.Unexpected();
                }
                node = new AstTemplateExpression(begin, PreviousToken, nodes.ToSpan());
            }
            finally
            {
                nodes.Clear();
            }
            return true;
        }

    }

}
