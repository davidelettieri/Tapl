using System;
using Common;
using FullUpdate.Syntax;

namespace FullUpdate.Visitors;

public sealed class ArrowTypeVisitor(TypeVisitor typeVisitor) : FullUpdateBaseVisitor<Func<Context, IType>>
{
    private readonly AppTypeVisitor _appTypeVisitor = new(typeVisitor);

    public override Func<Context, IType> VisitArrowtype_arrow(FullUpdateParser.Arrowtype_arrowContext context)
    {
        var left = _appTypeVisitor.Visit(context.apptype());
        var right = Visit(context.arrowtype());
        return ctx => new TypeArrow(left(ctx), right(ctx));
    }

    public override Func<Context, IType> VisitArrowtype_atype(FullUpdateParser.Arrowtype_atypeContext context)
        => _appTypeVisitor.Visit(context.apptype());
}
