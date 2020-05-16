using Common;
using System;
using System.Collections.Generic;
using System.IO;
using static System.Console;

namespace Demo
{
    static class Program
    {
        private static Dictionary<int, Sample> _samples = new Dictionary<int, Sample>()
        {
            { 1, new Sample( "untyped", Untyped.Functions.Process) },
            { 2, new Sample( "simplebool", SimpleBool.Functions.Process) },
            { 3, new Sample( "letexercise", LetExercise.Functions.Process) }
        };

        static void Main(string[] args)
        {
            WriteIntro();
            var n = ChooseSample();
            var t = ChooseRunType();
            RunSample(_samples[n], t);
        }

        private static void WriteIntro()
        {
            PrintText(">TAPL Examples:");
            foreach (var item in _samples)
            {
                PrintText($">{item.Key}: {item.Value.Name}");
            }
        }

        private static int ChooseSample()
        {
            while (true)
            {
                PrintText(">Enter sample number:");
                var sn = ReadLine();
                if (int.TryParse(sn, out var n) && _samples.ContainsKey(n))
                    return n;
            }
        }

        private static RunType ChooseRunType()
        {
            while (true)
            {
                PrintText("What do you want to run?");
                PrintText("1. File sample");
                PrintText("2. Command prompt");

                var sn = ReadLine();
                if (Enum.TryParse<RunType>(sn, out var n))
                    return n;
            }
        }

        private static void RunSample(Sample sample, RunType t)
        {
            PrintText($">Running sample {sample.Name}");

            if (t == RunType.FileSample)
            {
                var source = File.ReadAllText($"{sample.Name}.txt");
                PrintText(">Source file");
                WriteLine(source);
                PrintText(">Result:");
                sample.Run(source);
            }
            else
            {
                while (true)
                {
                    PrintText(">Insert command");
                    var commandSource = ReadLine();
                    sample.Run(commandSource);
                }
            }
        }

        private static void PrintText(string msg)
        {
            BackgroundColor = ConsoleColor.Blue;
            ForegroundColor = ConsoleColor.White;
            WriteLine(msg);
            ResetColor();
        }
    }
}
