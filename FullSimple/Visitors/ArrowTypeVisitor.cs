using System;
using Common;
using FullSimple.Syntax;

namespace FullSimple.Visitors;

public sealed class ArrowTypeVisitor(TypeVisitor typeVisitor) : FullSimpleBaseVisitor<Func<Context, IType>>
{
    private readonly ATypeVisitor _aTypeVisitor = new(typeVisitor);

    public override Func<Context, IType> VisitArrowtype_arrow(FullSimpleParser.Arrowtype_arrowContext context)
    {
        var atype = _aTypeVisitor.Visit(context.atype());
        var arrType = Visit(context.arrowtype());

        return ctx => new TypeArrow(atype(ctx), arrType(ctx));
    }

    public override Func<Context, IType> VisitArrowtype_atype(FullSimpleParser.Arrowtype_atypeContext context)
    {
        var atype = _aTypeVisitor.Visit(context.atype());

        return ctx => atype(ctx);
    }
}