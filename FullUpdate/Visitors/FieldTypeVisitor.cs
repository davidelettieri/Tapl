using System;
using Common;
using FullUpdate.Syntax;

namespace FullUpdate.Visitors;

public sealed class FieldTypeVisitor : FullUpdateBaseVisitor<Func<Context, int, (string, Variance, IType)>>
{
    private readonly TypeVisitor _typeVisitor;

    public FieldTypeVisitor(TypeVisitor typeVisitor) => _typeVisitor = typeVisitor;

    public override Func<Context, int, (string, Variance, IType)> VisitFieldtype_hash_lcid(
        FullUpdateParser.Fieldtype_hash_lcidContext context)
    {
        var name = context.LCID().GetText();
        var type = _typeVisitor.Visit(context.type());
        return (ctx, _) => (name, Variance.Invariant, type(ctx));
    }

    public override Func<Context, int, (string, Variance, IType)> VisitFieldtype_hash_type(
        FullUpdateParser.Fieldtype_hash_typeContext context)
    {
        var type = _typeVisitor.Visit(context.type());
        return (ctx, i) => (i.ToString(), Variance.Invariant, type(ctx));
    }

    public override Func<Context, int, (string, Variance, IType)> VisitFieldtype_lcid(
        FullUpdateParser.Fieldtype_lcidContext context)
    {
        var name = context.LCID().GetText();
        var type = _typeVisitor.Visit(context.type());
        return (ctx, _) => (name, Variance.Covariant, type(ctx));
    }

    public override Func<Context, int, (string, Variance, IType)> VisitFieldtype_type(
        FullUpdateParser.Fieldtype_typeContext context)
    {
        var type = _typeVisitor.Visit(context.type());
        return (ctx, i) => (i.ToString(), Variance.Covariant, type(ctx));
    }
}
