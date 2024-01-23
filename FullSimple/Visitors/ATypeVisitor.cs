using Antlr4.Runtime.Misc;
using Common;
using FullSimple.Syntax;
using System;

namespace FullSimple.Visitors;

public class ATypeVisitor : FullSimpleBaseVisitor<Func<Context, IType>>
{
    private readonly TypeVisitor _typeVisitor;
    private readonly FieldTypesVisitor _fieldTypesVisitor;

    public ATypeVisitor(TypeVisitor typeVisitor)
    {
            _typeVisitor = typeVisitor;
            _fieldTypesVisitor = new FieldTypesVisitor(typeVisitor);
        }
    public override Func<Context, IType> VisitAt_bool([NotNull] FullSimpleParser.At_boolContext context)
    {
            return _ => new TypeBool();
        }

    public override Func<Context, IType> VisitAt_nat([NotNull] FullSimpleParser.At_natContext context)
    {
            return _ => new TypeNat();
        }

    public override Func<Context, IType> VisitAt_type([NotNull] FullSimpleParser.At_typeContext context)
    {
            return _typeVisitor.Visit(context.type());
        }

    public override Func<Context, IType> VisitAt_ufloat([NotNull] FullSimpleParser.At_ufloatContext context)
    {
            return _ => new TypeFloat();
        }

    public override Func<Context, IType> VisitAt_ustring([NotNull] FullSimpleParser.At_ustringContext context)
    {
            return _ => new TypeString();
        }

    public override Func<Context, IType> VisitAt_uunit([NotNull] FullSimpleParser.At_uunitContext context)
    {
            return _ => new TypeUnit();
        }

    public override Func<Context, IType> VisitAt_record([NotNull] FullSimpleParser.At_recordContext context)
    {
            var types = _fieldTypesVisitor.Visit(context.fieldtypes());
            return ctx => new TypeRecord(types(ctx, 1));
        }

    public override Func<Context, IType> VisitAt_ucid([NotNull] FullSimpleParser.At_ucidContext context)
    {
            var name = context.UCID().GetText();

            return ctx =>
            {
                if (ctx.IsNameBound(name))
                    return new TypeVar(ctx.NameToIndex(name), ctx.Length);

                return new TypeId(name);
            };
        }

    public override Func<Context, IType> VisitAt_variant([NotNull] FullSimpleParser.At_variantContext context)
    {
            var types = _fieldTypesVisitor.Visit(context.fieldtypes());
            return ctx => new TypeVariant(types(ctx, 1));
        }
}