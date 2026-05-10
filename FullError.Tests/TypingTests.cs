using Common;
using FullError.Syntax;
using FullError.Syntax.Terms;
using FullError.Core;
using Xunit;

namespace FullError.Tests;

public class TypingTests
{
    private static Context EmptyCtx() => new Context();

    [Fact(DisplayName = "typeof true is Bool")]
    public void TypeOfTrueIsBool()
    {
        var t = new True(new UnknownInfo());
        var ty = Typing.TypeOf(EmptyCtx(), t);
        Assert.IsType<TypeBool>(ty);
    }

    [Fact(DisplayName = "typeof false is Bool")]
    public void TypeOfFalseIsBool()
    {
        var t = new False(new UnknownInfo());
        var ty = Typing.TypeOf(EmptyCtx(), t);
        Assert.IsType<TypeBool>(ty);
    }

    [Fact(DisplayName = "typeof error is Bot")]
    public void TypeOfErrorIsBot()
    {
        var t = new Error(new UnknownInfo());
        var ty = Typing.TypeOf(EmptyCtx(), t);
        Assert.IsType<TypeBot>(ty);
    }

    [Fact(DisplayName = "typeof lambda x:Bool.x is Bool->Bool")]
    public void TypeOfAbsIsBoolToBool()
    {
        var info = new UnknownInfo();
        var t = new Abs(info, "x", new TypeBool(), new Var(info, 0, 1));
        var ty = Typing.TypeOf(EmptyCtx(), t);
        var arr = Assert.IsType<TypeArrow>(ty);
        Assert.IsType<TypeBool>(arr.From);
        Assert.IsType<TypeBool>(arr.To);
    }

    [Fact(DisplayName = "typeof if true then false else true is Bool")]
    public void TypeOfIfTrueThenFalseElseTrue()
    {
        var info = new UnknownInfo();
        var t = new If(info, new True(info), new False(info), new True(info));
        var ty = Typing.TypeOf(EmptyCtx(), t);
        Assert.IsType<TypeBool>(ty);
    }

    [Fact(DisplayName = "typeof try true with false is Bool")]
    public void TypeOfTryTrueWithFalseIsBool()
    {
        var info = new UnknownInfo();
        var t = new Try(info, new True(info), new False(info));
        var ty = Typing.TypeOf(EmptyCtx(), t);
        Assert.IsType<TypeBool>(ty);
    }

    [Fact(DisplayName = "Bot subtypes Bool")]
    public void BotSubtypesBool()
    {
        Assert.True(Typing.Subtype(EmptyCtx(), new TypeBot(), new TypeBool()));
    }

    [Fact(DisplayName = "Bool subtypes Top")]
    public void BoolSubtypesTop()
    {
        Assert.True(Typing.Subtype(EmptyCtx(), new TypeBool(), new TypeTop()));
    }

    [Fact(DisplayName = "join Bool Bool is Bool")]
    public void JoinBoolBoolIsBool()
    {
        var ty = Typing.Join(EmptyCtx(), new TypeBool(), new TypeBool());
        Assert.IsType<TypeBool>(ty);
    }

    [Fact(DisplayName = "join Bot Bool is Bool")]
    public void JoinBotBoolIsBool()
    {
        var ty = Typing.Join(EmptyCtx(), new TypeBot(), new TypeBool());
        Assert.IsType<TypeBool>(ty);
    }

    [Fact(DisplayName = "typeof (lambda x:Bool. x) error is Bot")]
    public void TypeOfAppWithErrorArgIsBot()
    {
        // (lambda x:Bool. x) error
        // typeof(lambda x:Bool.x) = Bool->Bool
        // typeof(error) = Bot
        // Bot <: Bool, so result is Bool
        var info = new UnknownInfo();
        var abs = new Abs(info, "x", new TypeBool(), new Var(info, 0, 1));
        var err = new Error(info);
        var app = new App(info, abs, err);
        var ty = Typing.TypeOf(EmptyCtx(), app);
        Assert.IsType<TypeBool>(ty);
    }

    [Fact(DisplayName = "typeof error true is Bot")]
    public void TypeOfErrorAppIsBot()
    {
        // error true
        // typeof(error) = Bot (arrow type expected handling: Bot case returns Bot)
        var info = new UnknownInfo();
        var err = new Error(info);
        var t = new True(info);
        var app = new App(info, err, t);
        var ty = Typing.TypeOf(EmptyCtx(), app);
        Assert.IsType<TypeBot>(ty);
    }
}
