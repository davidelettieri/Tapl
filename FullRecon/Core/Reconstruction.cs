using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using FullRecon.Syntax;
using FullRecon.Syntax.Bindings;
using FullRecon.Syntax.Terms;

namespace FullRecon.Core;

public static class Reconstruction
{
    // Substitute tyX -> tyT in tyS
    private static IType SubstInType(string tyX, IType tyT, IType tyS) => tyS switch
    {
        TypeArrow ta => new TypeArrow(SubstInType(tyX, tyT, ta.From), SubstInType(tyX, tyT, ta.To)),
        TypeNat => tyS,
        TypeBool => tyS,
        TypeId ti when ti.Name == tyX => tyT,
        TypeId => tyS,
        _ => throw new InvalidOperationException($"Unexpected type: {tyS}")
    };

    // Check whether type variable tyX occurs in tyT
    private static bool OccursIn(string tyX, IType tyT) => tyT switch
    {
        TypeArrow ta => OccursIn(tyX, ta.From) || OccursIn(tyX, ta.To),
        TypeNat => false,
        TypeBool => false,
        TypeId ti => ti.Name == tyX,
        _ => false
    };

    // Substitute tyX -> tyT in all constraint pairs
    private static List<(IType, IType)> SubstInConstr(string tyX, IType tyT, List<(IType, IType)> constr)
        => constr.Select(p => (SubstInType(tyX, tyT, p.Item1), SubstInType(tyX, tyT, p.Item2))).ToList();

    // Apply the substitution (list of (name, type) pairs) to a type, applying in reverse order.
    // Matches OCaml's applysubst which uses List.fold_left on List.rev constr.
    public static IType ApplySubst(List<(string, IType)> subst, IType ty)
    {
        var result = ty;
        // Apply in reverse order (fold_left on List.rev)
        for (var i = subst.Count - 1; i >= 0; i--)
        {
            var (name, replacement) = subst[i];
            result = SubstInType(name, replacement, result);
        }
        return result;
    }

    // Unify: solve constraints. Returns substitution as list of (varName, type) pairs.
    // Matches OCaml's unify function.
    public static List<(string, IType)> Unify(IInfo fi, List<(IType, IType)> constr)
    {
        return UnifyHelper(fi, constr);
    }

    private static List<(string, IType)> UnifyHelper(IInfo fi, List<(IType, IType)> constr)
    {
        if (constr.Count == 0)
            return new List<(string, IType)>();

        var (tyS, tyT) = constr[0];
        var rest = constr.Skip(1).ToList();

        // Case: (tyS, TyId(tyX)) - second is a type variable
        if (tyT is TypeId { Name: var tyX1 })
        {
            if (tyS is TypeId { Name: var tyS1 } && tyS1 == tyX1)
                return UnifyHelper(fi, rest);

            if (OccursIn(tyX1, tyS))
                throw new TaplTypingException(fi, "Could not simplify constraints: circular constraints");

            var substituted = SubstInConstr(tyX1, tyS, rest);
            var result = UnifyHelper(fi, substituted);
            result.Add((tyX1, tyS));
            return result;
        }

        // Case: (TyId(tyX), tyT) - first is a type variable
        if (tyS is TypeId { Name: var tyX2 })
        {
            if (tyT is TypeId { Name: var tyT2 } && tyT2 == tyX2)
                return UnifyHelper(fi, rest);

            if (OccursIn(tyX2, tyT))
                throw new TaplTypingException(fi, "Could not simplify constraints: circular constraints");

            var substituted = SubstInConstr(tyX2, tyT, rest);
            var result = UnifyHelper(fi, substituted);
            result.Add((tyX2, tyT));
            return result;
        }

        // Case: (TyNat, TyNat)
        if (tyS is TypeNat && tyT is TypeNat)
            return UnifyHelper(fi, rest);

        // Case: (TyBool, TyBool)
        if (tyS is TypeBool && tyT is TypeBool)
            return UnifyHelper(fi, rest);

        // Case: (TyArr(s1, s2), TyArr(t1, t2))
        if (tyS is TypeArrow ta1 && tyT is TypeArrow ta2)
        {
            var expanded = new List<(IType, IType)> { (ta1.From, ta2.From), (ta1.To, ta2.To) };
            expanded.AddRange(rest);
            return UnifyHelper(fi, expanded);
        }

        throw new TaplTypingException(fi, "Unsolvable constraints");
    }

    // Type reconstruction: generates constraints and returns the reconstructed type.
    // nextUVar is an int counter for generating fresh type variable names (?X_0, ?X_1, ...).
    // Returns (type, updatedNextUVar, constraints).
    public static (IType Type, int NextUVar, List<(IType, IType)> Constraints) Recon(
        Context ctx, int nextUVar, ITerm t)
    {
        switch (t)
        {
            case Var var:
            {
                var tyT = ctx.GetTypeFromContext(var.Index);
                return (tyT, nextUVar, new List<(IType, IType)>());
            }

            case Abs abs when abs.Type is not null:
            {
                var ctx1 = ctx.AddBinding(abs.V, new VarBind(abs.Type));
                var (tyT2, nv2, c2) = Recon(ctx1, nextUVar, abs.Body);
                return (new TypeArrow(abs.Type, tyT2), nv2, c2);
            }

            case Abs abs:
            {
                // Unannotated lambda: generate fresh type variable for parameter
                var uName = $"?X{nextUVar}";
                var nextUVar0 = nextUVar + 1;
                var tyX = new TypeId(uName);
                var ctx1 = ctx.AddBinding(abs.V, new VarBind(tyX));
                var (tyT2, nv2, c2) = Recon(ctx1, nextUVar0, abs.Body);
                return (new TypeArrow(tyX, tyT2), nv2, c2);
            }

            case App app:
            {
                var (tyT1, nv1, c1) = Recon(ctx, nextUVar, app.Left);
                var (tyT2, nv2, c2) = Recon(ctx, nv1, app.Right);
                var uName = $"?X{nv2}";
                var nextUVar2 = nv2 + 1;
                var newConstr = new List<(IType, IType)> { (tyT1, new TypeArrow(tyT2, new TypeId(uName))) };
                var combined = new List<(IType, IType)>();
                combined.AddRange(newConstr);
                combined.AddRange(c1);
                combined.AddRange(c2);
                return (new TypeId(uName), nextUVar2, combined);
            }

            case Let let when !Evaluation.IsVal(ctx, let.LetTerm):
            {
                var (tyT1, nv1, c1) = Recon(ctx, nextUVar, let.LetTerm);
                var ctx1 = ctx.AddBinding(let.Variable, new VarBind(tyT1));
                var (tyT2, nv2, c2) = Recon(ctx1, nv1, let.InTerm);
                var combined = new List<(IType, IType)>();
                combined.AddRange(c1);
                combined.AddRange(c2);
                return (tyT2, nv2, combined);
            }

            case Let let:
            {
                // Value case: substitute directly
                var substituted = Substitution.TermSubstTop(let.LetTerm, let.InTerm);
                return Recon(ctx, nextUVar, substituted);
            }

            case Zero:
                return (new TypeNat(), nextUVar, new List<(IType, IType)>());

            case Succ s:
            {
                var (tyT1, nv1, c1) = Recon(ctx, nextUVar, s.Of);
                var newConstr = new List<(IType, IType)> { (tyT1, new TypeNat()) };
                newConstr.AddRange(c1);
                return (new TypeNat(), nv1, newConstr);
            }

            case Pred p:
            {
                var (tyT1, nv1, c1) = Recon(ctx, nextUVar, p.Of);
                var newConstr = new List<(IType, IType)> { (tyT1, new TypeNat()) };
                newConstr.AddRange(c1);
                return (new TypeNat(), nv1, newConstr);
            }

            case IsZero iz:
            {
                var (tyT1, nv1, c1) = Recon(ctx, nextUVar, iz.Term);
                var newConstr = new List<(IType, IType)> { (tyT1, new TypeNat()) };
                newConstr.AddRange(c1);
                return (new TypeBool(), nv1, newConstr);
            }

            case True:
                return (new TypeBool(), nextUVar, new List<(IType, IType)>());

            case False:
                return (new TypeBool(), nextUVar, new List<(IType, IType)>());

            case If ift:
            {
                var (tyT1, nv1, c1) = Recon(ctx, nextUVar, ift.Condition);
                var (tyT2, nv2, c2) = Recon(ctx, nv1, ift.Then);
                var (tyT3, nv3, c3) = Recon(ctx, nv2, ift.Else);
                var newConstr = new List<(IType, IType)> { (tyT1, new TypeBool()), (tyT2, tyT3) };
                var combined = new List<(IType, IType)>();
                combined.AddRange(newConstr);
                combined.AddRange(c1);
                combined.AddRange(c2);
                combined.AddRange(c3);
                return (tyT3, nv3, combined);
            }

            default:
                throw new InvalidOperationException($"Unexpected term: {t}");
        }
    }
}
