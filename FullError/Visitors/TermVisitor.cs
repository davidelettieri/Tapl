using System;
using Common;
using FullError.Syntax.Terms;

namespace FullError.Visitors;

public sealed class TermVisitor : FullErrorBaseVisitor<Func<Context, ITerm>>
{
    private readonly TypeVisitor _typeVisitor = new();

    public override Func<Context, ITerm> VisitTerm_appterm(FullErrorParser.Term_apptermContext context)
        => Visit(context.appterm());

    public override Func<Context, ITerm> VisitTerm_ift(FullErrorParser.Term_iftContext context)
    {
        var terms = context.term();
        var condition = Visit(terms[0]);
        var then = Visit(terms[1]);
        var @else = Visit(terms[2]);
        var info = context.GetFileInfo();
        return c => new If(info, condition(c), then(c), @else(c));
    }

    public override Func<Context, ITerm> VisitTerm_llcid(FullErrorParser.Term_llcidContext context)
    {
        var v = context.LCID().GetText();
        var info = context.GetFileInfo();
        var type = _typeVisitor.Visit(context.type());
        var term = Visit(context.term());
        return ctx =>
        {
            var ctx1 = ctx.AddName(v);
            return new Abs(info, v, type(ctx), term(ctx1));
        };
    }

    public override Func<Context, ITerm> VisitTerm_luc(FullErrorParser.Term_lucContext context)
    {
        var info = context.GetFileInfo();
        var type = _typeVisitor.Visit(context.type());
        var term = Visit(context.term());
        return ctx =>
        {
            var ctx1 = ctx.AddName("_");
            return new Abs(info, "_", type(ctx), term(ctx1));
        };
    }

    public override Func<Context, ITerm> VisitTerm_try(FullErrorParser.Term_tryContext context)
    {
        var info = context.GetFileInfo();
        var terms = context.term();
        var t1 = Visit(terms[0]);
        var t2 = Visit(terms[1]);
        return ctx => new Try(info, t1(ctx), t2(ctx));
    }

    public override Func<Context, ITerm> VisitAppterm_aterm(FullErrorParser.Appterm_atermContext context)
        => Visit(context.aterm());

    public override Func<Context, ITerm> VisitAppterm_app(FullErrorParser.Appterm_appContext context)
    {
        var t1 = Visit(context.appterm());
        var t2 = Visit(context.aterm());
        return ctx =>
        {
            var e1 = t1(ctx);
            var e2 = t2(ctx);
            return new App(new Common.UnknownInfo(), e1, e2);
        };
    }

    public override Func<Context, ITerm> VisitAterm_paren(FullErrorParser.Aterm_parenContext context)
        => Visit(context.term());

    public override Func<Context, ITerm> VisitAterm_lcid(FullErrorParser.Aterm_lcidContext context)
    {
        var info = context.GetFileInfo();
        var name = context.LCID().GetText();
        return ctx => new Var(info, ctx.NameToIndex(name), ctx.Length);
    }

    public override Func<Context, ITerm> VisitAterm_true(FullErrorParser.Aterm_trueContext context)
    {
        var info = context.GetFileInfo();
        return _ => new True(info);
    }

    public override Func<Context, ITerm> VisitAterm_false(FullErrorParser.Aterm_falseContext context)
    {
        var info = context.GetFileInfo();
        return _ => new False(info);
    }

    public override Func<Context, ITerm> VisitAterm_error(FullErrorParser.Aterm_errorContext context)
    {
        var info = context.GetFileInfo();
        return _ => new Error(info);
    }
}
