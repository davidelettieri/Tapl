using System;
using Common;
using FullRef.Syntax;

namespace FullRef.Visitors;

public sealed class ATypeVisitor(TypeVisitor typeVisitor) : FullRefBaseVisitor<Func<Context, IType>>
{
    private readonly FieldTypesVisitor _fieldTypesVisitor = new(typeVisitor);

    public override Func<Context, IType> VisitAt_tbot(FullRefParser.At_tbotContext context) => _ => new TypeBot();

    public override Func<Context, IType> VisitAt_ttop(FullRefParser.At_ttopContext context) => _ => new TypeTop();

    public override Func<Context, IType> VisitAt_bool(FullRefParser.At_boolContext context) => _ => new TypeBool();

    public override Func<Context, IType> VisitAt_nat(FullRefParser.At_natContext context) => _ => new TypeNat();

    public override Func<Context, IType> VisitAt_type(FullRefParser.At_typeContext context) => typeVisitor.Visit(context.type());

    public override Func<Context, IType> VisitAt_ufloat(FullRefParser.At_ufloatContext context) => _ => new TypeFloat();

    public override Func<Context, IType> VisitAt_ustring(FullRefParser.At_ustringContext context) => _ => new TypeString();

    public override Func<Context, IType> VisitAt_uunit(FullRefParser.At_uunitContext context) => _ => new TypeUnit();

    public override Func<Context, IType> VisitAt_record(FullRefParser.At_recordContext context)
    {
        var types = _fieldTypesVisitor.Visit(context.fieldtypes());
        return ctx => new TypeRecord(types(ctx, 1));
    }

    public override Func<Context, IType> VisitAt_ucid(FullRefParser.At_ucidContext context)
    {
        var name = context.UCID().GetText();

        return ctx =>
        {
            if (ctx.IsNameBound(name))
                return new TypeVar(ctx.NameToIndex(name), ctx.Length);

            return new TypeId(name);
        };
    }

    public override Func<Context, IType> VisitAt_variant(FullRefParser.At_variantContext context)
    {
        var types = _fieldTypesVisitor.Visit(context.fieldtypes());
        return ctx => new TypeVariant(types(ctx, 1));
    }
}