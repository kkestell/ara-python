using System;
using System.Collections.Immutable;
using Pidgin;
using Pidgin.Comment;
using Pidgin.Expression;
using static Pidgin.Parser;
using static Pidgin.Parser<char>;

namespace Ara
{
    public class Parser
    {
        private const char LeftParen = '(';
        private const char RightParen = ')';
        private const char DoubleQuote = '"';
        private const char ForwardSlash = '/';
        private const char Plus = '+';
        private const char Minus = '-';
        private const char Star = '*';
        private const char Comma = ',';
        private const char SemiColon = ';';

        private static readonly Parser<char, Unit> LineComment =
            CommentParser.SkipLineComment(Tok(String("//")));

        private static readonly Parser<char, Unit> BlockComment =
            CommentParser.SkipBlockComment(Tok(String("/*")), Tok(String("*/")));

        private static readonly Parser<char, IExpr> Identifier =
            Tok(Letter.Then(LetterOrDigit.ManyString(), (first, rest) => first + rest))
                .Select<IExpr>(name => new Identifier(name))
                .Labelled("Identifier");

        private static readonly Parser<char, IExpr> Number =
            Tok(Real)
                .Select<IExpr>(value => new Number(value))
                .Labelled("Number");

        private static readonly Parser<char, IExpr> String =
            Tok(Token(c => c != DoubleQuote).ManyString().Between(Char(DoubleQuote)))
                .Select<IExpr>(value => new String(value))
                .Labelled("String");

        private static readonly Parser<char, Func<IExpr, IExpr>> Negate =
            Unary(Tok(Char(Minus))
                .ThenReturn(UnaryOperatorType.Negate));

        private static readonly Parser<char, Func<IExpr, IExpr, IExpr>> Add =
            Binary(Tok(Char(Plus))
                .ThenReturn(BinaryOperatorType.Add));

        private static readonly Parser<char, Func<IExpr, IExpr, IExpr>> Subtract =
            Binary(Tok(Char(Minus))
                .ThenReturn(BinaryOperatorType.Subtract));

        private static readonly Parser<char, Func<IExpr, IExpr, IExpr>> Multiply =
            Binary(Tok(Char(Star))
                .ThenReturn(BinaryOperatorType.Multiply));

        private static readonly Parser<char, Func<IExpr, IExpr, IExpr>> Divide =
            Binary(Tok(Char(ForwardSlash))
                .ThenReturn(BinaryOperatorType.Divide));

        private static readonly Parser<char, IExpr> Expr =
            ExpressionParser.Build<char, IExpr>(expr =>
                (
                    OneOf(
                        Identifier,
                        Number,
                        String,
                        Parenthesised(expr).Labelled("Parenthesised Expression")
                    ),
                    new[]
                    {
                        Operator.PostfixChainable(Call(expr)),
                        Operator.Prefix(Negate),
                        Operator.InfixL(Multiply),
                        Operator.InfixL(Divide),
                        Operator.InfixL(Add),
                        Operator.InfixL(Subtract)
                    }
                )
            ).Labelled("Expression");

        private static Parser<char, T> Tok<T>(Parser<char, T> p) =>
            Try(p).Before(SkipWhitespaces);

        private static Parser<char, T> Parenthesised<T>(Parser<char, T> p) =>
            p.Between(Tok(Char(LeftParen)), Tok(Char(RightParen)));

        private static Parser<char, Func<IExpr, IExpr>> Unary(Parser<char, UnaryOperatorType> op) =>
            op.Select<Func<IExpr, IExpr>>(type => o => new UnaryOp(type, o));

        private static Parser<char, Func<IExpr, IExpr, IExpr>> Binary(Parser<char, BinaryOperatorType> op) =>
            op.Select<Func<IExpr, IExpr, IExpr>>(type => (l, r) => new BinaryOp(type, l, r));

        private static Parser<char, Func<IExpr, IExpr>> Call(Parser<char, IExpr> subExpr) =>
            Parenthesised(subExpr.Separated(Tok(Char(Comma))))
                .Select<Func<IExpr, IExpr>>(args => method => new Call(method, args.ToImmutableArray()))
                .Labelled("Function Call");

        public static IExpr ParseOrThrow(string input) =>
            Expr.ParseOrThrow(input);
    }
}
