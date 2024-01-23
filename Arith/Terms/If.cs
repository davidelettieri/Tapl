using Common;

namespace Arith.Terms;

public record If(ITerm Condition, ITerm Then, ITerm @Else) : ITerm;