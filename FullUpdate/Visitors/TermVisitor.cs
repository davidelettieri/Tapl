using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Common;
using FullUpdate.Syntax;
using FullUpdate.Syntax.Terms;

namespace FullUpdate.Visitors;

public sealed class TermVisitor : FullUpdateBaseVisitor<Func<Context, ITerm>>
{
    private readonly TypeVisitor _typeVisitor = new();
    private readonly FieldsVisitor _fieldsVisitor;

    public TermVisitor()
    {
        _fieldsVisitor = new FieldsVisitor(this);
    }

    // ---- term rules ----

    public override Func<Context, ITerm> VisitTerm_tabs(FullUpdateParser.Term_tabsContext context)
    {
        var info = context.GetFileInfo();
        var name = context.UCID().GetText();
        var otype = new OTypeVisitor().Visit(context.otype());
        var body = Visit(context.term());
        return ctx =>
        {
            var bound = otype(ctx);
            var ctx1 = ctx.AddName(name);
            return new TAbs(info, name, bound, body(ctx1));
        };
    }

    public override Func<Context, ITerm> VisitTerm_llcid(FullUpdateParser.Term_llcidContext context)
    {
        var info = context.GetFileInfo();
        var v = context.LCID().GetText();
        var type = _typeVisitor.Visit(context.type());
        var body = Visit(context.term());
        return ctx =>
        {
            var ctx1 = ctx.AddName(v);
            return new Abs(info, v, type(ctx), body(ctx1));
        };
    }

    public override Func<Context, ITerm> VisitTerm_luc(FullUpdateParser.Term_lucContext context)
    {
        var info = context.GetFileInfo();
        var type = _typeVisitor.Visit(context.type());
        var body = Visit(context.term());
        return ctx =>
        {
            var ctx1 = ctx.AddName("_");
            return new Abs(info, "_", type(ctx), body(ctx1));
        };
    }

    public override Func<Context, ITerm> VisitTerm_unpack(FullUpdateParser.Term_unpackContext context)
    {
        var info = context.GetFileInfo();
        var ucid = context.UCID().GetText();
        var lcid = context.LCID().GetText();
        var terms = context.term();
        var t1 = Visit(terms[0]);
        var t2 = Visit(terms[1]);
        return ctx =>
        {
            var ctx1 = ctx.AddName(ucid);
            var ctx2 = ctx1.AddName(lcid);
            return new Unpack(info, ucid, lcid, t1(ctx), t2(ctx2));
        };
    }

    public override Func<Context, ITerm> VisitTerm_ll(FullUpdateParser.Term_llContext context)
    {
        var info = context.GetFileInfo();
        var v = context.LCID().GetText();
        var terms = context.term();
        var t1 = Visit(terms[0]);
        var t2 = Visit(terms[1]);
        return ctx =>
        {
            var ctx1 = ctx.AddName(v);
            return new Let(info, v, t1(ctx), t2(ctx1));
        };
    }

    public override Func<Context, ITerm> VisitTerm_lu(FullUpdateParser.Term_luContext context)
    {
        var info = context.GetFileInfo();
        var terms = context.term();
        var t1 = Visit(terms[0]);
        var t2 = Visit(terms[1]);
        return ctx =>
        {
            var ctx1 = ctx.AddName("_");
            return new Let(info, "_", t1(ctx), t2(ctx1));
        };
    }

    public override Func<Context, ITerm> VisitTerm_letrec(FullUpdateParser.Term_letrecContext context)
    {
        var info = context.GetFileInfo();
        var v = context.LCID().GetText();
        var type = _typeVisitor.Visit(context.type());
        var terms = context.term();
        var t1 = Visit(terms[0]);
        var t2 = Visit(terms[1]);
        return ctx =>
        {
            var ctx1 = ctx.AddName(v);
            var abs = new Abs(info, v, type(ctx), t1(ctx1));
            var fix = new Fix(info, abs);
            return new Let(info, v, fix, t2(ctx1));
        };
    }

    public override Func<Context, ITerm> VisitTerm_ift(FullUpdateParser.Term_iftContext context)
    {
        var info = context.GetFileInfo();
        var terms = context.term();
        var cond = Visit(terms[0]);
        var then = Visit(terms[1]);
        var els = Visit(terms[2]);
        return ctx => new If(info, cond(ctx), then(ctx), els(ctx));
    }

    public override Func<Context, ITerm> VisitTerm_update(FullUpdateParser.Term_updateContext context)
    {
        var info = context.GetFileInfo();
        var record = new AppTermVisitor(this).Visit(context.appterm());
        var label = context.LCID().GetText();
        var newVal = Visit(context.term());
        return ctx => new Update(info, record(ctx), label, newVal(ctx));
    }

    public override Func<Context, ITerm> VisitTerm_appterm(FullUpdateParser.Term_apptermContext context)
        => new AppTermVisitor(this).Visit(context.appterm());
}
