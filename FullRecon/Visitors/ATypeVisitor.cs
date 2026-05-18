using System;
using Common;
using FullRecon.Syntax;

namespace FullRecon.Visitors;

public sealed class ATypeVisitor(TypeVisitor typeVisitor) : FullReconBaseVisitor<Func<Context, IType>>
{
    public override Func<Context, IType> VisitAt_bool(FullReconParser.At_boolContext context)
        => _ => new TypeBool();

    public override Func<Context, IType> VisitAt_nat(FullReconParser.At_natContext context)
        => _ => new TypeNat();

    public override Func<Context, IType> VisitAt_ucid(FullReconParser.At_ucidContext context)
    {
        var name = context.UCID().GetText();
        return _ => new TypeId(name);
    }

    public override Func<Context, IType> VisitAt_type(FullReconParser.At_typeContext context)
        => typeVisitor.Visit(context.type());
}
