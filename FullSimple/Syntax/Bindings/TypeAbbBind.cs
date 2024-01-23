using Common;

namespace FullSimple.Syntax.Bindings;

public class TypeAbbBind : IBinding
{
    public IType Type { get; }
    public TypeAbbBind(IType type)
    {
            Type = type;
        }
}