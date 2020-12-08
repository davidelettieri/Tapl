using Antlr4.Runtime.Misc;
using Common;
using FullSimple.Syntax;
using System;

namespace FullSimple.Visitors
{
    public class ArrowTypeVisitor : FullSimpleBaseVisitor<Func<Context, IType>>
    {
        private readonly ATypeVisitor _aTypeVisitor;

        public ArrowTypeVisitor(TypeVisitor typeVisitor)
        {
            _aTypeVisitor = new ATypeVisitor(typeVisitor);
        }

        public override Func<Context, IType> VisitArrowtype_arrow([NotNull] FullSimpleParser.Arrowtype_arrowContext context)
        {
            var atype = _aTypeVisitor.Visit(context.atype());
            var arrType = Visit(context.arrowtype());

            return ctx => new TypeArrow(atype(ctx), arrType(ctx));
        }

        public override Func<Context, IType> VisitArrowtype_atype([NotNull] FullSimpleParser.Arrowtype_atypeContext context)
        {
            var atype = _aTypeVisitor.Visit(context.atype());

            return ctx => atype(ctx);
        }
    }
}
