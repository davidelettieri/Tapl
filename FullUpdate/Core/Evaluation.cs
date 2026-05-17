using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using FullUpdate.Syntax;
using FullUpdate.Syntax.Bindings;
using FullUpdate.Syntax.Terms;
using static FullUpdate.Core.Shifting;
using static FullUpdate.Core.Substitution;

namespace FullUpdate.Core;

public static class Evaluation
{
    private static bool IsNumericVal(Context ctx, ITerm t) => t switch
    {
        Zero => true,
        Succ s => IsNumericVal(ctx, s.Of),
        _ => false
    };

    public static bool IsVal(Context ctx, ITerm t) => t switch
    {
        StringTerm => true,
        Pack p when IsVal(ctx, p.Term) => true,
        TAbs => true,
        True => true,
        False => true,
        ITerm s when IsNumericVal(ctx, s) => true,
        Abs => true,
        Record r when r.Fields.All(f => IsVal(ctx, f.Term)) => true,
        Unit => true,
        Float => true,
        _ => false
    };

    private static ITerm Eval1(Context ctx, ITerm t) => t switch
    {
        App { Left: Abs abs } app when IsVal(ctx, app.Right) => TermSubstTop(app.Right, abs.Body),
        App app when IsVal(ctx, app.Left) => new App(app.Info, app.Left, Eval1(ctx, app.Right)),
        App app => new App(app.Info, Eval1(ctx, app.Left), app.Right),
        Ascribe asc when IsVal(ctx, asc.Term) => asc.Term,
        Ascribe asc => new Ascribe(asc.Info, Eval1(ctx, asc.Term), asc.Type),
        TApp { Term: TAbs tabs } tapp => TyTermSubstTop(tapp.TypeArg, tabs.Body),
        TApp tapp => new TApp(tapp.Info, Eval1(ctx, tapp.Term), tapp.TypeArg),
        Unpack { Package: Pack pack } unpack when IsVal(ctx, pack.Term)
            => TyTermSubstTop(pack.WitnessType, TermSubstTop(TermShift(1, pack.Term), unpack.Body)),
        Unpack unpack => new Unpack(unpack.Info, unpack.TypeVar, unpack.V, Eval1(ctx, unpack.Package), unpack.Body),
        Pack pack => new Pack(pack.Info, pack.WitnessType, Eval1(ctx, pack.Term), pack.ExistType),
        Record rec => EvalRecord(ctx, rec),
        Proj { Term: Record rec } proj when IsVal(ctx, rec) => ProjRecord(rec, proj.Label),
        Proj proj => new Proj(proj.Info, Eval1(ctx, proj.Term), proj.Label),
        If { Condition: True } ift => ift.Then,
        If { Condition: False } ift => ift.Else,
        If ift => new If(ift.Info, Eval1(ctx, ift.Condition), ift.Then, ift.Else),
        Succ succ => new Succ(succ.Info, Eval1(ctx, succ.Of)),
        Pred { Of: Zero } => new Zero(new UnknownInfo()),
        Pred { Of: Succ s } when IsNumericVal(ctx, s.Of) => s.Of,
        Pred p => new Pred(p.Info, Eval1(ctx, p.Of)),
        IsZero { Term: Zero } => new True(new UnknownInfo()),
        IsZero { Term: Succ s } when IsNumericVal(ctx, s.Of) => new False(new UnknownInfo()),
        IsZero iz => new IsZero(iz.Info, Eval1(ctx, iz.Term)),
        Var var => EvalVar(ctx, var),
        Update { Record: Record rec } upd when IsVal(ctx, rec) =>
            new Record(new UnknownInfo(),
                rec.Fields.Select(f => f.Label == upd.Label ? (f.Label, f.Var, upd.NewValue) : f).ToList()),
        Update upd when IsVal(ctx, upd.Record) => new Update(upd.Info, upd.Record, upd.Label, Eval1(ctx, upd.NewValue)),
        Update upd => new Update(upd.Info, Eval1(ctx, upd.Record), upd.Label, upd.NewValue),
        TimesFloat { Left: Float fl, Right: Float fr } tf => new Float(tf.Info, fl.Value * fr.Value),
        TimesFloat { Left: Float } tf => new TimesFloat(tf.Info, tf.Left, Eval1(ctx, tf.Right)),
        TimesFloat tf => new TimesFloat(tf.Info, Eval1(ctx, tf.Left), tf.Right),
        Let let when IsVal(ctx, let.LetTerm) => TermSubstTop(let.LetTerm, let.InTerm),
        Let let => new Let(let.Info, let.Variable, Eval1(ctx, let.LetTerm), let.InTerm),
        Fix fix when IsVal(ctx, fix.Term) => fix.Term is Abs abs ? TermSubstTop(fix, abs.Body) : throw new NoRulesAppliesException(),
        Fix fix => new Fix(fix.Info, Eval1(ctx, fix.Term)),
        _ => throw new NoRulesAppliesException()
    };

    private static ITerm EvalVar(Context ctx, Var var)
    {
        var b = ctx.GetBinding(var.Index);
        if (b is TermAbbBind tab)
            return tab.Term;
        throw new NoRulesAppliesException();
    }

    private static ITerm ProjRecord(Record rec, string label)
    {
        var field = rec.Fields.FirstOrDefault(f => f.Label == label);
        if (field.Label is null)
            throw new NoRulesAppliesException();
        return field.Term;
    }

    private static ITerm EvalRecord(Context ctx, Record rec)
    {
        var fields = rec.Fields;
        for (int i = 0; i < fields.Count; i++)
        {
            var f = fields[i];
            if (!IsVal(ctx, f.Term))
            {
                var newFields = new List<(string, Variance, ITerm)>(fields);
                newFields[i] = (f.Label, f.Var, Eval1(ctx, f.Term));
                return new Record(rec.Info, newFields);
            }
        }
        throw new NoRulesAppliesException();
    }

    public static ITerm Eval(Context ctx, ITerm t)
    {
        try
        {
            return Eval(ctx, Eval1(ctx, t));
        }
        catch (NoRulesAppliesException)
        {
            return t;
        }
    }

    public static IBinding EvalBinding(Context ctx, IBinding bind) => bind switch
    {
        TermAbbBind tab => new TermAbbBind(Eval(ctx, tab.Term), tab.Type),
        _ => bind
    };
}
