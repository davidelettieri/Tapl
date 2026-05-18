using System;
using Common;
using FullRecon.Syntax.Bindings;

namespace FullRecon.Visitors;

public sealed class BinderVisitor : FullReconBaseVisitor<Func<Context, IBinding>>
{
    private static readonly TypeVisitor TypeVisitor = new();

    public override Func<Context, IBinding> VisitBinder_type(FullReconParser.Binder_typeContext context)
    {
        var type = TypeVisitor.Visit(context.type());
        return ctx => new VarBind(type(ctx));
    }
}
