using Common;

namespace SimpleBool.Syntax;

public class VarBind : IBinding
{
    public IType Type { get; }
    public VarBind(IType type)
    {
            Type = type;
        }
}