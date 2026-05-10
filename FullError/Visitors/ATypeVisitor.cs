using System;
using Common;
using FullError.Syntax;

namespace FullError.Visitors;

public sealed class ATypeVisitor(TypeVisitor typeVisitor) : FullErrorBaseVisitor<Func<Context, IType>>
{
    public override Func<Context, IType> VisitAt_bool(FullErrorParser.At_boolContext context)
        => _ => new TypeBool();

    public override Func<Context, IType> VisitAt_bot(FullErrorParser.At_botContext context)
        => _ => new TypeBot();

    public override Func<Context, IType> VisitAt_top(FullErrorParser.At_topContext context)
        => _ => new TypeTop();

    public override Func<Context, IType> VisitAt_type(FullErrorParser.At_typeContext context)
        => typeVisitor.Visit(context.type());

    public override Func<Context, IType> VisitAt_ucid(FullErrorParser.At_ucidContext context)
    {
        var name = context.UCID().GetText();

        return ctx => new TypeVar(ctx.NameToIndex(name), ctx.Length);
    }
}
