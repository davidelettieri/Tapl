using System;
using System.Collections.Generic;
using System.IO;
using Common;
using FullUpdate;
using FullUpdate.Core;
using FullUpdate.Syntax;
using FullUpdate.Syntax.Bindings;
using FullUpdate.Syntax.Terms;
using Xunit;

namespace FullUpdate.Tests;

public class ShiftingTests
{
    [Fact(DisplayName = "TypeShift increments free TypeVar index")]
    public void TypeShiftFreeVar()
    {
        var ty = new TypeVar(0, 1);
        var shifted = Shifting.TypeShift(1, ty);
        var result = Assert.IsType<TypeVar>(shifted);
        Assert.Equal(1, result.X);
        Assert.Equal(2, result.N);
    }

    [Fact(DisplayName = "TypeShift does not affect bound TypeVar")]
    public void TypeShiftBoundVar()
    {
        var ty = new TypeVar(0, 1);
        // Shifting at cutoff 1 means var 0 is bound
        var shifted = Shifting.TypeShiftAbove(1, 1, ty);
        var result = Assert.IsType<TypeVar>(shifted);
        Assert.Equal(0, result.X);
        Assert.Equal(2, result.N);
    }

    [Fact(DisplayName = "TermShift traverses Abs term")]
    public void TermShiftAbs()
    {
        var info = new UnknownInfo();
        var body = new Var(info, 0, 1);
        var abs = new Abs(info, "x", new TypeBool(), body);

        var shifted = Shifting.TermShift(1, abs);

        var result = Assert.IsType<Abs>(shifted);
        Assert.IsType<TypeBool>(result.Type);
        var v = Assert.IsType<Var>(result.Body);
        Assert.Equal(0, v.Index); // bound variable unchanged
        Assert.Equal(2, v.ContextLength);
    }

    [Fact(DisplayName = "TypeSubstTop replaces TypeVar(0)")]
    public void TypeSubstTop()
    {
        var param = new TypeVar(0, 1);
        var body = new TypeArrow(param, new TypeBool());
        // body is T0 -> Bool
        var result = Substitution.TypeSubstTop(new TypeNat(), body);
        // should be Nat -> Bool
        var arrow = Assert.IsType<TypeArrow>(result);
        Assert.IsType<TypeNat>(arrow.From);
        Assert.IsType<TypeBool>(arrow.To);
    }
}

public class EvaluationTests
{
    private static readonly Context EmptyCtx = new Context();

    [Fact(DisplayName = "Boolean identity evaluates correctly")]
    public void BoolIdentity()
    {
        var info = new UnknownInfo();
        var abs = new Abs(info, "x", new TypeBool(), new Var(info, 0, 1));
        var app = new App(info, abs, new True(info));
        var result = Evaluation.Eval(EmptyCtx, app);
        Assert.IsType<True>(result);
    }

    [Fact(DisplayName = "If-true reduces to then branch")]
    public void IfTrue()
    {
        var info = new UnknownInfo();
        var t = new If(info, new True(info), new False(info), new True(info));
        var result = Evaluation.Eval(EmptyCtx, t);
        Assert.IsType<False>(result);
    }

    [Fact(DisplayName = "Record projection evaluates correctly")]
    public void RecordProjection()
    {
        var info = new UnknownInfo();
        var rec = new FullUpdate.Syntax.Terms.Record(info, new List<(string, Variance, ITerm)>
        {
            ("x", Variance.Covariant, new True(info))
        });
        var proj = new Proj(info, rec, "x");
        var result = Evaluation.Eval(EmptyCtx, proj);
        Assert.IsType<True>(result);
    }
}

public class TypingTests
{
    private static readonly Context EmptyCtx = new Context();

    [Fact(DisplayName = "Bool literal has type Bool")]
    public void BoolLiteralType()
    {
        var info = new UnknownInfo();
        var ty = Typing.TypeOf(EmptyCtx, new True(info));
        Assert.IsType<TypeBool>(ty);
    }

    [Fact(DisplayName = "Abs has arrow type")]
    public void AbsType()
    {
        var info = new UnknownInfo();
        var abs = new Abs(info, "x", new TypeBool(), new Var(info, 0, 1));
        var ty = Typing.TypeOf(EmptyCtx, abs);
        var arrow = Assert.IsType<TypeArrow>(ty);
        Assert.IsType<TypeBool>(arrow.From);
        Assert.IsType<TypeBool>(arrow.To);
    }

    [Fact(DisplayName = "TypeAll introduces universal quantifier")]
    public void TAbsType()
    {
        var info = new UnknownInfo();
        // lambda X<:Top. lambda x:X. x  -- identity for any X
        var body = new Abs(info, "x", new TypeVar(0, 1), new Var(info, 0, 2));
        var tabs = new TAbs(info, "X", new TypeTop(), body);
        var ty = Typing.TypeOf(EmptyCtx, tabs);
        var all = Assert.IsType<TypeAll>(ty);
        Assert.Equal("X", all.Name);
        Assert.IsType<TypeTop>(all.Bound);
    }

    [Fact(DisplayName = "KindOf on KnStar type returns KnStar")]
    public void KindOfStar()
    {
        var k = Typing.KindOf(EmptyCtx, new TypeBool());
        Assert.IsType<KnStar>(k);
    }

    [Fact(DisplayName = "Subtype: Bool <: Top")]
    public void SubtypeTop()
    {
        Assert.True(Typing.Subtype(EmptyCtx, new TypeBool(), new TypeTop()));
    }
}

public class ProcessTests
{
    private static string Capture(Action action)
    {
        var sw = new StringWriter();
        var old = Console.Out;
        Console.SetOut(sw);
        try { action(); }
        finally { Console.SetOut(old); }
        return sw.ToString();
    }

    [Fact(DisplayName = "Process simple boolean identity")]
    public void ProcessBoolId()
    {
        var output = Capture(() => Functions.Process("lambda x:Bool. x;"));
        Assert.Contains("lambda x:Bool. x", output);
        Assert.Contains("Bool -> Bool", output);
    }

    [Fact(DisplayName = "Process universal type identity")]
    public void ProcessUniversalId()
    {
        var output = Capture(() => Functions.Process("lambda X. lambda x:X. x;"));
        Assert.Contains("X", output);
    }

    [Fact(DisplayName = "Process record term")]
    public void ProcessRecord()
    {
        var output = Capture(() => Functions.Process("{x=true, y=false};"));
        Assert.Contains("{", output);
        Assert.Contains("x", output);
    }

    [Fact(DisplayName = "Nat succ 0 prints as 1")]
    public void NatSuccPrintsOne()
    {
        var output = Capture(() => Functions.Process("succ 0;"));
        Assert.Contains("1 : Nat", output);
    }

    [Fact(DisplayName = "Nat succ (succ 0) prints as 2")]
    public void NatSuccSuccPrintsTwo()
    {
        var output = Capture(() => Functions.Process("succ (succ 0);"));
        Assert.Contains("2 : Nat", output);
    }

    [Fact(DisplayName = "Record update evaluates new value")]
    public void RecordUpdate()
    {
        var output = Capture(() => Functions.Process("{x=true, #y=0} <- y = succ 0;"));
        Assert.Contains("{x=true, #y=1}", output);
    }

    [Fact(DisplayName = "Higher-kinded type variable prints with :: annotation")]
    public void HigherKindedTypeVar()
    {
        var output = Capture(() => Functions.Process("lambda X::*=>*. lambda x:(X Bool). x;"));
        Assert.Contains("X::*=>*", output);
        Assert.Contains("All X::*=>*", output);
    }

    [Fact(DisplayName = "Valid unpack works")]
    public void ValidUnpack()
    {
        var output = Capture(() => Functions.Process("let {X,x} = {*Bool, true} as {Some X, X} in true;"));
        Assert.Contains("true : Bool", output);
    }

    [Fact(DisplayName = "Scope extrusion throws typing exception")]
    public void ScopeExtrusion()
    {
        Assert.ThrowsAny<Exception>(() =>
            Capture(() => Functions.Process("let {X,x} = {*Bool, true} as {Some X, X} in x;")));
    }

    [Fact(DisplayName = "Invariant-invariant subtype requires equal types")]
    public void InvariantInvariantSubtype()
    {
        // {#x:Bool} should NOT be a subtype of {#x:Top} (invariant requires exact equality)
        var ctx = new Context();
        var r1 = new TypeRecord(new List<(string, Variance, IType)> { ("x", Variance.Invariant, new TypeBool()) });
        var r2 = new TypeRecord(new List<(string, Variance, IType)> { ("x", Variance.Invariant, new TypeTop()) });
        Assert.False(Typing.Subtype(ctx, r1, r2));
    }

    [Fact(DisplayName = "Invariant-covariant subtype allows covariant relaxation")]
    public void InvariantCovariantSubtype()
    {
        // {#x:Bool} IS a subtype of {x:Top} (invariant satisfies covariant read)
        var ctx = new Context();
        var r1 = new TypeRecord(new List<(string, Variance, IType)> { ("x", Variance.Invariant, new TypeBool()) });
        var r2 = new TypeRecord(new List<(string, Variance, IType)> { ("x", Variance.Covariant, new TypeTop()) });
        Assert.True(Typing.Subtype(ctx, r1, r2));
    }
}
