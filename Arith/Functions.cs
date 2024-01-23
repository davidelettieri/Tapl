using Antlr4.Runtime;
using Arith.Terms;
using Common;
using System;

namespace Arith;

public static class Functions
{
    private static bool IsNumericalVal(ITerm t)
    {
        return t switch
        {
            Zero => true,
            Succ s => IsNumericalVal(s.Of),
            _ => false
        };
    }

    private static ITerm _eval(ITerm t)
    {
        return t switch
        {
            If { Condition: True } i => i.Then,
            If { Condition: False } i => i.Else,
            If i => _eval(new If(_eval(i.Condition), i.Then, i.Else)),
            Succ s => new Succ(_eval(s.Of)),
            Pred { Of: Zero z } => z,
            Pred { Of: Succ s } when IsNumericalVal(s.Of) => s.Of,
            Pred s => new Pred(_eval(s.Of)),
            IsZero { Term: Zero } => new True(),
            IsZero { Term: Succ s } when IsNumericalVal(s.Of) => new False(),
            IsZero iz => new IsZero(_eval(iz.Term)),
            _ => throw new NoRulesAppliesException()
        };
    }

    public static ITerm Eval(ITerm t)
    {
        try
        {
            var t1 = _eval(t);

            return Eval(t1);
        }
        catch (NoRulesAppliesException)
        {
            return t;
        }
    }

    public static ITerm Parse(string s)
    {
        if (string.IsNullOrWhiteSpace(s))
            throw new ArgumentException($"{nameof(s)} cannot be null or empty");


        var inputStream = new AntlrInputStream(s);
        var lexer = new TaplLexer(inputStream);
        var commonTokenStream = new CommonTokenStream(lexer);
        var parser = new TaplParser(commonTokenStream);
        var context = parser.term();

        var visitor = new TermVisitor();

        return visitor.Visit(context);
    }
}