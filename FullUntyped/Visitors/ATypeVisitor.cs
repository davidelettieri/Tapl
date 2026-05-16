using System;
using Common;
using FullUntyped.Syntax;

namespace FullUntyped.Visitors;

public sealed class ATypeVisitor(TypeVisitor typeVisitor) : FullUntypedBaseVisitor<Func<Context, IType>>
{
    private readonly FieldTypesVisitor _fieldTypesVisitor = new(typeVisitor);

    public override Func<Context, IType> VisitAt_bool(FullUntypedParser.At_boolContext context) => _ => new TypeBool();

    public override Func<Context, IType> VisitAt_nat(FullUntypedParser.At_natContext context) => _ => new TypeNat();

    public override Func<Context, IType> VisitAt_type(FullUntypedParser.At_typeContext context) => typeVisitor.Visit(context.type());

    public override Func<Context, IType> VisitAt_ufloat(FullUntypedParser.At_ufloatContext context) => _ => new TypeFloat();

    public override Func<Context, IType> VisitAt_ustring(FullUntypedParser.At_ustringContext context) => _ => new TypeString();

    public override Func<Context, IType> VisitAt_uunit(FullUntypedParser.At_uunitContext context) => _ => new TypeUnit();

    public override Func<Context, IType> VisitAt_record(FullUntypedParser.At_recordContext context)
    {
        var types = _fieldTypesVisitor.Visit(context.fieldtypes());
        return ctx => new TypeRecord(types(ctx, 1));
    }

    public override Func<Context, IType> VisitAt_ucid(FullUntypedParser.At_ucidContext context)
    {
        var name = context.UCID().GetText();

        return ctx =>
        {
            if (ctx.IsNameBound(name))
                return new TypeVar(ctx.NameToIndex(name), ctx.Length);

            return new TypeId(name);
        };
    }

    public override Func<Context, IType> VisitAt_variant(FullUntypedParser.At_variantContext context)
    {
        var types = _fieldTypesVisitor.Visit(context.fieldtypes());
        return ctx => new TypeVariant(types(ctx, 1));
    }
}