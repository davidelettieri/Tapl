using System;
using Common;
using FullRecon.Syntax;

namespace FullRecon.Visitors;

public sealed class TypeVisitor : FullReconBaseVisitor<Func<Context, IType>>
{
    private readonly ArrowTypeVisitor _arrowTypeVisitor;

    public TypeVisitor()
    {
        _arrowTypeVisitor = new ArrowTypeVisitor(this);
    }

    public override Func<Context, IType> VisitType_arrowtype(FullReconParser.Type_arrowtypeContext context)
        => _arrowTypeVisitor.Visit(context.arrowtype());
}
