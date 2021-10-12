#nullable enable
namespace YantraJS.Core.FastParser
{
    public class AstFunctionExpression : AstExpression
    {
        public bool Async;
        public bool Generator;
        public readonly AstIdentifier? Id;
        public readonly IFastEnumerable<VariableDeclarator> Params;
        public readonly AstStatement Body;
        public readonly bool IsArrowFunction;

        public AstFunctionExpression(
            FastToken token, 
            FastToken previousToken, 
            bool isArrow,
            bool isAsync, 
            bool generator, 
            AstIdentifier? id,
            IFastEnumerable<VariableDeclarator> declarators, AstStatement body)
            : base(token, FastNodeType.FunctionExpression, previousToken)
        {
            // detect duplicate parameter names...

            this.IsArrowFunction = isArrow;
            this.Async = isAsync;
            this.Generator = generator;
            this.Id = id;
            this.Params = declarators;
            this.Body = body;
        }

        public override string ToString()
        {
            if (IsArrowFunction)
            {
                if (Async)
                {
                    if (Generator)
                    {
                        if (Id != null)
                            return $"async *{Id}({Params.Join()}) => {Body}";
                        return $"async *({Params.Join()}) => {Body}";
                    }
                    if (Id != null)
                        return $"async {Id}({Params.Join()}) => {Body}";
                    return $"async ({Params.Join()}) => {Body}";

                }
                if (Generator)
                {
                    if (Id != null)
                        return $"*{Id}({Params.Join()}) => {Body}";
                    return $"*({Params.Join()}) => {Body}";
                }
                if (Id != null)
                    return $"{Id}({Params.Join()}) => {Body}";
                return $"({Params.Join()}) => {Body}";

            }
            if (Async)
            {
                if (Generator)
                {
                    if (Id != null)
                        return $"async function *{Id}({Params.Join()}) {Body}";
                    return $"async function *({Params.Join()}) {Body}";
                }
                if (Id != null)
                    return $"async function {Id}({Params.Join()}) {Body}";
                return $"async function ({Params.Join()}) {Body}";

            }
            if (Generator)
            {
                if (Id != null)
                    return $"function *{Id}({Params.Join()}) {Body}";
                return $"function *({Params.Join()}) {Body}";
            }
            if (Id != null)
                return $"function {Id}({Params.Join()}) {Body}";
            return $"function ({Params.Join()}) {Body}";
        }
    }
}
