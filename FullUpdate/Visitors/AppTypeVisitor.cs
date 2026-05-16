using System;
using Common;
using FullUpdate.Syntax;

namespace FullUpdate.Visitors;

public sealed class AppTypeVisitor(TypeVisitor typeVisitor) : FullUpdateBaseVisitor<Func<Context, IType>>
{
    private readonly ATypeVisitor _aTypeVisitor = new(typeVisitor);

    public override Func<Context, IType> VisitApptype_app(FullUpdateParser.Apptype_appContext context)
    {
        var left = Visit(context.apptype());
        var right = _aTypeVisitor.Visit(context.atype());
        return ctx => new TypeApp(left(ctx), right(ctx));
    }

    public override Func<Context, IType> VisitApptype_atype(FullUpdateParser.Apptype_atypeContext context)
        => _aTypeVisitor.Visit(context.atype());
}
