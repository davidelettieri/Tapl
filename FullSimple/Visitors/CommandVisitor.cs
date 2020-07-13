using Antlr4.Runtime.Misc;
using Common;
using System;

namespace FullSimple.Visitors
{
    public class CommandVisitor : FullSimpleBaseVisitor<Func<Context, (ICommand, Context)>>
    {
        private static readonly BinderVisitor _binderVisitor = new BinderVisitor();
        private static readonly TypeBinderVisitor _typeBinderVisitor = new TypeBinderVisitor();
        private static readonly TermVisitor _termVisitor = new TermVisitor();
        public override Func<Context, (ICommand, Context)> VisitCommand_binder([NotNull] FullSimpleParser.Command_binderContext context)
        {
            var info = context.GetFileInfo();
            var bind = _binderVisitor.Visit(context.binder());
            var name = context.LCID().GetText();

            return ctx => (new Bind(info, name, bind(ctx)), ctx.AddName(name));
        }

        public override Func<Context, (ICommand, Context)> VisitCommand_term([NotNull] FullSimpleParser.Command_termContext context)
        {
            var info = context.GetFileInfo();
            var term = _termVisitor.Visit(context.term());
            return ctx => (new Eval(info, term(ctx)), ctx);
        }

        public override Func<Context, (ICommand, Context)> VisitCommand_tybinder([NotNull] FullSimpleParser.Command_tybinderContext context)
        {
            var info = context.GetFileInfo();
            var bind = _typeBinderVisitor.Visit(context.tybinder());
            var name = context.UCID().GetText();

            return ctx => (new Bind(info, name, bind(ctx)), ctx.AddName(name));
        }
    }
}
