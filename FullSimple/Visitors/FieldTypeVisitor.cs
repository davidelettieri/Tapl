using Antlr4.Runtime.Misc;
using Common;
using System;

namespace FullSimple.Visitors;

public class FieldTypeVisitor : FullSimpleBaseVisitor<Func<Context, int, (string, IType)>>
{
    private readonly TypeVisitor _typeVisitor;

    public FieldTypeVisitor(TypeVisitor typeVisitor)
    {
            _typeVisitor = typeVisitor;
        }

    public override Func<Context, int, (string, IType)> VisitFieldtype_lcid([NotNull] FullSimpleParser.Fieldtype_lcidContext context)
    {
            var name = context.LCID().GetText();
            var type = _typeVisitor.Visit(context.type());

            return (ctx, _) => (name, type(ctx));
        }

    public override Func<Context, int, (string, IType)> VisitFieldtype_type([NotNull] FullSimpleParser.Fieldtype_typeContext context)
    {
            var type = _typeVisitor.Visit(context.type());

            return (ctx, i) => (i.ToString(), type(ctx));
        }
}