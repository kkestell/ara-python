//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.9.3
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from Ara.g4 by ANTLR 4.9.3

// Unreachable code detected
#pragma warning disable 0162
// The variable '...' is assigned but its value is never used
#pragma warning disable 0219
// Missing XML comment for publicly visible type or member '...'
#pragma warning disable 1591
// Ambiguous reference in cref attribute
#pragma warning disable 419

using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using IToken = Antlr4.Runtime.IToken;

/// <summary>
/// This interface defines a complete generic visitor for a parse tree produced
/// by <see cref="AraParser"/>.
/// </summary>
/// <typeparam name="Result">The return type of the visit operation.</typeparam>
[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.9.3")]
[System.CLSCompliant(false)]
public interface IAraVisitor<Result> : IParseTreeVisitor<Result> {
	/// <summary>
	/// Visit a parse tree produced by <see cref="AraParser.program"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitProgram([NotNull] AraParser.ProgramContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="AraParser.function"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitFunction([NotNull] AraParser.FunctionContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="AraParser.parameter"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitParameter([NotNull] AraParser.ParameterContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="AraParser.block"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitBlock([NotNull] AraParser.BlockContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>variableInitialization</c>
	/// labeled alternative in <see cref="AraParser.statement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitVariableInitialization([NotNull] AraParser.VariableInitializationContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>variableDeclaration</c>
	/// labeled alternative in <see cref="AraParser.statement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitVariableDeclaration([NotNull] AraParser.VariableDeclarationContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>returnStatement</c>
	/// labeled alternative in <see cref="AraParser.statement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitReturnStatement([NotNull] AraParser.ReturnStatementContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>ifStatement</c>
	/// labeled alternative in <see cref="AraParser.statement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitIfStatement([NotNull] AraParser.IfStatementContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>expressionStatement</c>
	/// labeled alternative in <see cref="AraParser.statement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitExpressionStatement([NotNull] AraParser.ExpressionStatementContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>valueExpression</c>
	/// labeled alternative in <see cref="AraParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitValueExpression([NotNull] AraParser.ValueExpressionContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>stringLiteral</c>
	/// labeled alternative in <see cref="AraParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitStringLiteral([NotNull] AraParser.StringLiteralContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>infixExpression</c>
	/// labeled alternative in <see cref="AraParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitInfixExpression([NotNull] AraParser.InfixExpressionContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>functionCall</c>
	/// labeled alternative in <see cref="AraParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitFunctionCall([NotNull] AraParser.FunctionCallContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>parensExpression</c>
	/// labeled alternative in <see cref="AraParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitParensExpression([NotNull] AraParser.ParensExpressionContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>unaryExpression</c>
	/// labeled alternative in <see cref="AraParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitUnaryExpression([NotNull] AraParser.UnaryExpressionContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="AraParser.atom"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitAtom([NotNull] AraParser.AtomContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="AraParser.number"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitNumber([NotNull] AraParser.NumberContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>true</c>
	/// labeled alternative in <see cref="AraParser.bool"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitTrue([NotNull] AraParser.TrueContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>false</c>
	/// labeled alternative in <see cref="AraParser.bool"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitFalse([NotNull] AraParser.FalseContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="AraParser.name"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitName([NotNull] AraParser.NameContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="AraParser.type"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitType([NotNull] AraParser.TypeContext context);
}