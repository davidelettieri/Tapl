namespace Common;

public record FileInfo(string Text, int Line, int Column) : IInfo;