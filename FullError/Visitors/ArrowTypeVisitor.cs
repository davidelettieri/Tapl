using System;
using Common;
using FullError.Syntax;

namespace FullError.Visitors;

public sealed class ArrowTypeVisitor(TypeVisitor typeVisitor) : FullErrorBaseVisitor<Func<Context, IType>>
{
    private readonly ATypeVisitor _aTypeVisitor = new(typeVisitor);

    public override Func<Context, IType> VisitArrowtype_arrow(FullErrorParser.Arrowtype_arrowContext context)
    {
        var atype = _aTypeVisitor.Visit(context.atype());
        var arrType = Visit(context.arrowtype());

        return ctx => new TypeArrow(atype(ctx), arrType(ctx));
    }

    public override Func<Context, IType> VisitArrowtype_atype(FullErrorParser.Arrowtype_atypeContext context)
    {
        var atype = _aTypeVisitor.Visit(context.atype());

        return ctx => atype(ctx);
    }
}
