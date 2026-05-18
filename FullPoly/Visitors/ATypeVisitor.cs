using System;
using Common;
using FullPoly.Syntax;

namespace FullPoly.Visitors;

public sealed class ATypeVisitor(TypeVisitor typeVisitor) : FullPolyBaseVisitor<Func<Context, IType>>
{
    private readonly FieldTypesVisitor _fieldTypesVisitor = new(typeVisitor);

    public override Func<Context, IType> VisitAt_bool(FullPolyParser.At_boolContext context) => _ => new TypeBool();

    public override Func<Context, IType> VisitAt_nat(FullPolyParser.At_natContext context) => _ => new TypeNat();

    public override Func<Context, IType> VisitAt_type(FullPolyParser.At_typeContext context) => typeVisitor.Visit(context.type());

    public override Func<Context, IType> VisitAt_ufloat(FullPolyParser.At_ufloatContext context) => _ => new TypeFloat();

    public override Func<Context, IType> VisitAt_ustring(FullPolyParser.At_ustringContext context) => _ => new TypeString();

    public override Func<Context, IType> VisitAt_uunit(FullPolyParser.At_uunitContext context) => _ => new TypeUnit();

    public override Func<Context, IType> VisitAt_record(FullPolyParser.At_recordContext context)
    {
        var types = _fieldTypesVisitor.Visit(context.fieldtypes());
        return ctx => new TypeRecord(types(ctx, 1));
    }

    public override Func<Context, IType> VisitAt_ucid(FullPolyParser.At_ucidContext context)
    {
        var name = context.UCID().GetText();

        return ctx =>
        {
            if (ctx.IsNameBound(name))
                return new TypeVar(ctx.NameToIndex(name), ctx.Length);

            return new TypeId(name);
        };
    }

    public override Func<Context, IType> VisitAt_variant(FullPolyParser.At_variantContext context)
    {
        var types = _fieldTypesVisitor.Visit(context.fieldtypes());
        return ctx => new TypeVariant(types(ctx, 1));
    }

    public override Func<Context, IType> VisitAt_some(FullPolyParser.At_someContext context)
    {
        var name = context.UCID().GetText();
        var body = typeVisitor.Visit(context.type());
        return ctx => new TypeSome(name, body(ctx.AddName(name)));
    }
}
