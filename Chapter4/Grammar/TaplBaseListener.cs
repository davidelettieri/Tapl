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
using IErrorNode = Antlr4.Runtime.Tree.IErrorNode;
using ITerminalNode = Antlr4.Runtime.Tree.ITerminalNode;
using IToken = Antlr4.Runtime.IToken;
using ParserRuleContext = Antlr4.Runtime.ParserRuleContext;

/// <summary>
/// This class provides an empty implementation of <see cref="ITaplListener"/>,
/// which can be extended to create a listener which only needs to handle a subset
/// of the available methods.
/// </summary>
[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.8")]
[System.CLSCompliant(false)]
public partial class TaplBaseListener : ITaplListener {
	/// <summary>
	/// Enter a parse tree produced by the <c>par</c>
	/// labeled alternative in <see cref="TaplParser.term"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterPar([NotNull] TaplParser.ParContext context) { }
	/// <summary>
	/// Exit a parse tree produced by the <c>par</c>
	/// labeled alternative in <see cref="TaplParser.term"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitPar([NotNull] TaplParser.ParContext context) { }
	/// <summary>
	/// Enter a parse tree produced by the <c>IfThenElse</c>
	/// labeled alternative in <see cref="TaplParser.term"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterIfThenElse([NotNull] TaplParser.IfThenElseContext context) { }
	/// <summary>
	/// Exit a parse tree produced by the <c>IfThenElse</c>
	/// labeled alternative in <see cref="TaplParser.term"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitIfThenElse([NotNull] TaplParser.IfThenElseContext context) { }
	/// <summary>
	/// Enter a parse tree produced by the <c>Succ</c>
	/// labeled alternative in <see cref="TaplParser.term"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterSucc([NotNull] TaplParser.SuccContext context) { }
	/// <summary>
	/// Exit a parse tree produced by the <c>Succ</c>
	/// labeled alternative in <see cref="TaplParser.term"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitSucc([NotNull] TaplParser.SuccContext context) { }
	/// <summary>
	/// Enter a parse tree produced by the <c>Pred</c>
	/// labeled alternative in <see cref="TaplParser.term"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterPred([NotNull] TaplParser.PredContext context) { }
	/// <summary>
	/// Exit a parse tree produced by the <c>Pred</c>
	/// labeled alternative in <see cref="TaplParser.term"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitPred([NotNull] TaplParser.PredContext context) { }
	/// <summary>
	/// Enter a parse tree produced by the <c>IsZero</c>
	/// labeled alternative in <see cref="TaplParser.term"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterIsZero([NotNull] TaplParser.IsZeroContext context) { }
	/// <summary>
	/// Exit a parse tree produced by the <c>IsZero</c>
	/// labeled alternative in <see cref="TaplParser.term"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitIsZero([NotNull] TaplParser.IsZeroContext context) { }
	/// <summary>
	/// Enter a parse tree produced by the <c>True</c>
	/// labeled alternative in <see cref="TaplParser.term"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterTrue([NotNull] TaplParser.TrueContext context) { }
	/// <summary>
	/// Exit a parse tree produced by the <c>True</c>
	/// labeled alternative in <see cref="TaplParser.term"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitTrue([NotNull] TaplParser.TrueContext context) { }
	/// <summary>
	/// Enter a parse tree produced by the <c>False</c>
	/// labeled alternative in <see cref="TaplParser.term"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterFalse([NotNull] TaplParser.FalseContext context) { }
	/// <summary>
	/// Exit a parse tree produced by the <c>False</c>
	/// labeled alternative in <see cref="TaplParser.term"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitFalse([NotNull] TaplParser.FalseContext context) { }
	/// <summary>
	/// Enter a parse tree produced by the <c>Zero</c>
	/// labeled alternative in <see cref="TaplParser.term"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterZero([NotNull] TaplParser.ZeroContext context) { }
	/// <summary>
	/// Exit a parse tree produced by the <c>Zero</c>
	/// labeled alternative in <see cref="TaplParser.term"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitZero([NotNull] TaplParser.ZeroContext context) { }
	/// <summary>
	/// Enter a parse tree produced by the <c>numericvalue</c>
	/// labeled alternative in <see cref="TaplParser.v"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterNumericvalue([NotNull] TaplParser.NumericvalueContext context) { }
	/// <summary>
	/// Exit a parse tree produced by the <c>numericvalue</c>
	/// labeled alternative in <see cref="TaplParser.v"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitNumericvalue([NotNull] TaplParser.NumericvalueContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="TaplParser.nv"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterNv([NotNull] TaplParser.NvContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="TaplParser.nv"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitNv([NotNull] TaplParser.NvContext context) { }

	/// <inheritdoc/>
	/// <remarks>The default implementation does nothing.</remarks>
	public virtual void EnterEveryRule([NotNull] ParserRuleContext context) { }
	/// <inheritdoc/>
	/// <remarks>The default implementation does nothing.</remarks>
	public virtual void ExitEveryRule([NotNull] ParserRuleContext context) { }
	/// <inheritdoc/>
	/// <remarks>The default implementation does nothing.</remarks>
	public virtual void VisitTerminal([NotNull] ITerminalNode node) { }
	/// <inheritdoc/>
	/// <remarks>The default implementation does nothing.</remarks>
	public virtual void VisitErrorNode([NotNull] IErrorNode node) { }
}
