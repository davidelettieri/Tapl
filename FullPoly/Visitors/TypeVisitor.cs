using System;
using Common;
using FullPoly.Syntax;

namespace FullPoly.Visitors;

public sealed class TypeVisitor : FullPolyBaseVisitor<Func<Context, IType>>
{
    private readonly ArrowTypeVisitor _arrowTypeVisitor;

    public TypeVisitor()
    {
        _arrowTypeVisitor = new ArrowTypeVisitor(this);
    }

    public override Func<Context, IType> VisitType_arrowtype(FullPolyParser.Type_arrowtypeContext context) => _arrowTypeVisitor.Visit(context.arrowtype());

    public override Func<Context, IType> VisitType_all(FullPolyParser.Type_allContext context)
    {
        var name = context.UCID().GetText();
        var body = Visit(context.type());
        return ctx => new TypeAll(name, body(ctx.AddName(name)));
    }
}
