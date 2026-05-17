using Arith.Terms;
using Common;

namespace Harness.Runner;

internal static class Program
{
    private static readonly StringComparer LanguageComparer = StringComparer.OrdinalIgnoreCase;

    public static int Main(string[] args)
    {
        if (!TryParseArguments(args, out var options, out var error))
        {
            Console.Error.WriteLine(error);
            PrintUsage(Console.Error);
            return 1;
        }

        if (!File.Exists(options.SourceFilePath))
        {
            Console.Error.WriteLine($"Source file not found: {options.SourceFilePath}");
            return 1;
        }

        var source = File.ReadAllText(options.SourceFilePath);
        var originalOut = Console.Out;
        var capture = new StringWriter();

        try
        {
            Console.SetOut(capture);
            RunLanguage(options.Language, source);
        }
        catch (TaplTypingException ex) when (ex.Info is Common.FileInfo c)
        {
            Console.SetOut(originalOut);
            originalOut.WriteLine($"{FormatWorkspacePath(options.SourceFilePath)}:{c.Line}.{c.Column}:");
            originalOut.WriteLine(ex.Message);
            originalOut.WriteLine();
            originalOut.WriteLine();
            return 1;
        }
        catch (Exception ex)
        {
            Console.SetOut(originalOut);
            Console.Error.WriteLine(ex.Message);
            return 1;
        }
        finally
        {
            Console.SetOut(originalOut);
        }

        originalOut.Write(capture.ToString());
        return 0;
    }

    private static bool TryParseArguments(string[] args, out RunnerOptions options, out string error)
    {
        options = default;
        error = string.Empty;

        if (args.Length != 2)
        {
            error = "Expected exactly 2 arguments.";
            return false;
        }

        var language = args[0].Trim();
        var sourceFilePath = Path.GetFullPath(args[1]);

        if (string.IsNullOrWhiteSpace(language))
        {
            error = "Language cannot be empty.";
            return false;
        }

        options = new RunnerOptions(language, sourceFilePath);
        return true;
    }

    private static void PrintUsage(TextWriter writer)
    {
        writer.WriteLine("Usage: dotnet run --project Harness.Runner -- <language> <source-file>");
        writer.WriteLine("Supported languages: arith, simplebool, untyped, letexercise, fullsimple, fulluntyped, fullref, fullerror, fullupdate");
    }

    private static void RunLanguage(string language, string source)
    {
        if (LanguageComparer.Equals(language, "arith"))
        {
            RunArith(source);
            return;
        }

        if (LanguageComparer.Equals(language, "simplebool"))
        {
            SimpleBool.Functions.Process(source);
            return;
        }

        if (LanguageComparer.Equals(language, "untyped"))
        {
            Untyped.Functions.Process(source);
            return;
        }

        if (LanguageComparer.Equals(language, "letexercise"))
        {
            LetExercise.Functions.Process(source);
            return;
        }

        if (LanguageComparer.Equals(language, "fullsimple"))
        {
            FullSimple.Functions.Process(source);
            return;
        }

        if (LanguageComparer.Equals(language, "fulluntyped"))
        {
            FullUntyped.Functions.Process(source);
            return;
        }

        if (LanguageComparer.Equals(language, "fullref"))
        {
            FullRef.Functions.Process(source);
            return;
        }

        if (LanguageComparer.Equals(language, "fullerror"))
        {
            FullError.Functions.Process(source);
            return;
        }

        if (LanguageComparer.Equals(language, "fullupdate"))
        {
            FullUpdate.Functions.Process(source);
            return;
        }

        throw new ArgumentException($"Unsupported language: {language}");
    }

    private static string FormatWorkspacePath(string sourceFilePath)
    {
        var repoRoot = Directory.GetCurrentDirectory();
        var relativePath = Path.GetRelativePath(repoRoot, sourceFilePath);

        if (relativePath.StartsWith(".." + Path.DirectorySeparatorChar, StringComparison.Ordinal)
            || relativePath == "..")
            return sourceFilePath.Replace(Path.DirectorySeparatorChar, '/');

        return "/workspace/" + relativePath.Replace(Path.DirectorySeparatorChar, '/');
    }

    private static void RunArith(string source)
    {
        var normalizedSource = NormalizeArithSource(source);
        var result = Arith.Functions.Eval(Arith.Functions.Parse(normalizedSource));
        Console.WriteLine(PrintArith(result));
    }

    private static string NormalizeArithSource(string source)
    {
        var trimmed = source.Trim();

        if (trimmed.EndsWith(';'))
            trimmed = trimmed[..^1].TrimEnd();

        return trimmed;
    }

    private static string PrintArith(Common.ITerm term)
    {
        if (TryGetNatValue(term, out var natValue))
            return natValue.ToString();

        return term switch
        {
            True => "true",
            False => "false",
            Zero => "0",
            Succ succ => $"succ {ParenthesizeArith(succ.Of)}",
            Pred pred => $"pred {ParenthesizeArith(pred.Of)}",
            IsZero isZero => $"iszero {ParenthesizeArith(isZero.Term)}",
            If ifTerm => $"if {PrintArith(ifTerm.Condition)} then {PrintArith(ifTerm.Then)} else {PrintArith(ifTerm.Else)}",
            _ => throw new InvalidOperationException($"Unsupported arith term: {term.GetType().Name}")
        };
    }

    private static string ParenthesizeArith(Common.ITerm term)
    {
        return term switch
        {
            True or False or Zero => PrintArith(term),
            _ when TryGetNatValue(term, out var natValue) => natValue.ToString(),
            _ => $"({PrintArith(term)})"
        };
    }

    private static bool TryGetNatValue(Common.ITerm term, out int value)
    {
        var current = term;
        var count = 0;

        while (current is Succ succ)
        {
            count++;
            current = succ.Of;
        }

        if (current is Zero)
        {
            value = count;
            return true;
        }

        value = 0;
        return false;
    }

    private readonly record struct RunnerOptions(string Language, string SourceFilePath);
}
