﻿using Chapter7;
using System;
using System.IO;
using static Chapter7.Functions;
namespace Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 1)
            {
                var source = File.ReadAllText(args[0]);

                Process(source);
            }
            else
            {
                var ctx = new Context();
                while (true)
                {
                    var commandSource = Console.ReadLine();
                    ctx = Process(commandSource + "\\n");
                }
            }
        }
    }
}
