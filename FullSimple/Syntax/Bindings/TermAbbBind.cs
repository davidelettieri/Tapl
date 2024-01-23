using Common;

namespace FullSimple.Syntax.Bindings;

public class TermAbbBind : IBinding
{
    public ITerm Term { get; }
    public IType Type { get; }
    public TermAbbBind(ITerm term, IType type)
    {
            Term = term;
            Type = type;
        }

    public override string ToString()
    {
            return $"TmAbbBind({Term},{(object)Type ?? "None"})";
        }
}