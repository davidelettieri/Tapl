using Common;
using System;
using System.Collections.Generic;
using System.IO;
using static System.Console;

namespace Demo
{

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
}
