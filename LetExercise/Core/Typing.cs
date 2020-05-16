using LetExercise.Syntax;
using Common;
using System;

namespace LetExercise.Core
{
    public static class Typing
    {
        public static IType TypeOf(Context ctx, ITerm t)
        {
            switch (t)
            {
                case Let let:
                    var letTermType = TypeOf(ctx, let.LetTerm);
                    var ctx2 = ctx.AddBinding(let.Variable, new VarBind(letTermType));
                    return TypeOf(ctx2, let.InTerm);
                case Var var:
                    return ctx.GetTypeFromContext(var.Index);
                case Abs abs:
                    var ctx1 = ctx.AddBinding(abs.BoundedVariable, new VarBind(abs.Type));
                    var typeBody = TypeOf(ctx1, abs.Body);
                    return new TypeArrow(abs.Type, typeBody);
                case App app:
                    var leftType = TypeOf(ctx, app.Left);
                    var rightType = TypeOf(ctx, app.Right);
                    if (leftType is TypeArrow ta)
                    {
                        if (rightType.Equals(ta.From))
                            return ta.To;

                        throw new ParameterTypeMismatchException();
                    }
                    throw new ArrowTypeExpectedException();
                case True _:
                    return new TypeBool();
                case False _:
                    return new TypeBool();
                case If ift:
                    if (TypeOf(ctx, ift.Condition) is TypeBool)
                    {
                        var typeThen = TypeOf(ctx, ift.Then);

                        if (typeThen.Equals(TypeOf(ctx, ift.Then)))
                            return typeThen;
                        else throw new ArmsOfConditionalHaveDifferentTypesException();
                    }

                    throw new GuardNotBooleanException();
                default:
                    throw new InvalidOperationException();
            }
        }
    }
}
