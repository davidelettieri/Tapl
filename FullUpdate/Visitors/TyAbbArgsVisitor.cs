using System;
using System.Collections.Generic;
using Common;
using FullUpdate.Syntax;

namespace FullUpdate.Visitors;

/// <summary>
/// Visits tyabbargs and accumulates the list of (name, kind) binders.
/// Returns Func&lt;(accumulatedBinders, ctx), (binders, extendedCtx)&gt;
/// </summary>
public sealed class TyAbbArgsVisitor
    : FullUpdateBaseVisitor<Func<List<(string, IKind)>, Context, (List<(string, IKind)>, Context)>>
{

    public override Func<List<(string, IKind)>, Context, (List<(string, IKind)>, Context)> VisitTyabbargs_ucid(
        FullUpdateParser.Tyabbargs_ucidContext context)
    {
        var name = context.UCID().GetText();
        var okind = new OKindVisitor().Visit(context.okind());
        var rest = Visit(context.tyabbargs());
        return (binders, ctx) =>
        {
            var k = okind(ctx);
            var ctx1 = ctx.AddName(name);
            var newBinders = new List<(string, IKind)>(binders) { (name, k) };
            return rest(newBinders, ctx1);
        };
    }

    public override Func<List<(string, IKind)>, Context, (List<(string, IKind)>, Context)> VisitTyabbargs_empty(
        FullUpdateParser.Tyabbargs_emptyContext context)
    {
        return (binders, ctx) => (binders, ctx);
    }
}
