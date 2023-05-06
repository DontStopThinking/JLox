using System;
using System.Collections.Generic;
using System.IO;
using JLox.Scanning;

namespace JLox
{
    public class Lox
    {
        private static bool _hadError = false;

        public static void Main(string[] args)
        {
            if (args.Length > 1)
            {
                Console.WriteLine("Usage: jlox [script]");
                Environment.Exit(64);
            }
            else if (args.Length == 1)
            {
                RunFile(args[0]);
            }
            else
            {
                RunPrompt();
            }
        }

        public static void RunFile(string path)
        {
            Run(File.ReadAllText(path));
            if (_hadError)
            {
                Environment.Exit(65);
            }
        }

        public static void RunPrompt()
        {
            while (true)
            {
                Console.Write("> ");
                string? line = Console.ReadLine();
                if (line == null)
                {
                    break;
                }
                Run(line);
                _hadError = false;
            }
        }

        public static void Run(string source)
        {
            Scanner scanner = new(source);
            List<Token> tokens = scanner.ScanTokens();

            // for now, just print the tokens
            foreach (Token token in tokens)
            {
                Console.WriteLine(token);
            }
        }

        public static void Error(int line, string message)
        {
            Report(line, string.Empty, message);
        }

        private static void Report(int line, string where, string message)
        {
            Console.Error.WriteLine($"[line {line}] Error{where}: {message}");
            _hadError = true;
        }
    }
}
