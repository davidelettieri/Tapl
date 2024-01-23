namespace Common;

public interface ITerm
{
    IInfo Info => new UnknownInfo();
}