using System;
using Common;
using FullRef.Syntax;

namespace FullRef.Visitors;

public sealed class ArrowTypeVisitor(TypeVisitor typeVisitor) : FullRefBaseVisitor<Func<Context, IType>>
{
    private readonly ATypeVisitor _aTypeVisitor = new(typeVisitor);

    public override Func<Context, IType> VisitArrowtype_arrow(FullRefParser.Arrowtype_arrowContext context)
    {
        var atype = _aTypeVisitor.Visit(context.atype());
        var arrType = Visit(context.arrowtype());

        return ctx => new TypeArrow(atype(ctx), arrType(ctx));
    }

    public override Func<Context, IType> VisitArrowtype_atype(FullRefParser.Arrowtype_atypeContext context)
    {
        var atype = _aTypeVisitor.Visit(context.atype());

        return ctx => atype(ctx);
    }
}