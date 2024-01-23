using Common;

namespace FullSimple.Syntax.Terms;

public class Float : ITerm
{
    public IInfo Info { get; }
    public float Value { get; }

    public Float(IInfo info, float value)
    {
            Info = info;
            Value = value;
        }
}