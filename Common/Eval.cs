namespace Common;

public record Eval(IInfo Info, ITerm Term) : ICommand;