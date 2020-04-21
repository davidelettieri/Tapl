using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using Chapter7.Terms;
using Common;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Chapter7
{
    public class TermVisitor : TaplBaseVisitor<ITerm>
    {
        private List<string> _boundVariables = new List<string>();
        public override ITerm VisitAbs([NotNull] TaplParser.AbsContext context)
        {
            var boundVar = context.VAR().GetText();
            _boundVariables.Add(boundVar);
            var result = new Abs(Visit(context.term()), boundVar);
            _boundVariables.Remove(boundVar);
            return result;
        }

        public override ITerm VisitApp([NotNull] TaplParser.AppContext context)
        {
            return new App(Visit(context.term()[0]), Visit(context.term()[1]));
        }

        public override ITerm VisitPar([NotNull] TaplParser.ParContext context)
        {
            return Visit(context.term());
        }

        public override ITerm VisitVar([NotNull] TaplParser.VarContext context)
        {
            var variable = context.VAR().GetText();
            var index = _boundVariables.Count - 1 - _boundVariables.IndexOf(variable);
            return new Var(index, 0);
        }
    }
}
