using System;
using Common;

namespace Demo;

public class Sample
{
    public string Name { get; }
    public Func<string, Context> Run { get; }

    public Sample(string name, Func<string, Context> run)
    {
        Name = name;
        Run = run;
    }
}