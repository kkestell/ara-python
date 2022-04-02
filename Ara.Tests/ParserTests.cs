using System.Linq;
using Ara.Ast;
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

            Assert.IsInstanceOfType(expr, typeof(Literal));

            var value = (Literal)expr;

            Assert.AreEqual((double)value, 3.14);
        }

        [TestMethod]
        public void String()
        {
            var expr = Parser.ParseOrThrow("\"Hello World\"");

            Assert.IsInstanceOfType(expr, typeof(Literal));

            var value = (Literal)expr;

            Assert.AreEqual((string)value, "Hello World");
        }

        /*
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

            Assert.AreEqual(op.Type, UnaryOpType.Negate);
            Assert.IsInstanceOfType(op.Expr, typeof(Identifier));
        }

        [TestMethod]
        public void BinaryAddition()
        {
            var expr = Parser.ParseOrThrow("1 + 2");

            Assert.IsInstanceOfType(expr, typeof(BinaryOp));

            var op = (BinaryOp) expr;

            Assert.AreEqual(op.Type, BinaryOpType.Add);
            Assert.IsInstanceOfType(op.Left, typeof(Number));
            Assert.IsInstanceOfType(op.Right, typeof(Number));
        }

        [TestMethod]
        public void BinarySubtraction()
        {
            var expr = Parser.ParseOrThrow("99 - 1");

            Assert.IsInstanceOfType(expr, typeof(BinaryOp));

            var op = (BinaryOp) expr;

            Assert.AreEqual(op.Type, BinaryOpType.Subtract);
            Assert.IsInstanceOfType(op.Left, typeof(Number));
            Assert.IsInstanceOfType(op.Right, typeof(Number));
        }

        [TestMethod]
        public void BinaryMultiplication()
        {
            var expr = Parser.ParseOrThrow("2 * 3");

            Assert.IsInstanceOfType(expr, typeof(BinaryOp));

            var op = (BinaryOp) expr;

            Assert.AreEqual(op.Type, BinaryOpType.Multiply);
            Assert.IsInstanceOfType(op.Left, typeof(Number));
            Assert.IsInstanceOfType(op.Right, typeof(Number));
        }

        [TestMethod]
        public void BinaryDivision()
        {
            var expr = Parser.ParseOrThrow("1 / 0");

            Assert.IsInstanceOfType(expr, typeof(BinaryOp));

            var op = (BinaryOp) expr;

            Assert.AreEqual(op.Type, BinaryOpType.Divide);
            Assert.IsInstanceOfType(op.Left, typeof(Number));
            Assert.IsInstanceOfType(op.Right, typeof(Number));
        }

        [TestMethod]
        public void Call()
        {
            var expr = Parser.ParseOrThrow("quux(1)");

            Assert.IsInstanceOfType(expr, typeof(Call));

            var call = (Call)expr;

            Assert.IsInstanceOfType(call.Expr, typeof(Identifier));
            Assert.AreEqual(call.Arguments.Length, 1);
            Assert.IsInstanceOfType(call.Arguments.First(), typeof(Number));
        }
        */
    }
}
