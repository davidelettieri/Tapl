using FullSimple.Syntax;
using Common;
using System;
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
                (TypeArrow ta1, TypeArrow ta2) => TypeEqual(ctx, ta1.From, ta2.From) && TypeEqual(ctx, ta1.To, ta2.To),
                (TypeBool, TypeBool) => true,
                (TypeNat, TypeNat) => true,
                (TypeRecord tr1, TypeRecord tr2) => TypeEqualRecord(tr1, tr2),
                (TypeVariant tv1, TypeVariant tv2) => TypeEqualVariant(tv1, tv2),
                _ => false
            };

            bool TypeEqualRecord(TypeRecord tr1, TypeRecord tr2)
            {
                if (tr1.Variants.Count() != tr2.Variants.Count())
                    return false;

                return tr1.Variants.Zip(tr2.Variants, (a, b) => TypeEqual(ctx, a.Item2, b.Item2)).All(p => p);
            }

            bool TypeEqualVariant(TypeVariant tv1, TypeVariant tv2)
            {
                if (tv1.Variants.Count() != tv2.Variants.Count())
                    return false;

                return tv1.Variants.Zip(tv2.Variants, (a, b) => a.Item1 == b.Item1 && TypeEqual(ctx, a.Item2, b.Item2)).All(p => p);
            }
        }

        public static IType TypeOf(Context ctx, ITerm t)
        {
            switch (t)
            {
                case True:
                case False:
                    return new TypeBool();
                case If ift:
                    if (TypeOf(ctx, ift.Condition) is TypeBool)
                    {
                        var typeThen = TypeOf(ctx, ift.Then);
                        var typeElse = TypeOf(ctx, ift.Else);

                        if (TypeEqual(ctx, typeThen, typeElse))
                            return typeThen;
                        else throw new ArmsOfConditionalHaveDifferentTypesException();
                    }
                    throw new GuardNotBooleanException();
                case Case c:
                    return SimplifyType(ctx, TypeOf(ctx, c.Term)) switch
                    {
                        TypeVariant tv => TypeOfVariant(ctx, c, tv),
                        _ => throw new Exception($"Expected variant type {t}.")
                    };
                case Tag tag:
                    var ty = SimplifyType(ctx, tag.Type);

                    if (ty is not TypeVariant tyv)
                        throw new Exception("annotation is not a variant type: " + t);

                    var field = tyv.Variants.FirstOrDefault(p => p.Item1 == tag.Label);

                    if (field.Item1 is null)
                        throw new Exception("label " + tag.Label + " not found " + t);

                    var ty2 = TypeOf(ctx, tag.Term);

                    if (TypeEqual(ctx, ty2, field.Item2))
                        return tag.Type;

                    throw new Exception("Field doesn't have expected type in " + t);
                case Var var:
                    return ctx.GetTypeFromContext(var.Index);
                case Abs abs:
                    var ctx1 = ctx.AddBinding(abs.V, new VarBind(abs.Type));
                    var typeBody = TypeOf(ctx1, abs.Body);
                    return new TypeArrow(abs.Type, typeBody);
                case App app:
                    var leftType = TypeOf(ctx, app.Left);
                    var rightType = TypeOf(ctx, app.Right);
                    var sLeftType = SimplifyType(ctx, leftType);
                    if (sLeftType is TypeArrow ta)
                    {
                        if (TypeEqual(ctx, rightType, ta.From))
                            return ta.To;

                        throw new ParameterTypeMismatchException();
                    }
                    throw new ArrowTypeExpectedException();
                case Let let:
                    var tyT1 = TypeOf(ctx, let.LetTerm);
                    var ctx2 = ctx.AddBinding(let.Variable, new VarBind(tyT1));
                    return Shifting.TypeShift(-1, TypeOf(ctx2, let.InTerm));
                case Fix fix:
                    var tyFix = TypeOf(ctx, fix.Term);
                    var sTyFix = SimplifyType(ctx, tyFix);
                    if (sTyFix is TypeArrow taw)
                    {
                        if (TypeEqual(ctx, taw.To, taw.From))
                            return taw.To;

                        throw new Exception("result of body not compatible with domain");
                    }
                    throw new Exception("error type expected in " + fix.Term);
                case StringTerm:
                    return new TypeString();
                case Unit:
                    return new TypeUnit();
                case Ascribe asc:
                    if (TypeEqual(ctx, TypeOf(ctx, asc.Term), asc.Type))
                        return asc.Type;
                    throw new Exception("body of as-term does not have the expected type");
                case Record rec:
                    return new TypeRecord(rec.Fields.Select(p => (p.Item1, TypeOf(ctx, p.Item2))));
                case Proj proj:
                    var t2 = SimplifyType(ctx, TypeOf(ctx, proj.Term));
                    if (t2 is TypeRecord tr)
                    {
                        var tyi = tr.Variants.FirstOrDefault(p => p.Item1 == proj.Label);

                        if (tyi.Item1 is null)
                        {
                            throw new Exception("Label " + proj.Label + " not found in " + t);
                        }

                        return tyi.Item2;
                    }

                    throw new Exception("Expected record type for " + t2);
                case Float:
                    return new TypeFloat();
                case TimesFloat tf:
                    var tf1 = TypeOf(ctx, tf.Left);
                    var tf2 = TypeOf(ctx, tf.Right);
                    var f = new TypeFloat();

                    if (TypeEqual(ctx, tf1, f) && TypeEqual(ctx, tf2, f))
                        return f;

                    throw new Exception("Argument of timesfloat is not a number");
                case Zero:
                    return new TypeNat();
                case Succ s:
                    if (TypeEqual(ctx, TypeOf(ctx, s.Of), new TypeNat()))
                        return new TypeNat();
                    throw new Exception("Argument of succ is not a number");
                case Pred p:
                    if (TypeEqual(ctx, TypeOf(ctx, p.Of), new TypeNat()))
                        return new TypeNat();
                    throw new Exception("Argument of pred is not a number");
                case IsZero iz:
                    if (TypeEqual(ctx, TypeOf(ctx, iz.Term), new TypeNat()))
                        return new TypeBool();
                    throw new Exception("Argument of iszero is not a number");
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
