using System;
using Common;
using FullError.Syntax;

namespace FullError.Visitors;

public sealed class TypeVisitor : FullErrorBaseVisitor<Func<Context, IType>>
{
    private readonly ArrowTypeVisitor _arrowTypeVisitor;

    public TypeVisitor()
    {
        _arrowTypeVisitor = new ArrowTypeVisitor(this);
    }

    public override Func<Context, IType> VisitType_arrowtype(FullErrorParser.Type_arrowtypeContext context)
        => _arrowTypeVisitor.Visit(context.arrowtype());
}
