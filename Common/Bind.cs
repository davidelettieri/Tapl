namespace Common;

public record Bind(IInfo Info, string Name, IBinding Binding) : ICommand;