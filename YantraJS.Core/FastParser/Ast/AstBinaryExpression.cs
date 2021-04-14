#nullable enable
namespace YantraJS.Core.FastParser
{
    public class AstBinaryExpression : AstExpression
    {
        public readonly AstExpression Left;
        public readonly TokenTypes Operator;
        public readonly AstExpression Right;

        public AstBinaryExpression(AstExpression node, TokenTypes type, AstExpression right)
            : base (node.Start, FastNodeType.BinaryExpression, right.End)
        {
            this.Left = node;
            this.Operator = type;
            this.Right = right;
        }

        private string OperatorToString(TokenTypes type)
        {
            switch(type)
            {
                case TokenTypes.BooleanAnd:
                    return "&&";
                case TokenTypes.BooleanOr:
                    return "||";
                case TokenTypes.BitwiseAnd:
                    return "&";
                case TokenTypes.BitwiseOr:
                    return "|";
                case TokenTypes.Plus:
                    return "+";
                case TokenTypes.Minus:
                    return "-";
                case TokenTypes.Mod:
                    return "%";
                case TokenTypes.Multiply:
                    return "*";
                case TokenTypes.NotEqual:
                    return "!=";
                case TokenTypes.Equal:
                    return "==";
                case TokenTypes.StrictlyNotEqual:
                    return "!==";
                case TokenTypes.StrictlyEqual:
                    return "===";
                case TokenTypes.Assign:
                    return "=";
            }
            return type.ToString();
        }

        public override string ToString()
        {
            return $"({Left} {OperatorToString(Operator)} {Right})";
        }
    }
}