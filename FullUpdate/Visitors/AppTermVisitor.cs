using System;
using Common;
using FullUpdate.Syntax;
using FullUpdate.Syntax.Terms;

namespace FullUpdate.Visitors;

public sealed class AppTermVisitor : FullUpdateBaseVisitor<Func<Context, ITerm>>
{
    private readonly TermVisitor _termVisitor;
    private readonly TypeVisitor _typeVisitor = new();
    private readonly PathTermVisitor _pathTermVisitor;

    public AppTermVisitor(TermVisitor termVisitor)
    {
        _termVisitor = termVisitor;
        _pathTermVisitor = new PathTermVisitor(termVisitor);
    }

    public override Func<Context, ITerm> VisitAppterm_tapp(FullUpdateParser.Appterm_tappContext context)
    {
        var info = context.GetFileInfo();
        var term = Visit(context.appterm());
        var type = _typeVisitor.Visit(context.type());
        return ctx => new TApp(info, term(ctx), type(ctx));
    }

    public override Func<Context, ITerm> VisitAppterm_app(FullUpdateParser.Appterm_appContext context)
    {
        var left = Visit(context.appterm());
        var right = _pathTermVisitor.Visit(context.pathterm());
        return ctx =>
        {
            var l = left(ctx);
            return new App(l.Info, l, right(ctx));
        };
    }

    public override Func<Context, ITerm> VisitAppterm_fix(FullUpdateParser.Appterm_fixContext context)
    {
        var info = context.GetFileInfo();
        var t = _pathTermVisitor.Visit(context.pathterm());
        return ctx => new Fix(info, t(ctx));
    }

    public override Func<Context, ITerm> VisitAppterm_times(FullUpdateParser.Appterm_timesContext context)
    {
        var info = context.GetFileInfo();
        var paths = context.pathterm();
        var t1 = _pathTermVisitor.Visit(paths[0]);
        var t2 = _pathTermVisitor.Visit(paths[1]);
        return ctx => new TimesFloat(info, t1(ctx), t2(ctx));
    }

    public override Func<Context, ITerm> VisitAppterm_succ(FullUpdateParser.Appterm_succContext context)
    {
        var info = context.GetFileInfo();
        var t = _pathTermVisitor.Visit(context.pathterm());
        return ctx => new Succ(info, t(ctx));
    }

    public override Func<Context, ITerm> VisitAppterm_pred(FullUpdateParser.Appterm_predContext context)
    {
        var info = context.GetFileInfo();
        var t = _pathTermVisitor.Visit(context.pathterm());
        return ctx => new Pred(info, t(ctx));
    }

    public override Func<Context, ITerm> VisitAppterm_iszero(FullUpdateParser.Appterm_iszeroContext context)
    {
        var info = context.GetFileInfo();
        var t = _pathTermVisitor.Visit(context.pathterm());
        return ctx => new IsZero(info, t(ctx));
    }

    public override Func<Context, ITerm> VisitAppterm_path(FullUpdateParser.Appterm_pathContext context)
        => _pathTermVisitor.Visit(context.pathterm());
}
