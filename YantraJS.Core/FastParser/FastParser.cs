using System;
using System.Collections.Generic;
using System.Text;

namespace YantraJS.Core.FastParser
{

    public class FastParser
    {
        private readonly FastTokenStream stream;

        public FastParser(FastTokenStream stream)
        {
            this.stream = stream;

        }

        private Func<T> Consume<T>(Func<T> func)
        {
            return () => {
                stream.Consume();
                return func();
            };
        }

        public bool PropertyName(out FastNode propertyName)
        {
            throw new NotImplementedException();
        }

        public bool SingleExpression(out FastNode expression)
        {
            throw new NotImplementedException();
        }

        public bool SourceElements(out IList<FastNode> expression)
        {
            expression = new SparseList<FastNode>();
            while (SourceElement(out var node))
                expression.Add(node);
            return true;
        }

        public bool FunctionBody(out FastNode expression)
        {
            if (SourceElements(out var body))
            {

                return true;
            }
            expression = null;
            return false;
        }

        public bool ArrowFunctionBody(out FastNode expression)
        {
            using (var mark = stream.UndoMark())
            {
                if (SingleExpression(out expression))
                {
                    return mark.Commit();
                }
                if(stream.CheckAndConsume(TokenTypes.CurlyBracketStart))
                {
                    if (FunctionBody(out expression)) {
                        stream.Expect(TokenTypes.CurlyBracketEnd);
                        return mark.Commit();
                    }
                }
            }
            expression = null;
            return false;
        }

        public bool AssignmentOperator(out FastToken token)
        {
            token = stream.Current;
            switch (token.Type)
            {
                case TokenTypes.AssignMultiply:
                case TokenTypes.AssignDivide:
                case TokenTypes.AssignMod:
                case TokenTypes.AssignAdd:
                case TokenTypes.AssignSubtract:
                case TokenTypes.AssignLeftShift:
                case TokenTypes.AssignRightShift:
                case TokenTypes.AssignUnsignedRightShift:
                case TokenTypes.AssignBitwideAnd:
                case TokenTypes.AssignBitwideOr:
                case TokenTypes.AssignXor:
                case TokenTypes.AssignPower:
                    return true;

            }
            return false;
        }

        public bool Literal(out FastToken literal)
        {
            var c = stream.Current;
            switch (c.Type)
            {
                case TokenTypes.Null:
                case TokenTypes.True:
                case TokenTypes.False:
                case TokenTypes.String:
                case TokenTypes.Number:
                case TokenTypes.TemplateBegin:
                case TokenTypes.TemplateEnd:
                    literal = c;
                    return true;
            }
            literal = null;
            return false;
        }

        public bool Getter(out (FastToken identifier, FastNode propertyName)? property)
        {
            using (var mark = stream.UndoMark())
            {
                if (Keyword(out var token) && token.ContextualKeyword == FastKeywords.get)
                {

                    if (Identifier(out var id))
                    {
                        if (PropertyName(out var propertyName))
                        {
                            mark.Commit((id, propertyName));
                        }
                    }
                }
            }
            property = null;
            return false;
        }

        public bool Setter(out (FastToken identifier, FastNode propertyName)? property)
        {
            using (var mark = stream.UndoMark())
            {
                if (Keyword(out var token) && token.ContextualKeyword == FastKeywords.set)
                {

                    if(Identifier(out var id)) { 
                        if(PropertyName(out var propertyName))
                        {
                            mark.Commit((id, propertyName));
                        }
                    }
                }
            }
            property = null;
            return false;
        }

        public bool IdentifierName(out FastToken token)
        {
            var c = stream.Current;
            if(c.Type == TokenTypes.Identifier)
            {
                stream.Consume();
                token = c;
                return true;
            }
            return ReservedWord(out token);
        }

        public bool Identifier(out FastToken token)
        {
            var c = stream.Current;
            if(c.Type == TokenTypes.Identifier)
            {
                if (c.Keyword == FastKeywords.async)
                {
                    stream.Consume();
                    token = c;
                    return true;
                }
                if (!c.IsKeyword)
                {
                    stream.Consume();
                    token = c;
                    return true;
                }
            }
            token = null;
            return false;
        }

        public bool ReservedWord(out FastToken token)
        {
            if (Keyword(out token))
                return true;
            var c = stream.Current;
            switch (c.Type)
            {
                case TokenTypes.Null:
                case TokenTypes.True:
                case TokenTypes.False:
                    stream.Consume();
                    token = c;
                    return true;
            }
            return false;
        }

        public bool Keyword(out FastToken keyword)
        {
            var c = stream.Current;
            if (c.IsKeyword)
            {
                stream.Consume();
                keyword = c;
                return true;
            }
            keyword = null;
            return false;
        }

        public bool EOS()
        {
            var c = stream.Current;
            switch (c.Type)
            {
                case TokenTypes.EOF:
                case TokenTypes.LineTerminator:
                case TokenTypes.SemiColon:
                case TokenTypes.CurlyBracketEnd:
                    stream.Consume();
                    return true;
            }
            return false;
        }

        

    }
}
