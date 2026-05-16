using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Common;
using FullUpdate.Syntax;
using FullUpdate.Syntax.Bindings;
using FullUpdate.Syntax.Terms;
using FullUpdate.Core;

namespace FullUpdate.Parser;

/// <summary>
/// Hand-rolled recursive-descent parser for fullupdate.
/// Mirrors the yacc grammar in TAPL-ocaml/fullupdate/lib/parser.mly.
/// </summary>
public sealed class Parser
{
    private readonly List<Token> _tokens;
    private int _pos;

    public Parser(List<Token> tokens) { _tokens = tokens; }

    private Token Current => _tokens[_pos];
    private Token Peek(int n = 1) => _pos + n < _tokens.Count ? _tokens[_pos + n] : _tokens[^1];
    private IInfo Info => Current.Info;

    private Token Consume()
    {
        var t = _tokens[_pos];
        _pos++;
        return t;
    }

    private Token Expect(TokenKind kind)
    {
        if (Current.Kind != kind)
            throw new Exception($"Expected {kind}, got {Current.Kind} ({Current.Text}) at {Current.Line}:{Current.Column}");
        return Consume();
    }

    private bool Check(TokenKind kind) => Current.Kind == kind;
    private bool TryConsume(TokenKind kind) { if (Check(kind)) { Consume(); return true; } return false; }

    // ---- Top-level ----
    public Func<Context, (ImmutableStack<ICommand>, Context)> ParseToplevel()
    {
        var cmds = new List<Func<Context, (ICommand, Context)>>();
        while (!Check(TokenKind.Eof))
        {
            var cmd = ParseCommand();
            Expect(TokenKind.Semi);
            cmds.Add(cmd);
        }
        return ctx =>
        {
            var stack = ImmutableStack<ICommand>.Empty;
            var commands = new List<ICommand>();
            foreach (var c in cmds)
            {
                var (cmd, ctx2) = c(ctx);
                ctx = ctx2;
                commands.Add(cmd);
            }
            foreach (var cmd in commands)
                stack = stack.Push(cmd);
            // Reverse to get original order
            var list = new List<ICommand>(commands);
            var result = ImmutableStack<ICommand>.Empty;
            for (int i = list.Count - 1; i >= 0; i--)
                result = result.Push(list[i]);
            return (result, ctx);
        };
    }

    private Func<Context, (ICommand, Context)> ParseCommand()
    {
        var info = Info;

        // {UCID, LCID} = Term  (SomeBind)
        if (Check(TokenKind.LCurly) && Peek().Kind == TokenKind.UCid)
        {
            Consume(); // {
            var tyX = Expect(TokenKind.UCid).Text;
            Expect(TokenKind.Comma);
            var x = Expect(TokenKind.LCid).Text;
            Expect(TokenKind.RCurly);
            Expect(TokenKind.Eq);
            var term = ParseTerm();
            return ctx =>
            {
                var ctx1 = ctx.AddName(tyX);
                var ctx2 = ctx1.AddName(x);
                return (new SomeBindCommand(info, tyX, x, term(ctx)), ctx2);
            };
        }

        // LCID Binder
        if (Check(TokenKind.LCid))
        {
            var name = Consume().Text;
            var binder = ParseBinder();
            return ctx => (new BindCommand(info, name, binder(ctx)), ctx.AddName(name));
        }

        // UCID TyBinder
        if (Check(TokenKind.UCid))
        {
            var name = Consume().Text;
            var binder = ParseTyBinder(name);
            return ctx => (new BindCommand(info, name, binder(ctx)), ctx.AddName(name));
        }

        // Term command
        {
            var term = ParseTerm();
            return ctx =>
            {
                var t = term(ctx);
                return (new EvalCommand(info, t), ctx);
            };
        }
    }

    private Func<Context, IBinding> ParseBinder()
    {
        if (TryConsume(TokenKind.Colon))
        {
            var ty = ParseType();
            return ctx => new VarBind(ty(ctx));
        }
        if (TryConsume(TokenKind.Eq))
        {
            var term = ParseTerm();
            return ctx => new TermAbbBind(term(ctx), null);
        }
        throw new Exception($"Expected binder, got {Current}");
    }

    // TyBinder for UCID bindings
    private Func<Context, IBinding> ParseTyBinder(string name)
    {
        // ::Kind  -> TyVarBind(maketop k)
        if (Check(TokenKind.ColonColon))
        {
            Consume();
            var kind = ParseKind();
            return ctx => new TypeVarBind(Typing.MakeTop(kind(ctx)));
        }
        // <: Type -> TyVarBind(T)
        if (Check(TokenKind.LEq))
        {
            Consume();
            var ty = ParseType();
            return ctx => new TypeVarBind(ty(ctx));
        }
        // TyAbbArgs = Type
        // We parse optional type arguments before =
        var tyAbbArgs = ParseTyAbbArgs(); // returns list of (name,kind)
        if (TryConsume(TokenKind.Eq))
        {
            var ty = ParseType();
            return ctx =>
            {
                var (binders, ctx2) = tyAbbArgs(new List<(string, IKind)>(), ctx);
                var bodyTy = ty(ctx2);
                // Wrap with TyAbs for each binder (in reverse)
                IType result = bodyTy;
                for (int i = binders.Count - 1; i >= 0; i--)
                    result = new TypeAbs(binders[i].Item1, binders[i].Item2, result);
                return new TypeAbbBind(result, null);
            };
        }
        // empty -> TyVarBind(Top)
        return _ => new TypeVarBind(new TypeTop());
    }

    // TyAbbArgs = { (UCID OKind)* }
    // returns function: (accumulated binders, current ctx) -> (binders, extended ctx)
    private Func<List<(string, IKind)>, Context, (List<(string, IKind)>, Context)> ParseTyAbbArgs()
    {
        if (Check(TokenKind.UCid))
        {
            var n = Consume().Text;
            var okind = ParseOKind();
            var rest = ParseTyAbbArgs();
            return (b, ctx) =>
            {
                var k = okind(ctx);
                var ctx1 = ctx.AddName(n);
                var newB = new List<(string, IKind)>(b) { (n, k) };
                return rest(newB, ctx1);
            };
        }
        return (b, ctx) => (b, ctx);
    }

    // ---- Kinds ----
    private Func<Context, IKind> ParseKind() => ParseArrowKind();

    private Func<Context, IKind> ParseArrowKind()
    {
        var left = ParseAKind();
        if (Check(TokenKind.DArrow))
        {
            Consume();
            var right = ParseArrowKind();
            return ctx => new KnArr(left(ctx), right(ctx));
        }
        return left;
    }

    private Func<Context, IKind> ParseAKind()
    {
        if (TryConsume(TokenKind.Star)) return _ => new KnStar();
        if (TryConsume(TokenKind.LParen))
        {
            var k = ParseKind();
            Expect(TokenKind.RParen);
            return k;
        }
        throw new Exception($"Expected kind, got {Current}");
    }

    private Func<Context, IKind> ParseOKind()
    {
        if (Check(TokenKind.ColonColon)) { Consume(); return ParseKind(); }
        return _ => new KnStar();
    }

    // ---- Types ----
    private Func<Context, IType> ParseType()
    {
        // lambda UCID OKind . Type  -> TyAbs
        if (Check(TokenKind.Lambda))
        {
            Consume();
            var tyX = Expect(TokenKind.UCid).Text;
            var okind = ParseOKind();
            Expect(TokenKind.Dot);
            var body = ParseType();
            return ctx =>
            {
                var k = okind(ctx);
                var ctx1 = ctx.AddName(tyX);
                return new TypeAbs(tyX, k, body(ctx1));
            };
        }
        // All UCID OType . Type
        if (Check(TokenKind.All))
        {
            Consume();
            var tyX = Expect(TokenKind.UCid).Text;
            var otype = ParseOType();
            Expect(TokenKind.Dot);
            var body = ParseType();
            return ctx =>
            {
                var bound = otype(ctx);
                var ctx1 = ctx.AddName(tyX);
                return new TypeAll(tyX, bound, body(ctx1));
            };
        }
        return ParseArrowType();
    }

    private Func<Context, IType> ParseOType()
    {
        if (Check(TokenKind.LEq)) { Consume(); return ParseType(); }
        if (Check(TokenKind.ColonColon)) { Consume(); var k = ParseKind(); return ctx => Typing.MakeTop(k(ctx)); }
        return _ => new TypeTop();
    }

    private Func<Context, IType> ParseArrowType()
    {
        var left = ParseAppType();
        if (Check(TokenKind.Arrow))
        {
            Consume();
            var right = ParseArrowType();
            return ctx => new TypeArrow(left(ctx), right(ctx));
        }
        return left;
    }

    private Func<Context, IType> ParseAppType()
    {
        var t = ParseAType();
        while (Check(TokenKind.UCid) || Check(TokenKind.LParen) || Check(TokenKind.Bool)
               || Check(TokenKind.UString) || Check(TokenKind.UUnit) || Check(TokenKind.LCurly)
               || Check(TokenKind.UFloat) || Check(TokenKind.Nat) || Check(TokenKind.TTop)
               || (Check(TokenKind.Some) && Peek().Kind == TokenKind.UCid))
        {
            // Check if next could start an AType
            var next = TryAType();
            if (next == null) break;
            var left = t;
            var right = next;
            t = ctx => new TypeApp(left(ctx), right(ctx));
        }
        return t;
    }

    private Func<Context, IType>? TryAType()
    {
        return Current.Kind switch
        {
            TokenKind.LParen => ParseAtypeParen(),
            TokenKind.UCid => ParseAtypeUcid(),
            TokenKind.Bool => Consume() is var _ ? _ => new TypeBool() : null,
            TokenKind.UString => Consume() is var _ ? _ => new TypeString() : null,
            TokenKind.UUnit => Consume() is var _ ? _ => new TypeUnit() : null,
            TokenKind.TTop => ParseAtypeTop(),
            TokenKind.UFloat => Consume() is var _ ? _ => new TypeFloat() : null,
            TokenKind.Nat => Consume() is var _ ? _ => new TypeNat() : null,
            TokenKind.LCurly => ParseAtypeRecord(),
            _ => null
        };
    }

    private Func<Context, IType> ParseAType()
    {
        var res = TryAType();
        if (res != null) return res;
        // {Some UCID OType, Type}
        if (Check(TokenKind.LCurly))
        {
            return ParseAtypeRecord();
        }
        throw new Exception($"Expected type, got {Current}");
    }

    private Func<Context, IType> ParseAtypeParen()
    {
        Consume(); // (
        var t = ParseType();
        Expect(TokenKind.RParen);
        return t;
    }

    private Func<Context, IType> ParseAtypeUcid()
    {
        var name = Consume().Text;
        return ctx =>
        {
            if (ctx.IsNameBound(name))
                return new TypeVar(ctx.NameToIndex(name), ctx.Length);
            return new TypeId(name);
        };
    }

    private Func<Context, IType> ParseAtypeTop()
    {
        Consume(); // Top
        // Top[Kind]
        if (TryConsume(TokenKind.LSquare))
        {
            var k = ParseKind();
            Expect(TokenKind.RSquare);
            return ctx => Typing.MakeTop(k(ctx));
        }
        return _ => new TypeTop();
    }

    private Func<Context, IType> ParseAtypeRecord()
    {
        Consume(); // {
        // {Some UCID OType, Type}
        if (Check(TokenKind.Some))
        {
            Consume();
            var tyX = Expect(TokenKind.UCid).Text;
            var otype = ParseOType();
            Expect(TokenKind.Comma);
            var body = ParseType();
            Expect(TokenKind.RCurly);
            return ctx =>
            {
                var bound = otype(ctx);
                var ctx1 = ctx.AddName(tyX);
                return new TypeSome(tyX, bound, body(ctx1));
            };
        }
        // {FieldTypes}
        var fts = ParseFieldTypes();
        Expect(TokenKind.RCurly);
        return ctx => new TypeRecord(fts(ctx, 1));
    }

    // FieldTypes
    private Func<Context, int, List<(string, Variance, IType)>> ParseFieldTypes()
    {
        if (Check(TokenKind.RCurly)) return (_, _) => new List<(string, Variance, IType)>();
        return ParseNEFieldTypes();
    }

    private Func<Context, int, List<(string, Variance, IType)>> ParseNEFieldTypes()
    {
        var ft = ParseFieldType();
        if (TryConsume(TokenKind.Comma))
        {
            var rest = ParseNEFieldTypes();
            return (ctx, i) =>
            {
                var head = ft(ctx, i);
                var tail = rest(ctx, i + 1);
                tail.Insert(0, head);
                return tail;
            };
        }
        return (ctx, i) => new List<(string, Variance, IType)> { ft(ctx, i) };
    }

    private Func<Context, int, (string, Variance, IType)> ParseFieldType()
    {
        var variance = ParseVariance();
        if (Check(TokenKind.LCid) && Peek().Kind == TokenKind.Colon)
        {
            var name = Consume().Text;
            Consume(); // :
            var ty = ParseType();
            return (ctx, _) => (name, variance, ty(ctx));
        }
        // else positional
        {
            var ty = ParseType();
            return (ctx, i) => (i.ToString(), variance, ty(ctx));
        }
    }

    // ---- Terms ----
    private Func<Context, ITerm> ParseTerm()
    {
        var info = Info;

        if (Check(TokenKind.Lambda))
        {
            Consume();
            // lambda UCID OType . Term  -> TmTAbs
            if (Check(TokenKind.UCid))
            {
                var tyX = Consume().Text;
                var otype = ParseOType();
                Expect(TokenKind.Dot);
                var body = ParseTerm();
                return ctx =>
                {
                    var bound = otype(ctx);
                    var ctx1 = ctx.AddName(tyX);
                    return new TAbs(info, tyX, bound, body(ctx1));
                };
            }
            // lambda LCID : Type . Term  -> Abs
            if (Check(TokenKind.LCid) || Check(TokenKind.Uscore))
            {
                string v = Check(TokenKind.Uscore) ? "_" : Current.Text;
                Consume();
                Expect(TokenKind.Colon);
                var ty = ParseType();
                Expect(TokenKind.Dot);
                var body = ParseTerm();
                return ctx =>
                {
                    var ctx1 = ctx.AddName(v);
                    return new Abs(info, v, ty(ctx), body(ctx1));
                };
            }
            throw new Exception($"Expected identifier after lambda, got {Current}");
        }

        if (Check(TokenKind.Let))
        {
            Consume();
            // let {UCID, LCID} = Term in Term  -> Unpack
            if (Check(TokenKind.LCurly))
            {
                Consume();
                var tyX = Expect(TokenKind.UCid).Text;
                Expect(TokenKind.Comma);
                var x = Expect(TokenKind.LCid).Text;
                Expect(TokenKind.RCurly);
                Expect(TokenKind.Eq);
                var t1 = ParseTerm();
                Expect(TokenKind.In);
                var t2 = ParseTerm();
                return ctx =>
                {
                    var ctx1 = ctx.AddName(tyX);
                    var ctx2 = ctx1.AddName(x);
                    return new Unpack(info, tyX, x, t1(ctx), t2(ctx2));
                };
            }
            // let LCID = Term in Term
            if (Check(TokenKind.LCid) || Check(TokenKind.Uscore))
            {
                string v = Check(TokenKind.Uscore) ? "_" : Current.Text;
                Consume();
                Expect(TokenKind.Eq);
                var t1 = ParseTerm();
                Expect(TokenKind.In);
                var t2 = ParseTerm();
                return ctx =>
                {
                    var ctx1 = ctx.AddName(v);
                    return new Let(info, v, t1(ctx), t2(ctx1));
                };
            }
            throw new Exception($"Unexpected token in let: {Current}");
        }

        if (Check(TokenKind.Letrec))
        {
            Consume();
            var v = Expect(TokenKind.LCid).Text;
            Expect(TokenKind.Colon);
            var ty = ParseType();
            Expect(TokenKind.Eq);
            var t1 = ParseTerm();
            Expect(TokenKind.In);
            var t2 = ParseTerm();
            return ctx =>
            {
                var ctx1 = ctx.AddName(v);
                var abs = new Abs(info, v, ty(ctx), t1(ctx1));
                var fix = new Fix(info, abs);
                return new Let(info, v, fix, t2(ctx1));
            };
        }

        if (Check(TokenKind.If))
        {
            Consume();
            var cond = ParseTerm();
            Expect(TokenKind.Then);
            var then = ParseTerm();
            Expect(TokenKind.Else);
            var els = ParseTerm();
            return ctx => new If(info, cond(ctx), then(ctx), els(ctx));
        }

        // AppTerm LEFTARROW LCID EQ Term  (update)
        var appTerm = ParseAppTerm();
        if (Check(TokenKind.LeftArrow))
        {
            Consume();
            var label = Expect(TokenKind.LCid).Text;
            Expect(TokenKind.Eq);
            var newVal = ParseTerm();
            return ctx => new Update(info, appTerm(ctx), label, newVal(ctx));
        }
        return appTerm;
    }

    private Func<Context, ITerm> ParseAppTerm()
    {
        var t = ParsePathTerm();

        while (true)
        {
            var info = Info;
            if (Check(TokenKind.LSquare))
            {
                // TApp
                Consume();
                var ty = ParseType();
                Expect(TokenKind.RSquare);
                var left = t;
                t = ctx => new TApp(info, left(ctx), ty(ctx));
                continue;
            }
            if (CanStartPathTerm())
            {
                var left = t;
                var right = ParsePathTerm();
                t = ctx =>
                {
                    var l = left(ctx);
                    return new App(l.Info, l, right(ctx));
                };
                continue;
            }
            break;
        }
        return t;
    }

    private bool CanStartPathTerm()
    {
        return Current.Kind switch
        {
            TokenKind.LParen => true,
            TokenKind.LCid => true,
            TokenKind.UCid => false, // would be a type name
            TokenKind.StringV => true,
            TokenKind.LCurly => true,
            TokenKind.True => true,
            TokenKind.False => true,
            TokenKind.IntV => true,
            TokenKind.FloatV => true,
            TokenKind.Unit => true,
            TokenKind.Inert => true,
            TokenKind.Succ => true,
            TokenKind.Pred => true,
            TokenKind.IsZero => true,
            TokenKind.TimesFlt => true,
            TokenKind.Fix => true,
            _ => false
        };
    }

    private Func<Context, ITerm> ParsePathTerm()
    {
        var t = ParseAscribeTerm();
        while (Check(TokenKind.Dot))
        {
            Consume();
            var info = Info;
            string label;
            if (Check(TokenKind.LCid)) label = Consume().Text;
            else if (Check(TokenKind.IntV)) label = Consume().Text;
            else throw new Exception($"Expected label after '.', got {Current}");
            var left = t;
            t = ctx => new Proj(info, left(ctx), label);
        }
        return t;
    }

    private Func<Context, ITerm> ParseAscribeTerm()
    {
        var t = ParseATerm();
        if (Check(TokenKind.As))
        {
            var info = Info;
            Consume();
            var ty = ParseType();
            return ctx => new Ascribe(info, t(ctx), ty(ctx));
        }
        return t;
    }

    private Func<Context, ITerm> ParseATerm()
    {
        var info = Info;

        if (Check(TokenKind.LParen))
        {
            Consume();
            var t = ParseTermSeq();
            Expect(TokenKind.RParen);
            return t;
        }

        if (Check(TokenKind.LCid))
        {
            var name = Consume().Text;
            return ctx => new Var(info, ctx.NameToIndex(name), ctx.Length);
        }

        if (Check(TokenKind.StringV))
        {
            var val = Consume().Text;
            return _ => new StringTerm(info, val);
        }

        // {* Type, Term} as Type  (Pack)
        if (Check(TokenKind.LCurly) && Peek().Kind == TokenKind.Star)
        {
            Consume(); // {
            Consume(); // *
            var ty1 = ParseType();
            Expect(TokenKind.Comma);
            var t = ParseTerm();
            Expect(TokenKind.RCurly);
            Expect(TokenKind.As);
            var ty3 = ParseType();
            return ctx => new Pack(info, ty1(ctx), t(ctx), ty3(ctx));
        }

        // {Fields}  (Record)
        if (Check(TokenKind.LCurly))
        {
            Consume();
            var fields = ParseFields();
            Expect(TokenKind.RCurly);
            return ctx => new Record(info, fields(ctx, 1));
        }

        if (Check(TokenKind.True)) { Consume(); return _ => new True(info); }
        if (Check(TokenKind.False)) { Consume(); return _ => new False(info); }

        if (Check(TokenKind.IntV))
        {
            var n = int.Parse(Consume().Text);
            return _ => { ITerm t = new Zero(info); for (int i = 0; i < n; i++) t = new Succ(info, t); return t; };
        }

        if (Check(TokenKind.FloatV))
        {
            var v = double.Parse(Consume().Text, System.Globalization.CultureInfo.InvariantCulture);
            return _ => new Float(info, v);
        }

        if (Check(TokenKind.Unit)) { Consume(); return _ => new Unit(info); }

        if (Check(TokenKind.Inert))
        {
            Consume();
            Expect(TokenKind.LSquare);
            var ty = ParseType();
            Expect(TokenKind.RSquare);
            return ctx => new Inert(info, ty(ctx));
        }

        if (Check(TokenKind.Succ))
        {
            Consume();
            var t = ParsePathTerm();
            return ctx => new Succ(info, t(ctx));
        }

        if (Check(TokenKind.Pred))
        {
            Consume();
            var t = ParsePathTerm();
            return ctx => new Pred(info, t(ctx));
        }

        if (Check(TokenKind.IsZero))
        {
            Consume();
            var t = ParsePathTerm();
            return ctx => new IsZero(info, t(ctx));
        }

        if (Check(TokenKind.TimesFlt))
        {
            Consume();
            var t1 = ParsePathTerm();
            var t2 = ParsePathTerm();
            return ctx => new TimesFloat(info, t1(ctx), t2(ctx));
        }

        if (Check(TokenKind.Fix))
        {
            Consume();
            var t = ParsePathTerm();
            return ctx => new Fix(info, t(ctx));
        }

        throw new Exception($"Unexpected token in term: {Current}");
    }

    private Func<Context, ITerm> ParseTermSeq()
    {
        var t = ParseTerm();
        if (Check(TokenKind.Semi))
        {
            var info = Info;
            Consume();
            var rest = ParseTermSeq();
            // TmApp(unit-abs, t)
            return ctx =>
            {
                var ctx1 = ctx.AddName("_");
                var abs = new Abs(info, "_", new TypeUnit(), rest(ctx1));
                return new App(info, abs, t(ctx));
            };
        }
        return t;
    }

    // Fields for records: { Field, ... }
    private Func<Context, int, List<(string, Variance, ITerm)>> ParseFields()
    {
        if (Check(TokenKind.RCurly)) return (_, _) => new List<(string, Variance, ITerm)>();
        return ParseNEFields();
    }

    private Func<Context, int, List<(string, Variance, ITerm)>> ParseNEFields()
    {
        var f = ParseField();
        if (TryConsume(TokenKind.Comma))
        {
            var rest = ParseNEFields();
            return (ctx, i) =>
            {
                var head = f(ctx, i);
                var tail = rest(ctx, i + 1);
                tail.Insert(0, head);
                return tail;
            };
        }
        return (ctx, i) => new List<(string, Variance, ITerm)> { f(ctx, i) };
    }

    private Func<Context, int, (string, Variance, ITerm)> ParseField()
    {
        var variance = ParseVariance();
        if (Check(TokenKind.LCid) && Peek().Kind == TokenKind.Eq)
        {
            var name = Consume().Text;
            Consume(); // =
            var t = ParseTerm();
            return (ctx, _) => (name, variance, t(ctx));
        }
        // Positional field
        {
            var t = ParseTerm();
            return (ctx, i) => (i.ToString(), variance, t(ctx));
        }
    }

    private Variance ParseVariance()
    {
        if (Check(TokenKind.Hash)) { Consume(); return Variance.Invariant; }
        return Variance.Covariant;
    }
}
