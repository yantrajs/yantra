using System;
using System.Collections.Generic;
using System.Text;

namespace YantraJS.Core.FastParser
{
    partial class FastParser
    {
        AstTemplateExpression Template()
        {
            var begin = stream.Current;
            stream.Consume();
            var nodes = new Sequence<AstExpression>();
            try
            {
                nodes.Add(new AstLiteral(TokenTypes.TemplatePart, begin));
                while (!stream.CheckAndConsume(TokenTypes.EOF))
                {
                    if (stream.CheckAndConsume(TokenTypes.TemplateEnd, out var end))
                    {
                        nodes.Add(new AstLiteral(TokenTypes.TemplatePart, end));
                        break;
                    }
                    if (stream.CheckAndConsume(TokenTypes.TemplatePart, out var token))
                    {
                        nodes.Add(new AstLiteral(TokenTypes.TemplatePart, token));
                        continue;
                    }
                    if (Expression(out var exp))
                    {
                        nodes.Add(exp);
                        continue;
                    }
                    throw stream.Unexpected();
                }
                return new AstTemplateExpression(begin, PreviousToken, nodes);
            }
            finally
            {
                // nodes.Clear();
            }
        }

    }

}
