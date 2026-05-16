using System;
using Common;
using FullUpdate.Syntax;
using FullUpdate.Syntax.Terms;

namespace FullUpdate.Visitors;

public sealed class TermSeqVisitor : FullUpdateBaseVisitor<Func<Context, ITerm>>
{
    private readonly TermVisitor _termVisitor;

    public TermSeqVisitor(TermVisitor termVisitor) => _termVisitor = termVisitor;

    public override Func<Context, ITerm> VisitTermseq_term(FullUpdateParser.Termseq_termContext context)
        => _termVisitor.Visit(context.term());

    public override Func<Context, ITerm> VisitTermseq_seq(FullUpdateParser.Termseq_seqContext context)
    {
        var info = context.GetFileInfo();
        var t1 = _termVisitor.Visit(context.term());
        var rest = Visit(context.termseq());
        return ctx =>
        {
            var ctx1 = ctx.AddName("_");
            var abs = new Abs(info, "_", new TypeUnit(), rest(ctx1));
            return new App(info, abs, t1(ctx));
        };
    }
}
