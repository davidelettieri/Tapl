using System;
using Common;

namespace FullRef.Visitors;

public sealed class FieldTypeVisitor(TypeVisitor typeVisitor)
    : FullRefBaseVisitor<Func<Context, int, (string, IType)>>
{
    public override Func<Context, int, (string, IType)> VisitFieldtype_lcid(
        FullRefParser.Fieldtype_lcidContext context)
    {
        var name = context.LCID().GetText();
        var type = typeVisitor.Visit(context.type());

        return (ctx, _) => (name, type(ctx));
    }

    public override Func<Context, int, (string, IType)> VisitFieldtype_type(
        FullRefParser.Fieldtype_typeContext context)
    {
        var type = typeVisitor.Visit(context.type());

        return (ctx, i) => (i.ToString(), type(ctx));
    }
}