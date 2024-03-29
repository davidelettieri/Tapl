﻿using SimpleBool.Syntax;
using Common;
using static SimpleBool.Core.Shifting;

namespace SimpleBool.Core;

public static class Substitution
{
    public static ITerm TermSubst(int j, ITerm s, ITerm t)
    {
        ITerm F(int j, Var v) => v.Index == j ? TermShift(j, s) : v;

        return TmMap(F, j, t);
    }

    // public static ITerm TermSubsTop(ITerm s, ITerm t)
    //     => TermShift(-1, TermSubst(0, TermShift(1, s), t));
}