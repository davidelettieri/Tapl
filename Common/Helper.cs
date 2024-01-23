using Antlr4.Runtime;

namespace Common;

public static class Helper
{
    public static IInfo GetFileInfo<T>(this T context) where T : ParserRuleContext
        => new FileInfo(context.GetText(), context.Start.Line, context.Start.Column);
}