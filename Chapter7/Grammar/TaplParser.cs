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

using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;
using Antlr4.Runtime;
using Antlr4.Runtime.Atn;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using DFA = Antlr4.Runtime.Dfa.DFA;

[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.8")]
[System.CLSCompliant(false)]
public partial class TaplParser : Parser {
	protected static DFA[] decisionToDFA;
	protected static PredictionContextCache sharedContextCache = new PredictionContextCache();
	public const int
		T__0=1, T__1=2, VAR=3, LAMBDA=4, DOT=5;
	public const int
		RULE_term = 0;
	public static readonly string[] ruleNames = {
		"term"
	};

	private static readonly string[] _LiteralNames = {
		null, "'('", "')'", null, "'\\'", "'.'"
	};
	private static readonly string[] _SymbolicNames = {
		null, null, null, "VAR", "LAMBDA", "DOT"
	};
	public static readonly IVocabulary DefaultVocabulary = new Vocabulary(_LiteralNames, _SymbolicNames);

	[NotNull]
	public override IVocabulary Vocabulary
	{
		get
		{
			return DefaultVocabulary;
		}
	}

	public override string GrammarFileName { get { return "Tapl.g4"; } }

	public override string[] RuleNames { get { return ruleNames; } }

	public override string SerializedAtn { get { return new string(_serializedATN); } }

	static TaplParser() {
		decisionToDFA = new DFA[_ATN.NumberOfDecisions];
		for (int i = 0; i < _ATN.NumberOfDecisions; i++) {
			decisionToDFA[i] = new DFA(_ATN.GetDecisionState(i), i);
		}
	}

		public TaplParser(ITokenStream input) : this(input, Console.Out, Console.Error) { }

		public TaplParser(ITokenStream input, TextWriter output, TextWriter errorOutput)
		: base(input, output, errorOutput)
	{
		Interpreter = new ParserATNSimulator(this, _ATN, decisionToDFA, sharedContextCache);
	}

	public partial class TermContext : ParserRuleContext {
		public TermContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_term; } }
	 
		public TermContext() { }
		public virtual void CopyFrom(TermContext context) {
			base.CopyFrom(context);
		}
	}
	public partial class ParContext : TermContext {
		public TermContext term() {
			return GetRuleContext<TermContext>(0);
		}
		public ParContext(TermContext context) { CopyFrom(context); }
		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor) {
			ITaplVisitor<TResult> typedVisitor = visitor as ITaplVisitor<TResult>;
			if (typedVisitor != null) return typedVisitor.VisitPar(this);
			else return visitor.VisitChildren(this);
		}
	}
	public partial class AppContext : TermContext {
		public TermContext[] term() {
			return GetRuleContexts<TermContext>();
		}
		public TermContext term(int i) {
			return GetRuleContext<TermContext>(i);
		}
		public AppContext(TermContext context) { CopyFrom(context); }
		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor) {
			ITaplVisitor<TResult> typedVisitor = visitor as ITaplVisitor<TResult>;
			if (typedVisitor != null) return typedVisitor.VisitApp(this);
			else return visitor.VisitChildren(this);
		}
	}
	public partial class AbsContext : TermContext {
		public ITerminalNode LAMBDA() { return GetToken(TaplParser.LAMBDA, 0); }
		public ITerminalNode VAR() { return GetToken(TaplParser.VAR, 0); }
		public ITerminalNode DOT() { return GetToken(TaplParser.DOT, 0); }
		public TermContext term() {
			return GetRuleContext<TermContext>(0);
		}
		public AbsContext(TermContext context) { CopyFrom(context); }
		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor) {
			ITaplVisitor<TResult> typedVisitor = visitor as ITaplVisitor<TResult>;
			if (typedVisitor != null) return typedVisitor.VisitAbs(this);
			else return visitor.VisitChildren(this);
		}
	}
	public partial class VarContext : TermContext {
		public ITerminalNode VAR() { return GetToken(TaplParser.VAR, 0); }
		public VarContext(TermContext context) { CopyFrom(context); }
		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor) {
			ITaplVisitor<TResult> typedVisitor = visitor as ITaplVisitor<TResult>;
			if (typedVisitor != null) return typedVisitor.VisitVar(this);
			else return visitor.VisitChildren(this);
		}
	}

	[RuleVersion(0)]
	public TermContext term() {
		return term(0);
	}

	private TermContext term(int _p) {
		ParserRuleContext _parentctx = Context;
		int _parentState = State;
		TermContext _localctx = new TermContext(Context, _parentState);
		TermContext _prevctx = _localctx;
		int _startState = 0;
		EnterRecursionRule(_localctx, 0, RULE_term, _p);
		try {
			int _alt;
			EnterOuterAlt(_localctx, 1);
			{
			State = 12;
			ErrorHandler.Sync(this);
			switch (TokenStream.LA(1)) {
			case T__0:
				{
				_localctx = new ParContext(_localctx);
				Context = _localctx;
				_prevctx = _localctx;

				State = 3; Match(T__0);
				State = 4; term(0);
				State = 5; Match(T__1);
				}
				break;
			case VAR:
				{
				_localctx = new VarContext(_localctx);
				Context = _localctx;
				_prevctx = _localctx;
				State = 7; Match(VAR);
				}
				break;
			case LAMBDA:
				{
				_localctx = new AbsContext(_localctx);
				Context = _localctx;
				_prevctx = _localctx;
				State = 8; Match(LAMBDA);
				State = 9; Match(VAR);
				State = 10; Match(DOT);
				State = 11; term(2);
				}
				break;
			default:
				throw new NoViableAltException(this);
			}
			Context.Stop = TokenStream.LT(-1);
			State = 18;
			ErrorHandler.Sync(this);
			_alt = Interpreter.AdaptivePredict(TokenStream,1,Context);
			while ( _alt!=2 && _alt!=global::Antlr4.Runtime.Atn.ATN.INVALID_ALT_NUMBER ) {
				if ( _alt==1 ) {
					if ( ParseListeners!=null )
						TriggerExitRuleEvent();
					_prevctx = _localctx;
					{
					{
					_localctx = new AppContext(new TermContext(_parentctx, _parentState));
					PushNewRecursionContext(_localctx, _startState, RULE_term);
					State = 14;
					if (!(Precpred(Context, 1))) throw new FailedPredicateException(this, "Precpred(Context, 1)");
					State = 15; term(2);
					}
					} 
				}
				State = 20;
				ErrorHandler.Sync(this);
				_alt = Interpreter.AdaptivePredict(TokenStream,1,Context);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			ErrorHandler.ReportError(this, re);
			ErrorHandler.Recover(this, re);
		}
		finally {
			UnrollRecursionContexts(_parentctx);
		}
		return _localctx;
	}

	public override bool Sempred(RuleContext _localctx, int ruleIndex, int predIndex) {
		switch (ruleIndex) {
		case 0: return term_sempred((TermContext)_localctx, predIndex);
		}
		return true;
	}
	private bool term_sempred(TermContext _localctx, int predIndex) {
		switch (predIndex) {
		case 0: return Precpred(Context, 1);
		}
		return true;
	}

	private static char[] _serializedATN = {
		'\x3', '\x608B', '\xA72A', '\x8133', '\xB9ED', '\x417C', '\x3BE7', '\x7786', 
		'\x5964', '\x3', '\a', '\x18', '\x4', '\x2', '\t', '\x2', '\x3', '\x2', 
		'\x3', '\x2', '\x3', '\x2', '\x3', '\x2', '\x3', '\x2', '\x3', '\x2', 
		'\x3', '\x2', '\x3', '\x2', '\x3', '\x2', '\x3', '\x2', '\x5', '\x2', 
		'\xF', '\n', '\x2', '\x3', '\x2', '\x3', '\x2', '\a', '\x2', '\x13', '\n', 
		'\x2', '\f', '\x2', '\xE', '\x2', '\x16', '\v', '\x2', '\x3', '\x2', '\x2', 
		'\x3', '\x2', '\x3', '\x2', '\x2', '\x2', '\x2', '\x19', '\x2', '\xE', 
		'\x3', '\x2', '\x2', '\x2', '\x4', '\x5', '\b', '\x2', '\x1', '\x2', '\x5', 
		'\x6', '\a', '\x3', '\x2', '\x2', '\x6', '\a', '\x5', '\x2', '\x2', '\x2', 
		'\a', '\b', '\a', '\x4', '\x2', '\x2', '\b', '\xF', '\x3', '\x2', '\x2', 
		'\x2', '\t', '\xF', '\a', '\x5', '\x2', '\x2', '\n', '\v', '\a', '\x6', 
		'\x2', '\x2', '\v', '\f', '\a', '\x5', '\x2', '\x2', '\f', '\r', '\a', 
		'\a', '\x2', '\x2', '\r', '\xF', '\x5', '\x2', '\x2', '\x4', '\xE', '\x4', 
		'\x3', '\x2', '\x2', '\x2', '\xE', '\t', '\x3', '\x2', '\x2', '\x2', '\xE', 
		'\n', '\x3', '\x2', '\x2', '\x2', '\xF', '\x14', '\x3', '\x2', '\x2', 
		'\x2', '\x10', '\x11', '\f', '\x3', '\x2', '\x2', '\x11', '\x13', '\x5', 
		'\x2', '\x2', '\x4', '\x12', '\x10', '\x3', '\x2', '\x2', '\x2', '\x13', 
		'\x16', '\x3', '\x2', '\x2', '\x2', '\x14', '\x12', '\x3', '\x2', '\x2', 
		'\x2', '\x14', '\x15', '\x3', '\x2', '\x2', '\x2', '\x15', '\x3', '\x3', 
		'\x2', '\x2', '\x2', '\x16', '\x14', '\x3', '\x2', '\x2', '\x2', '\x4', 
		'\xE', '\x14',
	};

	public static readonly ATN _ATN =
		new ATNDeserializer().Deserialize(_serializedATN);


}
