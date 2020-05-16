//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.8
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from Tapl.g4 by ANTLR 4.8

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
/// by <see cref="TaplParser"/>.
/// </summary>
/// <typeparam name="Result">The return type of the visit operation.</typeparam>
[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.8")]
[System.CLSCompliant(false)]
public interface ITaplVisitor<Result> : IParseTreeVisitor<Result> {
	/// <summary>
	/// Visit a parse tree produced by <see cref="TaplParser.toplevel"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitToplevel([NotNull] TaplParser.ToplevelContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="TaplParser.command"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitCommand([NotNull] TaplParser.CommandContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="TaplParser.bind"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitBind([NotNull] TaplParser.BindContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>par</c>
	/// labeled alternative in <see cref="TaplParser.term"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitPar([NotNull] TaplParser.ParContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>app</c>
	/// labeled alternative in <see cref="TaplParser.term"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitApp([NotNull] TaplParser.AppContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>abs</c>
	/// labeled alternative in <see cref="TaplParser.term"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitAbs([NotNull] TaplParser.AbsContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>var</c>
	/// labeled alternative in <see cref="TaplParser.term"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitVar([NotNull] TaplParser.VarContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>ift</c>
	/// labeled alternative in <see cref="TaplParser.term"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitIft([NotNull] TaplParser.IftContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>true</c>
	/// labeled alternative in <see cref="TaplParser.term"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitTrue([NotNull] TaplParser.TrueContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>false</c>
	/// labeled alternative in <see cref="TaplParser.term"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitFalse([NotNull] TaplParser.FalseContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>let</c>
	/// labeled alternative in <see cref="TaplParser.term"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitLet([NotNull] TaplParser.LetContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>bool</c>
	/// labeled alternative in <see cref="TaplParser.type"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitBool([NotNull] TaplParser.BoolContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>arrow</c>
	/// labeled alternative in <see cref="TaplParser.type"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitArrow([NotNull] TaplParser.ArrowContext context);
}
