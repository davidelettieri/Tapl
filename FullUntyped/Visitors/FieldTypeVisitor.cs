using System;
using Common;

namespace FullUntyped.Visitors;

public sealed class FieldTypeVisitor(TypeVisitor typeVisitor)
    : FullUntypedBaseVisitor<Func<Context, int, (string, IType)>>
{
    public override Func<Context, int, (string, IType)> VisitFieldtype_lcid(
        FullUntypedParser.Fieldtype_lcidContext context)
    {
        var name = context.LCID().GetText();
        var type = typeVisitor.Visit(context.type());

        return (ctx, _) => (name, type(ctx));
    }

    public override Func<Context, int, (string, IType)> VisitFieldtype_type(
        FullUntypedParser.Fieldtype_typeContext context)
    {
        var type = typeVisitor.Visit(context.type());

        return (ctx, i) => (i.ToString(), type(ctx));
    }
}