using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Antlr4.Runtime;
using Common;
using FullRecon.Core;
using FullRecon.Syntax;
using FullRecon.Syntax.Bindings;
using FullRecon.Visitors;
using static FullRecon.Core.Evaluation;
using static FullRecon.Core.Reconstruction;
using static FullRecon.Syntax.Printing;

namespace FullRecon;

public static class Functions
{
    public static Func<Context, (ImmutableStack<ICommand>, Context)> Parse(string s)
    {
        if (string.IsNullOrWhiteSpace(s))
            throw new ArgumentException($"{nameof(s)} cannot be null or empty");

        var inputStream = new AntlrInputStream(s);
        var lexer = new FullReconLexer(inputStream);
        var commonTokenStream = new CommonTokenStream(lexer);
        var parser = new FullReconParser(commonTokenStream);
        var context = parser.toplevel();

        var visitor = new TopLevelVisitor();
        return visitor.Visit(context);
    }

    // fullrecon accumulates constraints and a fresh-var counter across commands.
    // State: (ctx, nextUVar, constraints)
    // The constraints from Recon are combined with previously accumulated constraints
    // and re-unified on each Eval command.
    public static Context Process(string source)
    {
        if (string.IsNullOrWhiteSpace(source))
            throw new ArgumentException($"{nameof(source)} cannot be null or empty");

        var fcommands = Parse(source);
        var (commands, _) = fcommands(new Context());

        var ctx = new Context();
        var nextUVar = 0;
        var constr = new List<(IType, IType)>();

        foreach (var command in commands)
        {
            (ctx, nextUVar, constr) = ProcessCommand(ctx, nextUVar, constr, command);
        }

        return ctx;
    }

    private static (Context, int, List<(IType, IType)>) ProcessCommand(
        Context ctx,
        int nextUVar,
        List<(IType, IType)> constr,
        ICommand cmd)
    {
        switch (cmd)
        {
            case Eval e:
            {
                var (tyT, newNextUVar, constrT) = Recon(ctx, nextUVar, e.Term);
                var t = Eval(ctx, e.Term);

                // Combine accumulated constraints with newly generated ones
                var combined = new List<(IType, IType)>();
                combined.AddRange(constr);
                combined.AddRange(constrT);

                var solved = Unify(e.Info, combined);

                PrintTmATermWithType(ctx, t, ApplySubst(solved, tyT));

                // Convert solved substitution back to (IType, IType) list for accumulation
                var newConstr = new List<(IType, IType)>();
                foreach (var (name, ty) in solved)
                    newConstr.Add((new TypeId(name), ty));

                return (ctx, newNextUVar, newConstr);
            }

            case Bind b:
            {
                PrintBindingType(ctx, b.Binding, b.Name);
                // Reset nextuvar to 0 (uvargen) but keep accumulated constraints
                return (ctx.AddBinding(b.Name, b.Binding), 0, constr);
            }

            default:
                throw new InvalidOperationException($"Unexpected command: {cmd}");
        }
    }
}
