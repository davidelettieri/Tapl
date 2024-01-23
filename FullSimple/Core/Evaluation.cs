using FullSimple.Syntax;
using Common;
using static FullSimple.Core.Shifting;
using static FullSimple.Core.Substitution;
using FullSimple.Syntax.Terms;
using System.Linq;
using System.Collections.Generic;
using FullSimple.Syntax.Bindings;

namespace FullSimple.Core;

public static class Evaluation
{
    public static bool IsNumericVal(Context ctx, ITerm t)
    {
            return t switch
            {
                Zero => true,
                Succ s => IsNumericVal(ctx, s.Of),
                _ => false
            };
        }

    public static bool IsVal(Context ctx, ITerm t)
    {
            return t switch
            {
                True => true,
                False => true,
                Tag tag => IsVal(ctx, tag.Term),
                StringTerm => true,
                Unit => true,
                Float => true,
                Abs => true,
                Record r => r.Fields.All(f => IsVal(ctx, f.Item2)),
                ITerm s when IsNumericVal(ctx, s) => true,
                _ => false
            };
        }

    private static ITerm Eval1(Context ctx, ITerm t)
    {
            return t switch
            {
                If { Condition: True } ift => ift.Then,
                If { Condition: False } ift => ift.Else,
                If ift => new If(ift.Info, Eval1(ctx, ift.Condition), ift.Then, ift.Else),
                Tag tag => new Tag(tag.Info, tag.Label, Eval1(ctx, tag.Term), tag.Type),
                Case { Term: Tag tag } c when IsVal(ctx, tag.Term) => CaseTag(c.Cases, tag.Label, tag.Term),
                Case c => new Case(c.Info, Eval1(ctx, c.Term), c.Cases),
                App { Left: Abs abs } app when IsVal(ctx, app.Right) => TermSubsTop(app.Right, abs.Body),
                App app when IsVal(ctx, app.Left) => new App(app.Info, app.Left, Eval1(ctx, app.Right)),
                App app => new App(app.Info, Eval1(ctx, app.Left), app.Right),
                Let let when IsVal(ctx, let.LetTerm) => TermSubsTop(let.LetTerm, let.InTerm),
                Let let => new Let(let.Info, let.Variable, Eval1(ctx, let.LetTerm), let.InTerm),
                Fix fix when IsVal(ctx, fix.Term) => FixVal(fix.Term, t),
                Fix fix => new Fix(fix.Info, Eval1(ctx, fix.Term)),
                Var var => Var(ctx, var),
                Ascribe ascribe when IsVal(ctx, ascribe.Term) => ascribe.Term,
                Ascribe ascribe => new Ascribe(ascribe.Info, Eval1(ctx, ascribe.Term), ascribe.Type),
                Record rec => Record(ctx, rec),
                Proj { Term: Record rec } proj when IsVal(ctx, rec) => ProjRec(rec.Fields, proj.Label),
                Proj proj => new Proj(proj.Info, Eval1(ctx, proj.Term), proj.Label),
                TimesFloat { Left: Float fl, Right: Float fr } tf => new Float(tf.Info, fl.Value * fr.Value),
                TimesFloat { Left: Float } tf => new TimesFloat(tf.Info, tf.Left, Eval1(ctx, tf.Right)),
                TimesFloat tf => new TimesFloat(tf.Info, Eval1(ctx, tf.Left), tf.Right),
                Succ succ => new Succ(succ.Info, Eval1(ctx, succ.Of)),
                Pred { Of: Zero } => new Zero(new UnknownInfo()),
                Pred { Of: Succ s } when IsNumericVal(ctx, s.Of) => s.Of,
                Pred s => new Pred(s.Info, Eval1(ctx, s.Of)),
                IsZero { Term: Zero } => new True(new UnknownInfo()),
                IsZero { Term: Succ s } when IsNumericVal(ctx, s.Of) => new False(new UnknownInfo()),
                IsZero isZero => new IsZero(isZero.Info, Eval1(ctx, isZero.Term)),
                _ => throw new NoRulesAppliesException()
            };
        }

    private static ITerm ProjRec(List<(string, ITerm)> fields, string label)
    {
            var field = fields.FirstOrDefault(p => p.Item1 == label);

            if (string.IsNullOrWhiteSpace(field.Item1))
                throw new NoRulesAppliesException();

            return field.Item2;
        }

    private static ITerm Record(Context ctx, Record rec)
    {
            var result = new Record(rec.Info, evalAField(rec.Fields).ToList());

            return result;

            IEnumerable<(string, ITerm)> evalAField(IEnumerable<(string, ITerm)> l)
            {
                if (!l.Any())
                    throw new NoRulesAppliesException();

                var first = l.First();

                if (IsVal(ctx, first.Item2))
                    return Enumerable.Repeat(first, 1).Concat(evalAField(l.Skip(1)));

                return Enumerable.Repeat((first.Item1, Eval1(ctx, first.Item2)), 1).Concat(l.Skip(1));
            }
        }

    private static ITerm Var(Context ctx, Var var)
    {
            var b = ctx.GetBinding(var.Index);

            if (b is TermAbbBind tab)
                return tab.Term;

            throw new NoRulesAppliesException();
        }

    private static ITerm FixVal(ITerm fixTerm, ITerm t)
    {
            if (fixTerm is Abs abs)
                return TermSubsTop(t, abs.Body);

            throw new NoRulesAppliesException();
        }

    private static ITerm CaseTag(IEnumerable<(string label, string variable, ITerm term)> cases, string label, ITerm term)
    {
            var c = cases.FirstOrDefault(p => p.label == label);

            if (string.IsNullOrWhiteSpace(c.label))
                throw new NoRulesAppliesException();

            return TermSubsTop(term, c.term);
        }

    public static ITerm Eval(Context ctx, ITerm t)
    {
            try
            {
                var t1 = Eval1(ctx, t);

                return Eval(ctx, t1);
            }
            catch (NoRulesAppliesException)
            {
                return t;
            }
        }


    public static ITerm TermSubsTop(ITerm s, ITerm t)
        => TermShift(-1, TermSubst(0, TermShift(1, s), t));
}