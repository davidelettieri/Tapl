using System;
using System.Collections.Generic;
using System.IO;
using static System.Console;

namespace Chapter7.Interpreter
{
    class Program
    {
        static bool _hadError;
        static int Main(string[] args)
        {
            if (args.Length > 1)
            {
                WriteLine("Chapter7.Interpreter.exe [script]");
                return 65;
            }
            else if (args.Length == 1)
            {
                RunFile(args[0]);
            }
            else
            {
                RunPrompt();
            }

            if (_hadError)
                return 65;

            return 0;
        }

        private static void RunPrompt()
        {
            while (true)
            {
                Write("> ");
                Run(ReadLine());
            }
        }

        private static void RunFile(string path)
        {
            var source = File.ReadAllText(path);
            Run(source);
        }

        private static void Run(string source)
        {
            var scanner = new Scanner(source);
            var tokens = scanner.ScanTokens();

            foreach (var token in tokens)
            {
                WriteLine(token);
            }
        }

        public static void Error(int line, string message)
        {
            Report(line, "", message);
        }

        private static void Report(int line, string where, string message)
        {
            WriteLine("[line " + line + "] Error" + where + ": " + message);
            _hadError = true;
        }
    }
}
