using System;
using Common;
using FullUpdate.Syntax.Terms;

namespace FullUpdate.Visitors;

public sealed class PathTermVisitor : FullUpdateBaseVisitor<Func<Context, ITerm>>
{
    private readonly AscribeTermVisitor _ascribeTermVisitor;

    public PathTermVisitor(TermVisitor termVisitor)
    {
        _ascribeTermVisitor = new AscribeTermVisitor(termVisitor);
    }

    public override Func<Context, ITerm> VisitPathterm_lcid(FullUpdateParser.Pathterm_lcidContext context)
    {
        var info = context.GetFileInfo();
        var term = Visit(context.pathterm());
        var label = context.LCID().GetText();
        return ctx => new Proj(info, term(ctx), label);
    }

    public override Func<Context, ITerm> VisitPathterm_intv(FullUpdateParser.Pathterm_intvContext context)
    {
        var info = context.GetFileInfo();
        var term = Visit(context.pathterm());
        var label = context.INTV().GetText();
        return ctx => new Proj(info, term(ctx), label);
    }

    public override Func<Context, ITerm> VisitPathterm_asterm(FullUpdateParser.Pathterm_astermContext context)
        => _ascribeTermVisitor.Visit(context.ascribeterm());
}
