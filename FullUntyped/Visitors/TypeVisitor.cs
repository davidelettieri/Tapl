using System;
using Common;

namespace FullUntyped.Visitors;

public sealed class TypeVisitor : FullUntypedBaseVisitor<Func<Context, IType>>
{
    private readonly ArrowTypeVisitor _arrowTypeVisitor;

    public TypeVisitor()
    {
        _arrowTypeVisitor = new ArrowTypeVisitor(this);
    }

    public override Func<Context, IType> VisitType_arrowtype(FullUntypedParser.Type_arrowtypeContext context) => _arrowTypeVisitor.Visit(context.arrowtype());
}