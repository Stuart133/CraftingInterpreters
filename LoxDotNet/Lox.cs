using LoxDotNet.Interpreting;
using LoxDotNet.Parsing;
using LoxDotNet.Scanning;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LoxDotNet
{
    class Lox
    {
        private static readonly Interpreter _interpreter = new Interpreter();
        private static bool _hadError = false;
        private static bool _hadRuntimeError = false;

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

        private static void Run(string source)
        {
            var scanner = new Scanner(source);
            List<Token> tokens = scanner.ScanTokens();

            var parser = new Parser(tokens);
            var statements = parser.Parse();

            // Stop if there was a syntax error
            if (_hadError)
            {
                return;
            }

            _interpreter.Interpret(statements);
        }

        private static void RunFile(string path)
        {
            var bytes = File.ReadAllBytes(path);
            Run(Encoding.Default.GetString(bytes));

            if (_hadError)
            {
                System.Environment.Exit(65);
            }

            if (_hadRuntimeError)
            {
                System.Environment.Exit(70);
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
                _hadError = false;
            }
        }

        internal static void Error(int line, string message)
        {
            Report(line, "", message);
        }

        internal static void Error(Token token, string message)
        {
            if (token.Type == TokenType.EOF)
            {
                Report(token.Line, " at end", message);
            }
            else
            {
                Report(token.Line, $" at '{token.Lexeme}'", message);
            }
        }

        internal static void RuntimeError(RuntimeException error)
        {
            Console.Error.WriteLine($"{error.Message}\n[line {error.Token.Line}]");
            _hadRuntimeError = true;
        }

        private static void Report(int line, string where, string message)
        {
            Console.Error.WriteLine($"[line {line}] Error{where}: {message}");
            _hadError = true;
        }
    }
}
