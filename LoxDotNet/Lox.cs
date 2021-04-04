using LoxDotNet.Scanning;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LoxDotNet
{
    class Lox
    {
        private static bool HadError { get; set; } = false;

        static int Main(string[] args)
        {
            if (args.Length > 1)
            {
                Console.WriteLine("Usage: LoxDotNet [script]");
                return 64;
            }
            else if (args.Length == 1)
            {
                RunFile(args[0]);
            }
            else
            {
                RunPrompt();
            }

            return 0;
        }

        internal static void Error(int line, string message)
        {
            Report(line, "", message);
        }

        private static void Run(string source)
        {
            var scanner = new Scanner(source);
            List<Token> tokens = scanner.ScanTokens();

            // For now, just print the tokens
            foreach (var token in tokens)
            {
                Console.WriteLine(token);
            }
        }

        private static void RunFile(string path)
        {
            var bytes = File.ReadAllBytes(path);
            Run(Encoding.Default.GetString(bytes));

            if (HadError)
            {
                Environment.Exit(65);
            }
        }

        private static void RunPrompt()
        {
            while (true)
            {
                Console.Write("> ");
                var line = Console.ReadLine();
                if (line == null)
                {
                    break;
                }
                Run(line);
                HadError = false;
            }
        }

        private static void Report(int line, string where, string message)
        {
            Console.Error.WriteLine($"[line {line}] Error{where}: {message}");
            HadError = true;
        }
    }
}
