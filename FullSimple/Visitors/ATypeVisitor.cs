using Antlr4.Runtime.Misc;
using Common;
using FullSimple.Syntax;
using System;

namespace FullSimple.Visitors;

public sealed class ATypeVisitor(TypeVisitor typeVisitor) : FullSimpleBaseVisitor<Func<Context, IType>>
{
    private readonly FieldTypesVisitor _fieldTypesVisitor = new(typeVisitor);

    public override Func<Context, IType> VisitAt_bool(FullSimpleParser.At_boolContext context) => _ => new TypeBool();

    public override Func<Context, IType> VisitAt_nat(FullSimpleParser.At_natContext context) => _ => new TypeNat();

    public override Func<Context, IType> VisitAt_type(FullSimpleParser.At_typeContext context) => typeVisitor.Visit(context.type());

    public override Func<Context, IType> VisitAt_ufloat(FullSimpleParser.At_ufloatContext context) => _ => new TypeFloat();

    public override Func<Context, IType> VisitAt_ustring(FullSimpleParser.At_ustringContext context) => _ => new TypeString();

    public override Func<Context, IType> VisitAt_uunit(FullSimpleParser.At_uunitContext context) => _ => new TypeUnit();

    public override Func<Context, IType> VisitAt_record(FullSimpleParser.At_recordContext context)
    {
        var types = _fieldTypesVisitor.Visit(context.fieldtypes());
        return ctx => new TypeRecord(types(ctx, 1));
    }

    public override Func<Context, IType> VisitAt_ucid(FullSimpleParser.At_ucidContext context)
    {
        var name = context.UCID().GetText();

        return ctx =>
        {
            if (ctx.IsNameBound(name))
                return new TypeVar(ctx.NameToIndex(name), ctx.Length);

            return new TypeId(name);
        };
    }

    public override Func<Context, IType> VisitAt_variant(FullSimpleParser.At_variantContext context)
    {
        var types = _fieldTypesVisitor.Visit(context.fieldtypes());
        return ctx => new TypeVariant(types(ctx, 1));
    }
}