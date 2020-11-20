using Common;
using System;
using static FullSimple.Syntax.Printing;
using static FullSimple.Core.Evaluation;
using System.Collections.Immutable;
using Antlr4.Runtime;
using System.Linq;
using FullSimple.Visitors;
using FullSimple.Syntax.Bindings;
using FullSimple.Core;

namespace FullSimple
{
    public static class Functions
    {
        public static Func<Context, (ImmutableStack<ICommand>, Context)> Parse(string s)
        {
            if (string.IsNullOrWhiteSpace(s))
                throw new ArgumentException($"{nameof(s)} cannot be null or empty");

            var inputStream = new AntlrInputStream(s);
            var lexer = new FullSimpleLexer(inputStream);
            var commonTokenStream = new CommonTokenStream(lexer);
            var parser = new FullSimpleParser(commonTokenStream);
            var context = parser.toplevel();

            var visitor = new TopLevelVisitor();

            return visitor.Visit(context);
        }

        public static Context ProcessCommand(Context ctx, ICommand c)
        {
            switch (c)
            {
                case Eval e:
                    var t = Eval(ctx, e.Term);
                    PrintTerm(ctx, t);
                    return ctx;
                case Bind b:
                    var b1 = CheckBinding(ctx, b.Binding);
                    var b2 = EvalBinding(ctx, b1);
                    Console.Write(b.Name);
                    PrintBindingType(ctx, b.Binding);
                    return ctx.AddBinding(b.Name, b2);
                default:
                    throw new InvalidOperationException();
            }
        }

        private static IBinding CheckBinding(Context ctx, IBinding bind)
        {
            return bind switch
            {
                NameBinding n => n,
                TypeVarBind tvb => tvb,
                TermAbbBind tab when tab.Type is null => new TermAbbBind(tab.Term, Typing.TypeOf(ctx, tab.Term)),
                TermAbbBind tab when Typing.TypeEqual(ctx, tab.Type, Typing.TypeOf(ctx, tab.Term)) => tab,
                TermAbbBind => throw new Exception("type of binding doesn't match declared type in " + bind),
                VarBind vb => vb,
                TypeAbbBind tab => tab
            };
        }

        private static IBinding EvalBinding(Context ctx, IBinding bind)
        {
            return bind switch
            {
                TermAbbBind tab => new TermAbbBind(Eval(ctx, tab.Term), tab.Type),
                _ => bind
            };
        }

        public static Context Process(string source)
        {
            var fcommands = Parse(source);
            var commands = fcommands(new Context());

            return commands.Item1.Aggregate(new Context(), ProcessCommand);
        }
    }
}
