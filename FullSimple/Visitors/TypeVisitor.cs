using Antlr4.Runtime.Misc;
using Common;
using System;

namespace FullSimple.Visitors
{
    public class TypeVisitor : FullSimpleBaseVisitor<Func<Context, IType>>
    {
        private readonly ArrowTypeVisitor _arrowTypeVisitor = new ArrowTypeVisitor();
        public override Func<Context, IType> VisitType_arrowtype([NotNull] FullSimpleParser.Type_arrowtypeContext context)
        {
            return _arrowTypeVisitor.Visit(context.arrowtype());
        }
    }
}
