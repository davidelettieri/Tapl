using System;
using System.IO;
using Xunit;

namespace FullUntyped.Tests;

public class ProcessTests
{
    [Fact(DisplayName = "Process supports untyped lambda syntax")]
    public void ProcessSupportsUntypedLambdaSyntax()
    {
        var output = CaptureOutput(() => Functions.Process("lambda x. x;"));

        Assert.Contains("lambda x.", output);
        Assert.DoesNotContain("lambda x:", output);
    }

    [Fact(DisplayName = "Process supports slash binding syntax")]
    public void ProcessSupportsSlashBindingSyntax()
    {
        var output = CaptureOutput(() => Functions.Process("x /; x;"));
        var lines = output.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

        Assert.True(lines.Length >= 2);
        Assert.Equal("x", lines[0]);
        Assert.Equal("x", lines[1]);
    }

    private static string CaptureOutput(Action action)
    {
        var originalOut = Console.Out;
        var writer = new StringWriter();
        try
        {
            Console.SetOut(writer);
            action();
            return writer.ToString();
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }
}
