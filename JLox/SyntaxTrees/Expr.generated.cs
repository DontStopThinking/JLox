using JLox.Scanning;

namespace JLox.SyntaxTrees;

internal abstract class Expr
{
    internal class Binary
    {
        private readonly Expr _left;
        private readonly Token _op;
        private readonly Expr _right;

        public Binary(Expr left, Token op, Expr right)
        {
            _left = left;
            _op = op;
            _right = right;
        }
    }
    internal class Grouping
    {
        private readonly Expr _expression;

        public Grouping(Expr expression)
        {
            _expression = expression;
        }
    }
    internal class Literal
    {
        private readonly object _value;

        public Literal(object value)
        {
            _value = value;
        }
    }
    internal class Unary
    {
        private readonly Token _op;
        private readonly Expr _right;

        public Unary(Token op, Expr right)
        {
            _op = op;
            _right = right;
        }
    }
}
