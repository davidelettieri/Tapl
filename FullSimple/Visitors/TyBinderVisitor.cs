using Antlr4.Runtime.Misc;
using Common;
using FullSimple.Syntax.Bindings;
using System;

namespace FullSimple.Visitors;

public class TyBinderVisitor : FullSimpleBaseVisitor<Func<Context, IBinding>>
{
    public static readonly TypeVisitor _typeVisitor = new TypeVisitor();
    public override Func<Context, IBinding> VisitTybinder_type([NotNull] FullSimpleParser.Tybinder_typeContext context)
    {
            if (context.type() != null)
            {
                var type = _typeVisitor.Visit(context.type());

                return ctx => new TypeAbbBind(type(ctx));
            }

            return _ => new TypeVarBind();
        }
}