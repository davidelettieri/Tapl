using SimpleBool.Syntax;
using Common;
using System;

namespace SimpleBool.Core;

public static class Typing
{
    public static IType TypeOf(Context ctx, ITerm t)
    {
        switch (t)
        {
            case Var var:
                return ctx.GetTypeFromContext(var.Index);
            case Abs abs:
                var ctx1 = ctx.AddBinding(abs.BoundedVariable, new VarBind(abs.Type));
                var typeBody = TypeOf(ctx1, abs.Body);
                return new TypeArrow(abs.Type, typeBody);
            case App app:
                var leftType = TypeOf(ctx, app.Left);
                var rightType = TypeOf(ctx, app.Right);
                if (leftType is not TypeArrow ta)
                {
                    throw new ArrowTypeExpectedException();
                }
                if (rightType.Equals(ta.From))
                    return ta.To;
                throw new ParameterTypeMismatchException();
            case True:
                return new TypeBool();
            case False:
                return new TypeBool();
            case If ift:
                if (TypeOf(ctx, ift.Condition) is not TypeBool)
                {
                    throw new GuardNotBooleanException();
                }

                var typeThen = TypeOf(ctx, ift.Then);

                if (typeThen.Equals(TypeOf(ctx, ift.Then)))
                    return typeThen;
                throw new ArmsOfConditionalHaveDifferentTypesException();

            default:
                throw new InvalidOperationException();
        }
    }
}