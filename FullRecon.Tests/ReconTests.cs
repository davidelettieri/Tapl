using System.Collections.Generic;
using Common;
using FullRecon.Core;
using FullRecon.Syntax;
using FullRecon.Syntax.Terms;
using Xunit;

namespace FullRecon.Tests;

public class ReconTests
{
    private static Context EmptyCtx => new Context();

    [Fact(DisplayName = "Recon returns Bool for TmTrue")]
    public void ReconTrueReturnsBool()
    {
        var info = new UnknownInfo();
        var term = new True(info);
        var (ty, nv, c) = Reconstruction.Recon(EmptyCtx, 0, term);
        Assert.IsType<TypeBool>(ty);
        Assert.Equal(0, nv);
        Assert.Empty(c);
    }

    [Fact(DisplayName = "Recon returns Bool for TmFalse")]
    public void ReconFalseReturnsBool()
    {
        var info = new UnknownInfo();
        var term = new False(info);
        var (ty, nv, c) = Reconstruction.Recon(EmptyCtx, 0, term);
        Assert.IsType<TypeBool>(ty);
        Assert.Equal(0, nv);
        Assert.Empty(c);
    }

    [Fact(DisplayName = "Recon returns Nat for TmZero")]
    public void ReconZeroReturnsNat()
    {
        var info = new UnknownInfo();
        var term = new Zero(info);
        var (ty, nv, c) = Reconstruction.Recon(EmptyCtx, 0, term);
        Assert.IsType<TypeNat>(ty);
        Assert.Equal(0, nv);
        Assert.Empty(c);
    }

    [Fact(DisplayName = "Recon returns Nat->Nat for typed Nat abs")]
    public void ReconTypedAbsReturnsArrow()
    {
        var info = new UnknownInfo();
        var term = new Abs(info, "x", new TypeNat(), new Var(info, 0, 1));
        var (ty, nv, c) = Reconstruction.Recon(EmptyCtx, 0, term);
        var arrow = Assert.IsType<TypeArrow>(ty);
        Assert.IsType<TypeNat>(arrow.From);
        Assert.IsType<TypeNat>(arrow.To);
        Assert.Equal(0, nv);
        Assert.Empty(c);
    }

    [Fact(DisplayName = "Recon for untyped abs generates fresh type var")]
    public void ReconUntypedAbsGeneratesFreshTypeVar()
    {
        var info = new UnknownInfo();
        var term = new Abs(info, "x", null, new Var(info, 0, 1));
        var (ty, nv, c) = Reconstruction.Recon(EmptyCtx, 0, term);
        var arrow = Assert.IsType<TypeArrow>(ty);
        var from = Assert.IsType<TypeId>(arrow.From);
        Assert.Equal("?X0", from.Name);
        var to = Assert.IsType<TypeId>(arrow.To);
        Assert.Equal("?X0", to.Name);
        Assert.Equal(1, nv);
        Assert.Empty(c);
    }

    [Fact(DisplayName = "Recon for app generates fresh constraint")]
    public void ReconAppGeneratesConstraint()
    {
        var info = new UnknownInfo();
        // lambda x:Bool->Bool. x true
        var ctx = EmptyCtx.AddBinding("x", new FullRecon.Syntax.Bindings.VarBind(new TypeArrow(new TypeBool(), new TypeBool())));
        var t1 = new Var(info, 0, 1);
        var t2 = new True(info);
        var app = new App(info, t1, t2);
        var (ty, nv, c) = Reconstruction.Recon(ctx, 0, app);
        var resultTy = Assert.IsType<TypeId>(ty);
        Assert.Equal("?X0", resultTy.Name);
        Assert.Equal(1, nv);
        Assert.Single(c);
        // constraint: (Bool->Bool) = (Bool -> ?X_0)
        var (left, right) = c[0];
        Assert.IsType<TypeArrow>(left);
        var rightArrow = Assert.IsType<TypeArrow>(right);
        Assert.IsType<TypeBool>(rightArrow.From);
        Assert.IsType<TypeId>(rightArrow.To);
    }

    [Fact(DisplayName = "Unify solves simple type variable constraint")]
    public void UnifySolvesSimpleConstraint()
    {
        var info = new UnknownInfo();
        var constr = new List<(IType, IType)>
        {
            (new TypeId("?X0"), new TypeBool())
        };
        var subst = Reconstruction.Unify(info, constr);
        Assert.Single(subst);
        Assert.Equal("?X0", subst[0].Item1);
        Assert.IsType<TypeBool>(subst[0].Item2);
    }

    [Fact(DisplayName = "ApplySubst replaces type variable")]
    public void ApplySubstReplacesTypeVar()
    {
        var subst = new List<(string, IType)> { ("?X0", new TypeBool()) };
        var ty = new TypeArrow(new TypeId("?X0"), new TypeNat());
        var result = Reconstruction.ApplySubst(subst, ty);
        var arrow = Assert.IsType<TypeArrow>(result);
        Assert.IsType<TypeBool>(arrow.From);
        Assert.IsType<TypeNat>(arrow.To);
    }
}
