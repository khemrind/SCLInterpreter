using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace Interpreter
{
    public static class Executer
    {
        public static void Process()
        {
            // import code
            var text = SourceText.From("using System;\n" + File.ReadAllText("test.cs"), Encoding.UTF8);

            // parse tree for C#
            var version = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.CSharp10);
            var tree = SyntaxFactory.ParseSyntaxTree(text, version);

            // refer common assemblies
            var references = Assembly.GetEntryAssembly().GetReferencedAssemblies().Select(assembly => 
                MetadataReference.CreateFromFile(Assembly.Load(assembly).Location)).ToList();

            references.Add(MetadataReference.CreateFromFile(typeof(object).Assembly.Location)); // mscorlib
            references.Add(MetadataReference.CreateFromFile(typeof(Console).Assembly.Location)); // System.Console

            // compile
            var compilation = CSharpCompilation.Create("test.dll", new SyntaxTree[] { tree }, references);

            // load assembly
            var assembly = Assembly.LoadFile(AppContext.BaseDirectory + "/test.dll");
            var type = assembly.GetType("test");

            // invoke Main function
            var flags = BindingFlags.Default | BindingFlags.InvokeMethod;
            type.InvokeMember("Main", flags, null, null, null);
        }
    }
}
