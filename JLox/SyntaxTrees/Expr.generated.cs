using JLox.Scanning;

namespace JLox.SyntaxTrees;

internal abstract class Expr
{
    internal interface Visitor<R>
    {
        R VisitBinaryExpr(Binary expr);
        R VisitGroupingExpr(Grouping expr);
        R VisitLiteralExpr(Literal expr);
        R VisitUnaryExpr(Unary expr);
    }

    internal abstract R Accept<R>(Visitor<R> visitor);

    // The AST classes
    internal class Binary : Expr
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

        internal override R Accept<R>(Visitor<R> visitor)
        {
            return visitor.VisitBinaryExpr(this);
        }
    }

    internal class Grouping : Expr
    {
        private readonly Expr _expression;

        public Grouping(Expr expression)
        {
            _expression = expression;
        }

        internal override R Accept<R>(Visitor<R> visitor)
        {
            return visitor.VisitGroupingExpr(this);
        }
    }

    internal class Literal : Expr
    {
        private readonly object _value;

        public Literal(object value)
        {
            _value = value;
        }

        internal override R Accept<R>(Visitor<R> visitor)
        {
            return visitor.VisitLiteralExpr(this);
        }
    }

    internal class Unary : Expr
    {
        private readonly Token _op;
        private readonly Expr _right;

        public Unary(Token op, Expr right)
        {
            _op = op;
            _right = right;
        }

        internal override R Accept<R>(Visitor<R> visitor)
        {
            return visitor.VisitUnaryExpr(this);
        }
    }
}
