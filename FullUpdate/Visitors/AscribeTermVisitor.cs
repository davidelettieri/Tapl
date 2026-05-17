using System;
using Common;
using FullUpdate.Syntax;
using FullUpdate.Syntax.Terms;

namespace FullUpdate.Visitors;

public sealed class AscribeTermVisitor : FullUpdateBaseVisitor<Func<Context, ITerm>>
{
    private readonly ATermVisitor _aTermVisitor;
    private readonly TypeVisitor _typeVisitor = new();

    public AscribeTermVisitor(TermVisitor termVisitor)
    {
        _aTermVisitor = new ATermVisitor(termVisitor);
    }

    public override Func<Context, ITerm> VisitAscribeterm_aaa(FullUpdateParser.Ascribeterm_aaaContext context)
    {
        var info = context.GetFileInfo();
        var t = _aTermVisitor.Visit(context.aterm());
        var ty = _typeVisitor.Visit(context.type());
        return ctx => new Ascribe(info, t(ctx), ty(ctx));
    }

    public override Func<Context, ITerm> VisitAscribeterm_a(FullUpdateParser.Ascribeterm_aContext context)
        => _aTermVisitor.Visit(context.aterm());
}
