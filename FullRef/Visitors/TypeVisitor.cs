using System;
using Common;
using FullRef.Syntax;

namespace FullRef.Visitors;

public sealed class TypeVisitor : FullRefBaseVisitor<Func<Context, IType>>
{
    private readonly ArrowTypeVisitor _arrowTypeVisitor;

    public TypeVisitor()
    {
        _arrowTypeVisitor = new ArrowTypeVisitor(this);
    }

    public override Func<Context, IType> VisitType_arrowtype(FullRefParser.Type_arrowtypeContext context) => _arrowTypeVisitor.Visit(context.arrowtype());

    public override Func<Context, IType> VisitType_ref(FullRefParser.Type_refContext context)
    {
        var atype = new ATypeVisitor(this).Visit(context.atype());
        return ctx => new TypeRef(atype(ctx));
    }

    public override Func<Context, IType> VisitType_sink(FullRefParser.Type_sinkContext context)
    {
        var atype = new ATypeVisitor(this).Visit(context.atype());
        return ctx => new TypeSink(atype(ctx));
    }

    public override Func<Context, IType> VisitType_source(FullRefParser.Type_sourceContext context)
    {
        var atype = new ATypeVisitor(this).Visit(context.atype());
        return ctx => new TypeSource(atype(ctx));
    }
}