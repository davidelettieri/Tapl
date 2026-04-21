using Antlr4.Runtime.Misc;
using Common;
using System;

namespace FullSimple.Visitors;

public sealed class TypeVisitor : FullSimpleBaseVisitor<Func<Context, IType>>
{
    private readonly ArrowTypeVisitor _arrowTypeVisitor;

    public TypeVisitor()
    {
        _arrowTypeVisitor = new ArrowTypeVisitor(this);
    }

    public override Func<Context, IType> VisitType_arrowtype(FullSimpleParser.Type_arrowtypeContext context) => _arrowTypeVisitor.Visit(context.arrowtype());
}