using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using FullUpdate.Core;
using FullUpdate.Syntax;

namespace FullUpdate.Visitors;

public sealed class ATypeVisitor(TypeVisitor typeVisitor) : FullUpdateBaseVisitor<Func<Context, IType>>
{
    private readonly FieldTypesVisitor _fieldTypesVisitor = new(typeVisitor);
    private static readonly KindVisitor KindVisitor = new();

    public override Func<Context, IType> VisitAt_type(FullUpdateParser.At_typeContext context)
        => typeVisitor.Visit(context.type());

    public override Func<Context, IType> VisitAt_ucid(FullUpdateParser.At_ucidContext context)
    {
        var name = context.UCID().GetText();
        return ctx => ctx.IsNameBound(name)
            ? new TypeVar(ctx.NameToIndex(name), ctx.Length)
            : new TypeId(name);
    }

    public override Func<Context, IType> VisitAt_bool(FullUpdateParser.At_boolContext context)
        => _ => new TypeBool();

    public override Func<Context, IType> VisitAt_ustring(FullUpdateParser.At_ustringContext context)
        => _ => new TypeString();

    public override Func<Context, IType> VisitAt_uunit(FullUpdateParser.At_uunitContext context)
        => _ => new TypeUnit();

    public override Func<Context, IType> VisitAt_ufloat(FullUpdateParser.At_ufloatContext context)
        => _ => new TypeFloat();

    public override Func<Context, IType> VisitAt_nat(FullUpdateParser.At_natContext context)
        => _ => new TypeNat();

    public override Func<Context, IType> VisitAt_top(FullUpdateParser.At_topContext context)
        => _ => new TypeTop();

    public override Func<Context, IType> VisitAt_top_kn(FullUpdateParser.At_top_knContext context)
    {
        var kind = KindVisitor.Visit(context.kind());
        return ctx => Typing.MakeTop(kind(ctx));
    }

    public override Func<Context, IType> VisitAt_some(FullUpdateParser.At_someContext context)
    {
        var name = context.UCID().GetText();
        var otype = new OTypeVisitor().Visit(context.otype());
        var body = typeVisitor.Visit(context.type());
        return ctx =>
        {
            var bound = otype(ctx);
            var ctx1 = ctx.AddName(name);
            return new TypeSome(name, bound, body(ctx1));
        };
    }

    public override Func<Context, IType> VisitAt_record(FullUpdateParser.At_recordContext context)
    {
        var types = _fieldTypesVisitor.Visit(context.fieldtypes());
        return ctx => new TypeRecord(types(ctx, 1).ToList());
    }
}
