using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using FullRef.Syntax;
using FullRef.Syntax.Bindings;
using FullRef.Syntax.Terms;

namespace FullRef.Core;

public static class Typing
{
    private static bool IsTyAbb(Context ctx, int index) => ctx.GetBinding(index) is TypeAbbBind;

    private static IType GetTyAbb(Context ctx, int index)
        => ctx.GetBinding(index) switch
        {
            TypeAbbBind typeAbbBind => typeAbbBind.Type,
            _ => throw new NoRulesAppliesException()
        };

    private static IType ComputeType(Context ctx, IType type)
        => type switch
        {
            TypeVar typeVar when IsTyAbb(ctx, typeVar.X) => GetTyAbb(ctx, typeVar.X),
            _ => throw new NoRulesAppliesException()
        };

    private static IType SimplifyType(Context ctx, IType type)
    {
        try
        {
            return SimplifyType(ctx, ComputeType(ctx, type));
        }
        catch (NoRulesAppliesException)
        {
            return type;
        }
    }

    public static bool TypeEqual(Context ctx, IType left, IType right)
    {
        var simplifiedLeft = SimplifyType(ctx, left);
        var simplifiedRight = SimplifyType(ctx, right);

        return (simplifiedLeft, simplifiedRight) switch
        {
            (TypeVariant leftVariant, TypeVariant rightVariant) => TypeEqualVariant(leftVariant, rightVariant),
            (TypeBot, TypeBot) => true,
            (TypeId leftId, TypeId rightId) => leftId.Name == rightId.Name,
            (TypeString, TypeString) => true,
            (TypeFloat, TypeFloat) => true,
            (TypeArrow leftArrow, TypeArrow rightArrow) => TypeEqual(ctx, leftArrow.From, rightArrow.From)
                && TypeEqual(ctx, leftArrow.To, rightArrow.To),
            (TypeUnit, TypeUnit) => true,
            (TypeRef leftRef, TypeRef rightRef) => TypeEqual(ctx, leftRef.Type, rightRef.Type),
            (TypeSource leftSource, TypeSource rightSource) => TypeEqual(ctx, leftSource.Type, rightSource.Type),
            (TypeSink leftSink, TypeSink rightSink) => TypeEqual(ctx, leftSink.Type, rightSink.Type),
            (TypeTop, TypeTop) => true,
            (TypeBool, TypeBool) => true,
            (TypeNat, TypeNat) => true,
            (TypeRecord leftRecord, TypeRecord rightRecord) => TypeEqualRecord(leftRecord, rightRecord),
            (TypeVar leftVar, _) when IsTyAbb(ctx, leftVar.X) => TypeEqual(ctx, GetTyAbb(ctx, leftVar.X), simplifiedRight),
            (_, TypeVar rightVar) when IsTyAbb(ctx, rightVar.X) => TypeEqual(ctx, simplifiedLeft, GetTyAbb(ctx, rightVar.X)),
            (TypeVar leftVar, TypeVar rightVar) => leftVar.X == rightVar.X,
            _ => false
        };

        bool TypeEqualVariant(TypeVariant leftVariant, TypeVariant rightVariant)
        {
            var leftFields = leftVariant.Variants.ToList();
            var rightFields = rightVariant.Variants.ToList();
            return leftFields.Count == rightFields.Count
                && leftFields.Zip(rightFields, (l, r) => l.Item1 == r.Item1 && TypeEqual(ctx, l.Item2, r.Item2)).All(result => result);
        }

        bool TypeEqualRecord(TypeRecord leftRecord, TypeRecord rightRecord)
        {
            var leftFields = leftRecord.Variants.ToList();
            var rightFields = rightRecord.Variants.ToList();
            return leftFields.Count == rightFields.Count
                && rightFields.All(field => TryGetFieldType(leftFields, field.Item1, out var leftType)
                    && TypeEqual(ctx, leftType, field.Item2));
        }
    }

    public static IType TypeOf(Context ctx, ITerm term)
    {
        switch (term)
        {
            case Var variable:
                return ctx.GetTypeFromContext(variable.Index);
            case Abs abs:
            {
                var ctx1 = ctx.AddBinding(abs.V, new VarBind(abs.Type));
                var bodyType = TypeOf(ctx1, abs.Body);
                return new TypeArrow(abs.Type, Shifting.TypeShift(-1, bodyType));
            }
            case App app:
            {
                var leftType = TypeOf(ctx, app.Left);
                var rightType = TypeOf(ctx, app.Right);
                return SimplifyType(ctx, leftType) switch
                {
                    TypeArrow arrow when Subtype(ctx, rightType, arrow.From) => arrow.To,
                    TypeArrow => throw TypeError(app.Info, "parameter type mismatch"),
                    TypeBot => new TypeBot(),
                    _ => throw TypeError(app.Info, "arrow type expected")
                };
            }
            case Ascribe ascribe:
                return Subtype(ctx, TypeOf(ctx, ascribe.Term), ascribe.Type)
                    ? ascribe.Type
                    : throw TypeError(ascribe.Info, "body of as-term does not have the expected type");
            case StringTerm:
                return new TypeString();
            case Unit:
                return new TypeUnit();
            case Ref reference:
                return new TypeRef(TypeOf(ctx, reference.Term));
            case Loc loc:
                throw TypeError(loc.Info, "locations are not supposed to occur in source programs!");
            case Let letTerm:
            {
                var letType = TypeOf(ctx, letTerm.LetTerm);
                var ctx1 = ctx.AddBinding(letTerm.Variable, new VarBind(letType));
                return Shifting.TypeShift(-1, TypeOf(ctx1, letTerm.InTerm));
            }
            case True:
            case False:
                return new TypeBool();
            case If ifTerm:
                return Subtype(ctx, TypeOf(ctx, ifTerm.Condition), new TypeBool())
                    ? Join(ctx, TypeOf(ctx, ifTerm.Then), TypeOf(ctx, ifTerm.Else))
                    : throw TypeError(ifTerm.Info, "guard of conditional not a boolean");
            case Record record:
                return new TypeRecord(record.Fields.Select(field => (field.Item1, TypeOf(ctx, field.Item2))).ToList());
            case Proj proj:
            {
                var projectedType = SimplifyType(ctx, TypeOf(ctx, proj.Term));
                return projectedType switch
                {
                    TypeRecord recordType when TryGetFieldType(recordType.Variants, proj.Label, out var fieldType) => fieldType,
                    TypeRecord => throw TypeError(proj.Info, $"label {proj.Label} not found"),
                    TypeBot => new TypeBot(),
                    _ => throw TypeError(proj.Info, "Expected record type")
                };
            }
            case Case @case:
            {
                var caseType = SimplifyType(ctx, TypeOf(ctx, @case.Term));
                return caseType switch
                {
                    TypeVariant variantType => TypeOfVariant(ctx, @case, variantType),
                    TypeBot => new TypeBot(),
                    _ => throw TypeError(@case.Info, "Expected variant type")
                };
            }
            case Tag tag:
            {
                var simplifiedType = SimplifyType(ctx, tag.Type);
                if (simplifiedType is not TypeVariant variantType)
                    throw TypeError(tag.Info, "Annotation is not a variant type");

                if (!TryGetFieldType(variantType.Variants, tag.Label, out var expectedType))
                    throw TypeError(tag.Info, $"label {tag.Label} not found");

                var actualType = TypeOf(ctx, tag.Term);
                return Subtype(ctx, actualType, expectedType)
                    ? tag.Type
                    : throw TypeError(tag.Info, "field does not have expected type");
            }
            case Fix fix:
            {
                var fixedType = TypeOf(ctx, fix.Term);
                return SimplifyType(ctx, fixedType) switch
                {
                    TypeArrow arrow when Subtype(ctx, arrow.To, arrow.From) => arrow.To,
                    TypeArrow => throw TypeError(fix.Info, "result of body not compatible with domain"),
                    TypeBot => new TypeBot(),
                    _ => throw TypeError(fix.Info, "arrow type expected")
                };
            }
            case Deref deref:
            {
                var derefType = SimplifyType(ctx, TypeOf(ctx, deref.Term));
                return derefType switch
                {
                    TypeRef referenceType => referenceType.Type,
                    TypeBot => new TypeBot(),
                    TypeSource sourceType => sourceType.Type,
                    _ => throw TypeError(deref.Info, "argument of ! is not a Ref or Source")
                };
            }
            case Assign assign:
            {
                var leftType = SimplifyType(ctx, TypeOf(ctx, assign.Left));
                return leftType switch
                {
                    TypeRef referenceType when Subtype(ctx, TypeOf(ctx, assign.Right), referenceType.Type) => new TypeUnit(),
                    TypeRef => throw TypeError(assign.Info, "arguments of := are incompatible"),
                    TypeBot => TypeOf(ctx, assign.Right) is IType ? new TypeBot() : new TypeBot(),
                    TypeSink sinkType when Subtype(ctx, TypeOf(ctx, assign.Right), sinkType.Type) => new TypeUnit(),
                    TypeSink => throw TypeError(assign.Info, "arguments of := are incompatible"),
                    _ => throw TypeError(assign.Info, "argument of ! is not a Ref or Sink")
                };
            }
            case Float:
                return new TypeFloat();
            case TimesFloat timesFloat:
                return Subtype(ctx, TypeOf(ctx, timesFloat.Left), new TypeFloat())
                    && Subtype(ctx, TypeOf(ctx, timesFloat.Right), new TypeFloat())
                    ? new TypeFloat()
                    : throw TypeError(timesFloat.Info, "argument of timesfloat is not a number");
            case Inert inert:
                return inert.Type;
            case Zero:
                return new TypeNat();
            case Succ succ:
                return Subtype(ctx, TypeOf(ctx, succ.Of), new TypeNat())
                    ? new TypeNat()
                    : throw TypeError(succ.Info, "argument of succ is not a number");
            case Pred pred:
                return Subtype(ctx, TypeOf(ctx, pred.Of), new TypeNat())
                    ? new TypeNat()
                    : throw TypeError(pred.Info, "argument of pred is not a number");
            case IsZero isZero:
                return Subtype(ctx, TypeOf(ctx, isZero.Term), new TypeNat())
                    ? new TypeBool()
                    : throw TypeError(isZero.Info, "argument of iszero is not a number");
            default:
                throw new InvalidOperationException();
        }
    }

    private static IType TypeOfVariant(Context ctx, Case @case, TypeVariant variantType)
    {
        foreach (var (label, _, _) in @case.Cases)
        {
            if (!TryGetFieldType(variantType.Variants, label, out _))
                throw TypeError(@case.Info, $"label {label} not in type");
        }

        var caseTypes = @case.Cases.Select(branch =>
        {
            if (!TryGetFieldType(variantType.Variants, branch.label, out var branchType))
                throw TypeError(@case.Info, $"label {branch.label} not found");

            var ctx1 = ctx.AddBinding(branch.variable, new VarBind(branchType));
            return Shifting.TypeShift(-1, TypeOf(ctx1, branch.term));
        });

        return caseTypes.Aggregate<IType, IType>(new TypeBot(), (current, next) => Join(ctx, current, next));
    }

    private static bool Subtype(Context ctx, IType source, IType target)
    {
        if (TypeEqual(ctx, source, target))
            return true;

        var simplifiedSource = SimplifyType(ctx, source);
        var simplifiedTarget = SimplifyType(ctx, target);

        return (simplifiedSource, simplifiedTarget) switch
        {
            (TypeBot, _) => true,
            (_, TypeTop) => true,
            (TypeArrow sourceArrow, TypeArrow targetArrow) => Subtype(ctx, targetArrow.From, sourceArrow.From)
                && Subtype(ctx, sourceArrow.To, targetArrow.To),
            (TypeRef sourceRef, TypeRef targetRef) => Subtype(ctx, sourceRef.Type, targetRef.Type)
                && Subtype(ctx, targetRef.Type, sourceRef.Type),
            (TypeRef sourceRef, TypeSource targetSource) => Subtype(ctx, sourceRef.Type, targetSource.Type),
            (TypeSource sourceSource, TypeSource targetSource) => Subtype(ctx, sourceSource.Type, targetSource.Type),
            (TypeRef sourceRef, TypeSink targetSink) => Subtype(ctx, targetSink.Type, sourceRef.Type),
            (TypeSink sourceSink, TypeSink targetSink) => Subtype(ctx, targetSink.Type, sourceSink.Type),
            (TypeVariant sourceVariant, TypeVariant targetVariant) => sourceVariant.Variants.All(field =>
                TryGetFieldType(targetVariant.Variants, field.Item1, out var targetFieldType)
                && Subtype(ctx, field.Item2, targetFieldType)),
            (TypeRecord sourceRecord, TypeRecord targetRecord) => targetRecord.Variants.All(field =>
                TryGetFieldType(sourceRecord.Variants, field.Item1, out var sourceFieldType)
                && Subtype(ctx, sourceFieldType, field.Item2)),
            _ => false
        };
    }

    private static IType Join(Context ctx, IType left, IType right)
    {
        if (Subtype(ctx, left, right))
            return right;

        if (Subtype(ctx, right, left))
            return left;

        var simplifiedLeft = SimplifyType(ctx, left);
        var simplifiedRight = SimplifyType(ctx, right);

        return (simplifiedLeft, simplifiedRight) switch
        {
            (TypeArrow leftArrow, TypeArrow rightArrow) => new TypeArrow(
                Meet(ctx, leftArrow.From, rightArrow.From),
                Join(ctx, leftArrow.To, rightArrow.To)),
            (TypeRef leftRef, TypeRef rightRef) => Subtype(ctx, leftRef.Type, rightRef.Type)
                && Subtype(ctx, rightRef.Type, leftRef.Type)
                ? new TypeRef(leftRef.Type)
                : new TypeSource(Join(ctx, leftRef.Type, rightRef.Type)),
            (TypeSource leftSource, TypeSource rightSource) => new TypeSource(Join(ctx, leftSource.Type, rightSource.Type)),
            (TypeRef leftRef, TypeSource rightSource) => new TypeSource(Join(ctx, leftRef.Type, rightSource.Type)),
            (TypeSource leftSource, TypeRef rightRef) => new TypeSource(Join(ctx, leftSource.Type, rightRef.Type)),
            (TypeSink leftSink, TypeSink rightSink) => new TypeSink(Meet(ctx, leftSink.Type, rightSink.Type)),
            (TypeRef leftRef, TypeSink rightSink) => new TypeSink(Meet(ctx, leftRef.Type, rightSink.Type)),
            (TypeSink leftSink, TypeRef rightRef) => new TypeSink(Meet(ctx, leftSink.Type, rightRef.Type)),
            (TypeRecord leftRecord, TypeRecord rightRecord) => new TypeRecord(
                leftRecord.Variants
                    .Select(field => field.Item1)
                    .Where(label => TryGetFieldType(rightRecord.Variants, label, out _))
                    .Select(label =>
                    {
                        TryGetFieldType(leftRecord.Variants, label, out var leftType);
                        TryGetFieldType(rightRecord.Variants, label, out var rightType);
                        return (label, Join(ctx, leftType, rightType));
                    })
                    .ToList()),
            _ => new TypeTop()
        };
    }

    private static IType Meet(Context ctx, IType left, IType right)
    {
        if (Subtype(ctx, left, right))
            return left;

        if (Subtype(ctx, right, left))
            return right;

        var simplifiedLeft = SimplifyType(ctx, left);
        var simplifiedRight = SimplifyType(ctx, right);

        return (simplifiedLeft, simplifiedRight) switch
        {
            (TypeArrow leftArrow, TypeArrow rightArrow) => new TypeArrow(
                Join(ctx, leftArrow.From, rightArrow.From),
                Meet(ctx, leftArrow.To, rightArrow.To)),
            (TypeRef leftRef, TypeRef rightRef) => Subtype(ctx, leftRef.Type, rightRef.Type)
                && Subtype(ctx, rightRef.Type, leftRef.Type)
                ? new TypeRef(leftRef.Type)
                : new TypeSource(Meet(ctx, leftRef.Type, rightRef.Type)),
            (TypeSource leftSource, TypeSource rightSource) => new TypeSource(Meet(ctx, leftSource.Type, rightSource.Type)),
            (TypeRef leftRef, TypeSource rightSource) => new TypeSource(Meet(ctx, leftRef.Type, rightSource.Type)),
            (TypeSource leftSource, TypeRef rightRef) => new TypeSource(Meet(ctx, leftSource.Type, rightRef.Type)),
            (TypeSink leftSink, TypeSink rightSink) => new TypeSink(Join(ctx, leftSink.Type, rightSink.Type)),
            (TypeRef leftRef, TypeSink rightSink) => new TypeSink(Join(ctx, leftRef.Type, rightSink.Type)),
            (TypeSink leftSink, TypeRef rightRef) => new TypeSink(Join(ctx, leftSink.Type, rightRef.Type)),
            (TypeRecord leftRecord, TypeRecord rightRecord) => new TypeRecord(
                leftRecord.Variants.Select(field => field.Item1)
                    .Concat(rightRecord.Variants.Select(field => field.Item1).Where(label => !leftRecord.Variants.Any(existing => existing.Item1 == label)))
                    .Select(label =>
                    {
                        var hasLeft = TryGetFieldType(leftRecord.Variants, label, out var leftType);
                        var hasRight = TryGetFieldType(rightRecord.Variants, label, out var rightType);

                        if (hasLeft && hasRight)
                            return (label, Meet(ctx, leftType, rightType));

                        return hasLeft ? (label, leftType) : (label, rightType);
                    })
                    .ToList()),
            _ => new TypeBot()
        };
    }

    private static bool TryGetFieldType(IEnumerable<(string, IType)> fields, string label, out IType type)
    {
        foreach (var (fieldLabel, fieldType) in fields)
        {
            if (fieldLabel != label)
                continue;

            type = fieldType;
            return true;
        }

        type = null!;
        return false;
    }

    private static TaplTypingException TypeError(IInfo info, string message) => new(info, message);
}