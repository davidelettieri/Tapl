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

    [Fact(DisplayName = "Process prints application argument abstraction in parentheses")]
    public void ProcessPrintsApplicationArgumentAbstractionInParentheses()
    {
        var output = CaptureOutput(() => Functions.Process("lambda _. (lambda x. x) (lambda y. y);"));
        var lines = output.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

        Assert.Equal("(lambda _. (lambda x.x) (lambda y.y))", lines[0]);
    }

    [Fact(DisplayName = "Process prints floats with trailing dot")]
    public void ProcessPrintsFloatsWithTrailingDot()
    {
        var output = CaptureOutput(() => Functions.Process("timesfloat 2.0 3.0;"));
        var lines = output.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

        Assert.Equal("6.", lines[0]);
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
