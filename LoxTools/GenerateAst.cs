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
            //if (args.Length != 1)
            //{
            //    Console.Error.WriteLine("Usage: generate_ast <output directory");
            //    return 64;
            //}

            // var outputDir = args[0];
            DefineAst("", "Expr", new string[]
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

            // Create the sub classes
            foreach (var type in types)
            {
                var className = type.Split(':')[0].Trim();
                var fields = type.Split(':')[1].Trim();
                DefineType(stringBuilder, baseName, className, fields);
            }

            stringBuilder.AppendLine("\t}");
            stringBuilder.AppendLine("}");
            Console.WriteLine(stringBuilder.ToString());
            //File.WriteAllText(path, stringBuilder.ToString());
        }

        private static void DefineType(StringBuilder stringBuilder, string baseName, string className, string fieldList)
        {
            stringBuilder.AppendLine($"\t\tclass {className} : {baseName}");
            stringBuilder.AppendLine("\t\t{");

            // Constructor
            stringBuilder.AppendLine($"\t\t\tinternal {className}({fieldList})");
            stringBuilder.AppendLine("\t\t\t{");

            // Store parameters in properties
            foreach (var field in fieldList.Split(", "))
            {
                var name = field.Split(' ')[1];
                stringBuilder.AppendLine($"\t\t\t\tthis.{name} = {name};");
            }
            stringBuilder.AppendLine("\t\t\t}");
            stringBuilder.AppendLine();

            // Properties
            foreach (var field in fieldList.Split(", "))
            {
                stringBuilder.AppendLine($"\t\t\tinternal {field} {{ get; }}");
            }

            stringBuilder.AppendLine("\t\t}");
            stringBuilder.AppendLine();
        }
    }
}
