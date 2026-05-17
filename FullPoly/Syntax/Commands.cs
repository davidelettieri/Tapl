using Common;

namespace FullPoly.Syntax;

public sealed record SomeBindCommand(IInfo Info, string TypeVar, string Var, ITerm Term) : ICommand;
