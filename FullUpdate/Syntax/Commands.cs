using Common;

namespace FullUpdate.Syntax;

public sealed record EvalCommand(IInfo Info, ITerm Term) : ICommand;
public sealed record BindCommand(IInfo Info, string Name, IBinding Binding) : ICommand;
public sealed record SomeBindCommand(IInfo Info, string TypeVar, string Var, ITerm Term) : ICommand;
