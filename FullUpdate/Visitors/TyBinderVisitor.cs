using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using FullUpdate.Core;
using FullUpdate.Syntax;
using FullUpdate.Syntax.Bindings;

namespace FullUpdate.Visitors;

public sealed class TyBinderVisitor : FullUpdateBaseVisitor<Func<Context, IBinding>>
{
    private static readonly TypeVisitor TypeVisitor = new();
    private static readonly KindVisitor KindVisitor = new();

    public override Func<Context, IBinding> VisitTybinder_kn(FullUpdateParser.Tybinder_knContext context)
    {
        var kind = KindVisitor.Visit(context.kind());
        return ctx => new TypeVarBind(Typing.MakeTop(kind(ctx)));
    }

    public override Func<Context, IBinding> VisitTybinder_leq(FullUpdateParser.Tybinder_leqContext context)
    {
        var type = TypeVisitor.Visit(context.type());
        return ctx => new TypeVarBind(type(ctx));
    }

    public override Func<Context, IBinding> VisitTybinder_abbtype(FullUpdateParser.Tybinder_abbtypeContext context)
    {
        var tyabbargs = new TyAbbArgsVisitor().Visit(context.tyabbargs());
        var typeFunc = TypeVisitor.Visit(context.type());
        return ctx =>
        {
            var (binders, ctx2) = tyabbargs(new List<(string, IKind)>(), ctx);
            var bodyTy = typeFunc(ctx2);
            IType result = bodyTy;
            for (int i = binders.Count - 1; i >= 0; i--)
                result = new TypeAbs(binders[i].Item1, binders[i].Item2, result);
            return new TypeAbbBind(result, null);
        };
    }

    public override Func<Context, IBinding> VisitTybinder_empty(FullUpdateParser.Tybinder_emptyContext context)
    {
        return _ => new TypeVarBind(new TypeTop());
    }
}
