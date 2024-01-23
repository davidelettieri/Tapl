using Common;

namespace FullSimple.Syntax.Bindings;

public class VarBind : IBinding
{
    public IType Type { get; }
    public VarBind(IType type)
    {
            Type = type;
        }
}