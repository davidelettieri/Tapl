using System;
using Common;
using FullUpdate.Syntax;

namespace FullUpdate.Visitors;

public sealed class TypeVisitor : FullUpdateBaseVisitor<Func<Context, IType>>
{
    private readonly ArrowTypeVisitor _arrowTypeVisitor;

    public TypeVisitor()
    {
        _arrowTypeVisitor = new ArrowTypeVisitor(this);
    }

    public override Func<Context, IType> VisitType_arrowtype(FullUpdateParser.Type_arrowtypeContext context)
        => _arrowTypeVisitor.Visit(context.arrowtype());

    public override Func<Context, IType> VisitType_abs(FullUpdateParser.Type_absContext context)
    {
        var name = context.UCID().GetText();
        var okind = new OKindVisitor().Visit(context.okind());
        var body = Visit(context.type());
        return ctx =>
        {
            var k = okind(ctx);
            var ctx1 = ctx.AddName(name);
            return new TypeAbs(name, k, body(ctx1));
        };
    }

    public override Func<Context, IType> VisitType_all(FullUpdateParser.Type_allContext context)
    {
        var name = context.UCID().GetText();
        var otype = new OTypeVisitor().Visit(context.otype());
        var body = Visit(context.type());
        return ctx =>
        {
            var bound = otype(ctx);
            var ctx1 = ctx.AddName(name);
            return new TypeAll(name, bound, body(ctx1));
        };
    }
}
