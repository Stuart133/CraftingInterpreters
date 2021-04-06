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
                "Binary     : Expr left, Token op, Expr right",
                "Grouping   : Expr expression",
                "Literal    : object value",
                "Unary      : Token op, Expr right"
            });

            return 0;
        }

        private static void DefineAst(string outputDir, string baseName, IEnumerable<string> types)
        {
            var path = $"{outputDir}/{baseName}.cs";
            var builder = new StringBuilder();

            builder.AppendLine("// This file is auto generated from LoxTools - Do not modify directly");
            builder.AppendLine();

            builder.AppendLine("using LoxDotNet.Scanning;");
            builder.AppendLine();
            builder.AppendLine("namespace LoxDotNet.Parsing");
            builder.AppendLine("{");
            builder.AppendLine($"\tpublic abstract class {baseName}");
            builder.AppendLine("\t{");

            // Create the visitor interface
            DefineVisitor(builder, baseName, types);
            builder.AppendLine();

            // Create the sub classes
            foreach (var type in types)
            {
                var className = type.Split(':')[0].Trim();
                var fields = type.Split(':')[1].Trim();
                DefineType(builder, baseName, className, fields);
            }

            // Add the base Accept<T> method
            builder.AppendLine("\t\tinternal abstract T Accept<T>(IVisitor<T> visitor);");

            builder.AppendLine("\t}");
            builder.AppendLine("}");
            File.WriteAllText(path, builder.ToString());
        }

        private static void DefineVisitor(StringBuilder builder, string baseName, IEnumerable<string> types)
        {
            builder.AppendLine("\t\tpublic interface IVisitor<T>");
            builder.AppendLine("\t\t{");

            foreach (var type in types)
            {
                var typeName = type.Split(':')[0].Trim();
                builder.AppendLine($"\t\t\tT Visit{typeName}{baseName}({typeName} {baseName.ToLower()});");
            }

            builder.AppendLine("\t\t}");
        }

        private static void DefineType(StringBuilder builder, string baseName, string className, string fieldList)
        {
            builder.AppendLine($"\t\tpublic class {className} : {baseName}");
            builder.AppendLine("\t\t{");

            // Constructor
            builder.AppendLine($"\t\t\tinternal {className}({fieldList})");
            builder.AppendLine("\t\t\t{");

            // Store parameters in properties
            foreach (var field in fieldList.Split(", "))
            {
                var name = field.Split(' ')[1];
                builder.AppendLine($"\t\t\t\tthis.{name} = {name};");
            }
            builder.AppendLine("\t\t\t}");
            builder.AppendLine();

            // Visitor implementation
            builder.AppendLine("\t\t\tinternal override T Accept<T>(IVisitor<T> visitor)");
            builder.AppendLine("\t\t\t{");
            builder.AppendLine($"\t\t\t\treturn visitor.Visit{className}{baseName}(this);");
            builder.AppendLine("\t\t\t}");
            builder.AppendLine();

            // Properties
            foreach (var field in fieldList.Split(", "))
            {
                builder.AppendLine($"\t\t\tinternal {field} {{ get; }}");
            }

            builder.AppendLine("\t\t}");
            builder.AppendLine();
        }
    }
}
