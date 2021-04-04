using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LoxTools
{
    class GenerateAst
    {
        static int Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.Error.WriteLine("Usage: generate_ast <output directory");
                return 64;
            }

            var outputDir = args[0];
            DefineAst(outputDir, "Expr", new string[]
            {
                "Binary     : Expr left, Token operator, Expr right",
                "Grouping   : Expr expression",
                "Literal    : Object value",
                "Unary      : Token operator, Expr right"
            });

            return 0;
        }

        private static void DefineAst(string outputDir, string baseName, IEnumerable<string> types)
        {
            var path = $"{outputDir}/{baseName}.cs";
            var stringBuilder = new StringBuilder();

            stringBuilder.AppendLine("using LoxDotNet.Scanning;");
            stringBuilder.AppendLine();
            stringBuilder.AppendLine("namespace LoxDotNet.Parsing");
            stringBuilder.AppendLine("{");
            stringBuilder.AppendLine($"\tabstract class {baseName}");
            stringBuilder.AppendLine("\t{");

            stringBuilder.AppendLine("\t}");
            stringBuilder.AppendLine("}");
            File.WriteAllText(path, stringBuilder.ToString());
        }
    }
}
