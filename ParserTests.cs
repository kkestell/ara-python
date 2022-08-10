using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ara.Tests
{
    [TestClass]
    public class ParserTests
    {
        [TestMethod]
        public void Number()
        {
            var expr = Parser.ParseOrThrow("3.14");

            Assert.IsInstanceOfType(expr, typeof(Number));

            var number = (Number) expr;

            Assert.AreEqual(number.Value, 3.14);
        }

        [TestMethod]
        public void String()
        {
            var expr = Parser.ParseOrThrow("\"Hello World\"");

            Assert.IsInstanceOfType(expr, typeof(String));

            var str = (String) expr;

            Assert.AreEqual(str.Value, "Hello World");
        }

        [TestMethod]
        public void Identifier()
        {
            var expr = Parser.ParseOrThrow("foo");

            Assert.IsInstanceOfType(expr, typeof(Identifier));

            var identifier = (Identifier) expr;

            Assert.AreEqual(identifier.Name, "foo");
        }

        [TestMethod]
        public void UnaryNegation()
        {
            var expr = Parser.ParseOrThrow("-foo");

            Assert.IsInstanceOfType(expr, typeof(UnaryOp));

            var op = (UnaryOp) expr;

            Assert.AreEqual(op.Type, UnaryOperatorType.Negate);
            Assert.IsInstanceOfType(op.Expr, typeof(Identifier));
        }

        [TestMethod]
        public void BinaryAddition()
        {
            var expr = Parser.ParseOrThrow("1 + 2");

            Assert.IsInstanceOfType(expr, typeof(BinaryOp));

            var op = (BinaryOp) expr;

            Assert.AreEqual(op.Type, BinaryOperatorType.Add);
            Assert.IsInstanceOfType(op.Left, typeof(Number));
            Assert.IsInstanceOfType(op.Right, typeof(Number));
        }

        [TestMethod]
        public void BinarySubtraction()
        {
            var expr = Parser.ParseOrThrow("99 - 1");

            Assert.IsInstanceOfType(expr, typeof(BinaryOp));

            var op = (BinaryOp) expr;

            Assert.AreEqual(op.Type, BinaryOperatorType.Subtract);
            Assert.IsInstanceOfType(op.Left, typeof(Number));
            Assert.IsInstanceOfType(op.Right, typeof(Number));
        }

        [TestMethod]
        public void BinaryMultiplication()
        {
            var expr = Parser.ParseOrThrow("2 * 3");

            Assert.IsInstanceOfType(expr, typeof(BinaryOp));

            var op = (BinaryOp) expr;

            Assert.AreEqual(op.Type, BinaryOperatorType.Multiply);
            Assert.IsInstanceOfType(op.Left, typeof(Number));
            Assert.IsInstanceOfType(op.Right, typeof(Number));
        }

        [TestMethod]
        public void BinaryDivision()
        {
            var expr = Parser.ParseOrThrow("1 / 0");

            Assert.IsInstanceOfType(expr, typeof(BinaryOp));

            var op = (BinaryOp) expr;

            Assert.AreEqual(op.Type, BinaryOperatorType.Divide);
            Assert.IsInstanceOfType(op.Left, typeof(Number));
            Assert.IsInstanceOfType(op.Right, typeof(Number));
        }

        [TestMethod]
        public void Call()
        {
            var expr = Parser.ParseOrThrow("foo(1)");

            Assert.IsInstanceOfType(expr, typeof(Call));

            var call = (Call) expr;

            Assert.IsInstanceOfType(call.Expr, typeof(Identifier));
            Assert.AreEqual(call.Arguments.Length, 1);
            Assert.IsInstanceOfType(call.Arguments.First(), typeof(Number));
        }
    }
}
