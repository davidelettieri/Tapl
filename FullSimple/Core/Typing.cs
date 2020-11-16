using FullSimple.Syntax;
using Common;
using System;
using FullSimple.Syntax.Types;
using FullSimple.Syntax.Terms;
using FullSimple.Syntax.Bindings;
using System.Linq;

namespace FullSimple.Core
{
    public static class Typing
    {
        private static bool IsTyAbb(Context ctx, int i) => ctx.GetBinding(i) is TypeAbbBind;

        private static IType GetTyAbb(Context ctx, int i)
            => ctx.GetBinding(i) switch
            {
                TypeAbbBind ta => ta.Type,
                _ => throw new NoRulesAppliesException()
            };

        private static IType ComputeType(Context ctx, IType t)
            => t switch
            {
                TypeVar tv when IsTyAbb(ctx, tv.X) => GetTyAbb(ctx, tv.X),
                _ => throw new NoRulesAppliesException()
            };

        private static IType SimplifyType(Context ctx, IType t)
        {
            try
            {
                var t1 = ComputeType(ctx, t);
                return SimplifyType(ctx, t1);
            }
            catch (NoRulesAppliesException)
            {
                return t;
            }
        }

        public static bool TypeEqual(Context ctx, IType t1, IType t2)
        {
            var t1s = SimplifyType(ctx, t1);
            var t2s = SimplifyType(ctx, t2);

            return (t1s, t2s) switch
            {
                (TypeString _, TypeString _) => true,
                (TypeUnit _, TypeUnit _) => true,
                (TypeId b1, TypeId b2) => b1.Equals(b2),
                (TypeVar tv, _) when IsTyAbb(ctx, tv.N) => TypeEqual(ctx, GetTyAbb(ctx, tv.N), t2s),
                (_, TypeVar tv) when IsTyAbb(ctx, tv.N) => TypeEqual(ctx, t1s, GetTyAbb(ctx, tv.N)),
                (TypeVar ti, TypeVar tj) => ti.X == tj.X,


                _ => false
            };
        }

        public static IType TypeOf(Context ctx, ITerm t)
        {
            switch (t)
            {
                case Abs abs:
                    var ctx1 = ctx.AddBinding(abs.V, new VarBind(abs.Type));
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
                case Ascribe asc:
                    return asc.Type;
                case Case c:
                    return SimplifyType(ctx, TypeOf(ctx, c.Term)) switch
                    {
                        TypeVariant tv => TypeOfVariant(ctx, c, tv),
                        _ => throw new Exception($"Expected variant type {t}.")
                    };
                case Var var:
                    return ctx.GetTypeFromContext(var.Index);
                case True:
                    return new TypeBool();
                case False:
                    return new TypeBool();
                case StringTerm:
                    return new TypeString();
                case If ift:
                    if (TypeOf(ctx, ift.Condition) is TypeBool)
                    {
                        var typeThen = TypeOf(ctx, ift.Then);

                        if (typeThen.Equals(TypeOf(ctx, ift.Then)))
                            return typeThen;
                        else throw new ArmsOfConditionalHaveDifferentTypesException();
                    }

                    throw new GuardNotBooleanException();
                case Float:
                    return new TypeFloat();
                case Unit:
                    return new TypeUnit();
                case Succ:
                    return new TypeNat();
                case Record rec:
                    return new TypeRecord(rec.Fields.Select(p => (p.Item1, TypeOf(ctx, p.Item2))));
                case Zero:
                    return new TypeNat();
                case Proj proj:
                    var t2 = SimplifyType(ctx, TypeOf(ctx, proj.Term));
                    return TypeOf(ctx, proj.Term);
                case Fix fix:
                    return TypeOf(ctx, fix.Term);
                default:
                    throw new InvalidOperationException();
            }
        }

        private static IType TypeOfVariant(Context ctx, Case c, TypeVariant tv)
        {
            var variants = tv.Variants.Select(p => p.Item1).ToHashSet();
            foreach (var item in c.Cases)
            {
                if (!variants.Contains(item.label))
                    throw new Exception($"Label {item.label} is not in type.");
            }

            var caseTypes = c.Cases.Select(p =>
            {
                var tyTi = tv.Variants.FirstOrDefault(v => v.Item1 == p.label);

                if (tyTi.Item1 is null)
                    throw new Exception($"Label {p.label} is not found.");

                var ctx1 = ctx.AddBinding(p.variable, new VarBind(tyTi.Item2));
                return Shifting.TypeShift(-1, TypeOf(ctx1, p.term));
            });

            var tyT1 = caseTypes.FirstOrDefault();
            var restTy = caseTypes.Skip(1);

            //foreach (var tyI in restTy)
            //{
            //    if (!TypeEqual(ctx, tyI, tyT1))
            //        throw new Exception("fields do not have the same type in " + string.Join(',', caseTypes));
            //}

            return tyT1;
        }
    }
}
