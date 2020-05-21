using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using FullSimple.Syntax;
using Common;
using System;
using System.Collections.Immutable;
using FullSimple.Syntax.Terms;
using FullSimple.Syntax.Types;
using static FullSimple.Helper;
using FullSimple.Command;
using System.Collections;
using System.Collections.Generic;

namespace FullSimple.Visitors
{
    public class AppTermVisitor : FullSimpleBaseVisitor<Func<Context, ITerm>>
    {
        public override Func<Context, ITerm> VisitAppterm_path([NotNull] FullSimpleParser.Appterm_pathContext context)
        {
            return base.VisitAppterm_path(context);
        }

        public override Func<Context, ITerm> VisitAppterm_app_path([NotNull] FullSimpleParser.Appterm_app_pathContext context)
        {
            return base.VisitAppterm_app_path(context);
        }

        public override Func<Context, ITerm> VisitAppterm_fix([NotNull] FullSimpleParser.Appterm_fixContext context)
        {
            return base.VisitAppterm_fix(context);
        }

        public override Func<Context, ITerm> VisitAppterm_iszero([NotNull] FullSimpleParser.Appterm_iszeroContext context)
        {
            return base.VisitAppterm_iszero(context);
        }

        public override Func<Context, ITerm> VisitAppterm_pred([NotNull] FullSimpleParser.Appterm_predContext context)
        {
            return base.VisitAppterm_pred(context);
        }

        public override Func<Context, ITerm> VisitAppterm_succ([NotNull] FullSimpleParser.Appterm_succContext context)
        {
            return base.VisitAppterm_succ(context);
        }

        public override Func<Context, ITerm> VisitAppterm_times([NotNull] FullSimpleParser.Appterm_timesContext context)
        {
            return base.VisitAppterm_times(context);
        }
    }
}
