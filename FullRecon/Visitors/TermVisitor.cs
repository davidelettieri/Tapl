using System;
using Common;
using FullRecon.Syntax.Terms;

namespace FullRecon.Visitors;

public sealed class TermVisitor : FullReconBaseVisitor<Func<Context, ITerm>>
{
    private readonly TypeVisitor _typeVisitor = new();

    public override Func<Context, ITerm> VisitTerm_appterm(FullReconParser.Term_apptermContext context)
        => Visit(context.appterm());

    public override Func<Context, ITerm> VisitTerm_ift(FullReconParser.Term_iftContext context)
    {
        var terms = context.term();
        var condition = Visit(terms[0]);
        var then = Visit(terms[1]);
        var @else = Visit(terms[2]);
        var info = context.GetFileInfo();
        return ctx => new If(info, condition(ctx), then(ctx), @else(ctx));
    }

    public override Func<Context, ITerm> VisitTerm_ll(FullReconParser.Term_llContext context)
    {
        var info = context.GetFileInfo();
        var v = context.LCID().GetText();
        var terms = context.term();
        var letTerm = Visit(terms[0]);
        var inTerm = Visit(terms[1]);
        return ctx => new Let(info, v, letTerm(ctx), inTerm(ctx.AddName(v)));
    }

    public override Func<Context, ITerm> VisitTerm_lu(FullReconParser.Term_luContext context)
    {
        var info = context.GetFileInfo();
        var terms = context.term();
        var letTerm = Visit(terms[0]);
        var inTerm = Visit(terms[1]);
        return ctx => new Let(info, "_", letTerm(ctx), inTerm(ctx.AddName("_")));
    }

    public override Func<Context, ITerm> VisitTerm_lambda_untyped(
        FullReconParser.Term_lambda_untypedContext context)
    {
        var info = context.GetFileInfo();
        var v = context.LCID().GetText();
        var body = Visit(context.term());
        return ctx =>
        {
            var ctx1 = ctx.AddName(v);
            return new Abs(info, v, null, body(ctx1));
        };
    }

    public override Func<Context, ITerm> VisitTerm_lambda_typed(
        FullReconParser.Term_lambda_typedContext context)
    {
        var info = context.GetFileInfo();
        var v = context.LCID().GetText();
        var type = _typeVisitor.Visit(context.type());
        var body = Visit(context.term());
        return ctx =>
        {
            var ctx1 = ctx.AddName(v);
            return new Abs(info, v, type(ctx), body(ctx1));
        };
    }

    public override Func<Context, ITerm> VisitTerm_lambda_uscore(
        FullReconParser.Term_lambda_uscoreContext context)
    {
        var info = context.GetFileInfo();
        var type = _typeVisitor.Visit(context.type());
        var body = Visit(context.term());
        return ctx =>
        {
            var ctx1 = ctx.AddName("_");
            return new Abs(info, "_", type(ctx), body(ctx1));
        };
    }

    public override Func<Context, ITerm> VisitAppterm_aterm(FullReconParser.Appterm_atermContext context)
        => Visit(context.aterm());

    public override Func<Context, ITerm> VisitAppterm_app(FullReconParser.Appterm_appContext context)
    {
        var t1 = Visit(context.appterm());
        var t2 = Visit(context.aterm());
        return ctx =>
        {
            var e1 = t1(ctx);
            var e2 = t2(ctx);
            return new App(new UnknownInfo(), e1, e2);
        };
    }

    public override Func<Context, ITerm> VisitAppterm_succ(FullReconParser.Appterm_succContext context)
    {
        var info = context.GetFileInfo();
        var t = Visit(context.aterm());
        return ctx => new Succ(info, t(ctx));
    }

    public override Func<Context, ITerm> VisitAppterm_pred(FullReconParser.Appterm_predContext context)
    {
        var info = context.GetFileInfo();
        var t = Visit(context.aterm());
        return ctx => new Pred(info, t(ctx));
    }

    public override Func<Context, ITerm> VisitAppterm_iszero(FullReconParser.Appterm_iszeroContext context)
    {
        var info = context.GetFileInfo();
        var t = Visit(context.aterm());
        return ctx => new IsZero(info, t(ctx));
    }

    public override Func<Context, ITerm> VisitAterm_paren(FullReconParser.Aterm_parenContext context)
        => Visit(context.term());

    public override Func<Context, ITerm> VisitAterm_lcid(FullReconParser.Aterm_lcidContext context)
    {
        var info = context.GetFileInfo();
        var name = context.LCID().GetText();
        return ctx => new Var(info, ctx.NameToIndex(name), ctx.Length);
    }

    public override Func<Context, ITerm> VisitAterm_true(FullReconParser.Aterm_trueContext context)
    {
        var info = context.GetFileInfo();
        return _ => new True(info);
    }

    public override Func<Context, ITerm> VisitAterm_false(FullReconParser.Aterm_falseContext context)
    {
        var info = context.GetFileInfo();
        return _ => new False(info);
    }

    public override Func<Context, ITerm> VisitAterm_intv(FullReconParser.Aterm_intvContext context)
    {
        var info = context.GetFileInfo();
        var value = int.Parse(context.INTV().GetText());
        return _ => BuildNat(info, value);
    }

    private static ITerm BuildNat(IInfo info, int n)
    {
        if (n == 0)
            return new Zero(info);
        return new Succ(info, BuildNat(info, n - 1));
    }
}
