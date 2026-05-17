using System;
using System.Globalization;
using Common;
using FullUpdate.Syntax;
using FullUpdate.Syntax.Terms;

namespace FullUpdate.Visitors;

public sealed class ATermVisitor : FullUpdateBaseVisitor<Func<Context, ITerm>>
{
    private readonly TermVisitor _termVisitor;
    private readonly TypeVisitor _typeVisitor = new();
    private readonly FieldsVisitor _fieldsVisitor;

    public ATermVisitor(TermVisitor termVisitor)
    {
        _termVisitor = termVisitor;
        _fieldsVisitor = new FieldsVisitor(termVisitor);
    }

    public override Func<Context, ITerm> VisitAterm_paren(FullUpdateParser.Aterm_parenContext context)
        => new TermSeqVisitor(_termVisitor).Visit(context.termseq());

    public override Func<Context, ITerm> VisitAterm_inert(FullUpdateParser.Aterm_inertContext context)
    {
        var info = context.GetFileInfo();
        var ty = _typeVisitor.Visit(context.type());
        return ctx => new Inert(info, ty(ctx));
    }

    public override Func<Context, ITerm> VisitAterm_true(FullUpdateParser.Aterm_trueContext context)
    {
        var info = context.GetFileInfo();
        return _ => new True(info);
    }

    public override Func<Context, ITerm> VisitAterm_false(FullUpdateParser.Aterm_falseContext context)
    {
        var info = context.GetFileInfo();
        return _ => new False(info);
    }

    public override Func<Context, ITerm> VisitAterm_pack(FullUpdateParser.Aterm_packContext context)
    {
        var info = context.GetFileInfo();
        var types = context.type();
        var ty1 = _typeVisitor.Visit(types[0]);
        var t = _termVisitor.Visit(context.term());
        var ty2 = _typeVisitor.Visit(types[1]);
        return ctx => new Pack(info, ty1(ctx), t(ctx), ty2(ctx));
    }

    public override Func<Context, ITerm> VisitAterm_fields(FullUpdateParser.Aterm_fieldsContext context)
    {
        var info = context.GetFileInfo();
        var fields = _fieldsVisitor.Visit(context.fields());
        return ctx => new Record(info, fields(ctx, 1));
    }

    public override Func<Context, ITerm> VisitAterm_lcid(FullUpdateParser.Aterm_lcidContext context)
    {
        var info = context.GetFileInfo();
        var name = context.LCID().GetText();
        return ctx => new Var(info, ctx.NameToIndex(name), ctx.Length);
    }

    public override Func<Context, ITerm> VisitAterm_stringv(FullUpdateParser.Aterm_stringvContext context)
    {
        var info = context.GetFileInfo();
        var text = context.STRINGV().GetText();
        // Strip surrounding quotes
        var val = text.Substring(1, text.Length - 2);
        return _ => new StringTerm(info, val);
    }

    public override Func<Context, ITerm> VisitAterm_unit(FullUpdateParser.Aterm_unitContext context)
    {
        var info = context.GetFileInfo();
        return _ => new Unit(info);
    }

    public override Func<Context, ITerm> VisitAterm_floatv(FullUpdateParser.Aterm_floatvContext context)
    {
        var info = context.GetFileInfo();
        var v = double.Parse(context.FLOATV().GetText(), CultureInfo.InvariantCulture);
        return _ => new Float(info, v);
    }

    public override Func<Context, ITerm> VisitAterm_intv(FullUpdateParser.Aterm_intvContext context)
    {
        var info = context.GetFileInfo();
        var n = int.Parse(context.INTV().GetText());
        return _ =>
        {
            ITerm t = new Zero(info);
            for (int i = 0; i < n; i++) t = new Succ(info, t);
            return t;
        };
    }
}
