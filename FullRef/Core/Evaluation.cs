using System.Collections.Generic;
using System.Linq;
using Common;
using FullRef.Syntax;
using FullRef.Syntax.Bindings;
using FullRef.Syntax.Terms;
using static FullRef.Core.Substitution;

namespace FullRef.Core;

public static class Evaluation
{
    private static bool IsNumericVal(Context ctx, ITerm term) => term switch
    {
        Zero => true,
        Succ succ => IsNumericVal(ctx, succ.Of),
        _ => false
    };

    private static bool IsVal(Context ctx, ITerm term) => term switch
    {
        StringTerm => true,
        Unit => true,
        Loc => true,
        Tag tag => IsVal(ctx, tag.Term),
        True => true,
        False => true,
        Float => true,
        Abs => true,
        Record record => record.Fields.All(field => IsVal(ctx, field.Item2)),
        _ when IsNumericVal(ctx, term) => true,
        _ => false
    };

    public static (ITerm Term, Store Store) Eval(Context ctx, Store store, ITerm term)
    {
        try
        {
            var (nextTerm, nextStore) = Eval1(ctx, store, term);
            return Eval(ctx, nextStore, nextTerm);
        }
        catch (NoRulesAppliesException)
        {
            return (term, store);
        }
    }

    public static ITerm Eval(Context ctx, ITerm term) => Eval(ctx, Store.Empty, term).Term;

    public static (IBinding Binding, Store Store) EvalBinding(Context ctx, Store store, IBinding binding)
        => binding switch
        {
            TermAbbBind termAbbBind => EvalTermBinding(ctx, store, termAbbBind),
            _ => (binding, store)
        };

    private static (IBinding Binding, Store Store) EvalTermBinding(Context ctx, Store store, TermAbbBind binding)
    {
        var (term, nextStore) = Eval(ctx, store, binding.Term);
        return (new TermAbbBind(term, binding.Type), nextStore);
    }

    private static (ITerm Term, Store Store) Eval1(Context ctx, Store store, ITerm term)
    {
        switch (term)
        {
            case App { Left: Abs abs, Right: var right } when IsVal(ctx, right):
                return (TermSubsTop(right, abs.Body), store);
            case App app when IsVal(ctx, app.Left):
            {
                var (nextRight, nextStore) = Eval1(ctx, store, app.Right);
                return (new App(app.Info, app.Left, nextRight), nextStore);
            }
            case App app:
            {
                var (nextLeft, nextStore) = Eval1(ctx, store, app.Left);
                return (new App(app.Info, nextLeft, app.Right), nextStore);
            }
            case Ascribe ascribe when IsVal(ctx, ascribe.Term):
                return (ascribe.Term, store);
            case Ascribe ascribe:
            {
                var (nextTerm, nextStore) = Eval1(ctx, store, ascribe.Term);
                return (new Ascribe(ascribe.Info, nextTerm, ascribe.Type), nextStore);
            }
            case Ref reference when !IsVal(ctx, reference.Term):
            {
                var (nextTerm, nextStore) = Eval1(ctx, store, reference.Term);
                return (new Ref(reference.Info, nextTerm), nextStore);
            }
            case Ref reference:
            {
                var (location, nextStore) = store.Extend(reference.Term);
                return (new Loc(new UnknownInfo(), location), nextStore);
            }
            case Deref deref when !IsVal(ctx, deref.Term):
            {
                var (nextTerm, nextStore) = Eval1(ctx, store, deref.Term);
                return (new Deref(deref.Info, nextTerm), nextStore);
            }
            case Deref { Term: Loc loc }:
                return (store.Lookup(loc.Location), store);
            case Assign assign when !IsVal(ctx, assign.Left):
            {
                var (nextLeft, nextStore) = Eval1(ctx, store, assign.Left);
                return (new Assign(assign.Info, nextLeft, assign.Right), nextStore);
            }
            case Assign assign when !IsVal(ctx, assign.Right):
            {
                var (nextRight, nextStore) = Eval1(ctx, store, assign.Right);
                return (new Assign(assign.Info, assign.Left, nextRight), nextStore);
            }
            case Assign { Left: Loc loc, Right: var right }:
                return (new Unit(new UnknownInfo()), store.Update(loc.Location, right));
            case Tag tag:
            {
                var (nextTerm, nextStore) = Eval1(ctx, store, tag.Term);
                return (new Tag(tag.Info, tag.Label, nextTerm, tag.Type), nextStore);
            }
            case Case { Term: Tag tag, Cases: var cases } when IsVal(ctx, tag.Term):
                return (CaseTag(cases, tag.Label, tag.Term), store);
            case Case @case:
            {
                var (nextTerm, nextStore) = Eval1(ctx, store, @case.Term);
                return (new Case(@case.Info, nextTerm, @case.Cases), nextStore);
            }
            case Let { LetTerm: var letTerm, InTerm: var inTerm } when IsVal(ctx, letTerm):
                return (TermSubsTop(letTerm, inTerm), store);
            case Let letTerm:
            {
                var (nextTerm, nextStore) = Eval1(ctx, store, letTerm.LetTerm);
                return (new Let(letTerm.Info, letTerm.Variable, nextTerm, letTerm.InTerm), nextStore);
            }
            case Fix fix when IsVal(ctx, fix.Term):
                return fix.Term switch
                {
                    Abs abs => (TermSubsTop(fix, abs.Body), store),
                    _ => throw new NoRulesAppliesException()
                };
            case Fix fix:
            {
                var (nextTerm, nextStore) = Eval1(ctx, store, fix.Term);
                return (new Fix(fix.Info, nextTerm), nextStore);
            }
            case If { Condition: True, Then: var thenTerm }:
                return (thenTerm, store);
            case If { Condition: False, Else: var elseTerm }:
                return (elseTerm, store);
            case If ifTerm:
            {
                var (nextCondition, nextStore) = Eval1(ctx, store, ifTerm.Condition);
                return (new If(ifTerm.Info, nextCondition, ifTerm.Then, ifTerm.Else), nextStore);
            }
            case TimesFloat { Left: Float left, Right: Float right } timesFloat:
                return (new Float(timesFloat.Info, left.Value * right.Value), store);
            case TimesFloat { Left: Float } timesFloat:
            {
                var (nextRight, nextStore) = Eval1(ctx, store, timesFloat.Right);
                return (new TimesFloat(timesFloat.Info, timesFloat.Left, nextRight), nextStore);
            }
            case TimesFloat timesFloat:
            {
                var (nextLeft, nextStore) = Eval1(ctx, store, timesFloat.Left);
                return (new TimesFloat(timesFloat.Info, nextLeft, timesFloat.Right), nextStore);
            }
            case Var variable:
                return ctx.GetBinding(variable.Index) switch
                {
                    TermAbbBind termAbbBind => (termAbbBind.Term, store),
                    _ => throw new NoRulesAppliesException()
                };
            case Record record:
                return EvalRecord(ctx, store, record);
            case Proj { Term: Record record, Label: var label } when IsVal(ctx, record):
            {
                var field = record.Fields.FirstOrDefault(entry => entry.Item1 == label);
                return field.Item1 is null
                    ? throw new NoRulesAppliesException()
                    : (field.Item2, store);
            }
            case Proj proj:
            {
                var (nextTerm, nextStore) = Eval1(ctx, store, proj.Term);
                return (new Proj(proj.Info, nextTerm, proj.Label), nextStore);
            }
            case Succ succ:
            {
                var (nextTerm, nextStore) = Eval1(ctx, store, succ.Of);
                return (new Succ(succ.Info, nextTerm), nextStore);
            }
            case Pred { Of: Zero }:
                return (new Zero(new UnknownInfo()), store);
            case Pred { Of: Succ succ } when IsNumericVal(ctx, succ.Of):
                return (succ.Of, store);
            case Pred pred:
            {
                var (nextTerm, nextStore) = Eval1(ctx, store, pred.Of);
                return (new Pred(pred.Info, nextTerm), nextStore);
            }
            case IsZero { Term: Zero }:
                return (new True(new UnknownInfo()), store);
            case IsZero { Term: Succ succ } when IsNumericVal(ctx, succ.Of):
                return (new False(new UnknownInfo()), store);
            case IsZero isZero:
            {
                var (nextTerm, nextStore) = Eval1(ctx, store, isZero.Term);
                return (new IsZero(isZero.Info, nextTerm), nextStore);
            }
            default:
                throw new NoRulesAppliesException();
        }
    }

    private static ITerm CaseTag(IEnumerable<(string label, string variable, ITerm term)> cases, string label, ITerm term)
    {
        var @case = cases.FirstOrDefault(entry => entry.label == label);
        return @case.label is null
            ? throw new NoRulesAppliesException()
            : TermSubsTop(term, @case.term);
    }

    private static (ITerm Term, Store Store) EvalRecord(Context ctx, Store store, Record record)
    {
        if (record.Fields.Count == 0)
            throw new NoRulesAppliesException();

        for (var index = 0; index < record.Fields.Count; index++)
        {
            var (label, fieldTerm) = record.Fields[index];
            if (IsVal(ctx, fieldTerm))
                continue;

            var (nextField, nextStore) = Eval1(ctx, store, fieldTerm);
            var nextFields = record.Fields.ToList();
            nextFields[index] = (label, nextField);
            return (new Record(record.Info, nextFields), nextStore);
        }

        throw new NoRulesAppliesException();
    }
}