using System;
using Common;
using FullPoly.Syntax;

namespace FullPoly.Visitors;

public sealed class ArrowTypeVisitor(TypeVisitor typeVisitor) : FullPolyBaseVisitor<Func<Context, IType>>
{
    private readonly ATypeVisitor _aTypeVisitor = new(typeVisitor);

    public override Func<Context, IType> VisitArrowtype_arrow(FullPolyParser.Arrowtype_arrowContext context)
    {
        var atype = _aTypeVisitor.Visit(context.atype());
        var arrType = Visit(context.arrowtype());

        return ctx => new TypeArrow(atype(ctx), arrType(ctx));
    }

    public override Func<Context, IType> VisitArrowtype_atype(FullPolyParser.Arrowtype_atypeContext context)
    {
        var atype = _aTypeVisitor.Visit(context.atype());

        return ctx => atype(ctx);
    }
}