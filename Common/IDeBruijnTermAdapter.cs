using System;

namespace Common;

public interface IDeBruijnTermAdapter<TTerm, TVariable>
{
    TTerm Map(Func<int, TVariable, TTerm> onVar, int c, TTerm term);
    int GetIndex(TVariable variable);
    int GetContextLength(TVariable variable);
    TTerm ToTerm(TVariable variable);
    TTerm CreateShiftedVar(TVariable variable, int shiftedIndex, int shiftedContextLength);
}