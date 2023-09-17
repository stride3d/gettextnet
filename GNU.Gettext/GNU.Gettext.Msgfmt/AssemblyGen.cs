using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using System.CodeDom.Compiler;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using System.Resources;
using System.Runtime.Versioning;

namespace GNU.Gettext.Msgfmt
{
    public class AssemblyGen
    {
        private readonly IndentedTextWriter cw;
        private readonly StringWriter sw;
        private Catalog catalog;

        public Dictionary<string, string> Entries { get; private set; }
        public string CsharpSourceFileName { get; private set; }
        public string AssemblyOutDir { get; private set; }
        public Options Options { get; private set; }
        public string ClassName { get; private set; }

        #region Constructors
        public AssemblyGen(Options options)
        {
            sw = new StringWriter();
            cw = new IndentedTextWriter(sw);
            Options = options;
            ClassName = GettextResourceManager.MakeResourceSetClassName(Options.BaseName, Options.Locale);
            CsharpSourceFileName = Path.GetRandomFileName();
        }
        #endregion

        public void Run()
        {
            catalog = new Catalog();
            catalog.Load(Options.InputFiles[0]);
            for (int i = 1; i < Options.InputFiles.Count; i++)
            {
                Catalog temp = new Catalog();
                temp.Load(Options.InputFiles[i]);
                catalog.Append(temp);
            }
            Check();
            Generate();
            SaveToFile();
            Compile();
            if (!Options.DebugMode)
                File.Delete(CsharpSourceFileName);
        }

        private void Check()
        {
            if (!Options.CheckFormat)
                return;
            foreach (var entry in catalog.Where(entry => entry.IsInFormat("csharp")))
            {
                ValidateFormatString(entry.String);
                ValidateFormatString(entry.PluralString);
                for (int i = 0; i < entry.TranslationsCount; i++)
                    ValidateFormatString(entry.GetTranslation(i));
            }
        }

        private void ValidateFormatString(string s)
        {
            FormatValidator v = new FormatValidator(s);
            FormatValidateResult result = v.Validate();
            if (!result.Result)
            {
                throw new FormatException(string.Format("Invalid sting format: '{0}'\n{1}", s, result.ErrorMessage));
            }
        }

        private void Generate()
        {
            cw.WriteLine("// This file was generated by GNU msgfmt at {0}", DateTime.Now);
            cw.WriteLine("// Do not modify it!");
            cw.WriteLine();
            cw.WriteLine("using GNU.Gettext;");
            cw.WriteLine();

            /* Assign a strong name to the assembly, so that two different localizations
			 * of the same domain can be loaded one after the other.  This strong name
			 * tells the Global Assembly Cache that they are meant to be different.
			 */

            // Get the assembly that represents the executing code
            Assembly assembly = Assembly.GetExecutingAssembly();

            // Get the target framework of the assembly
            var targetFrameworkAttribute = assembly.GetCustomAttribute<TargetFrameworkAttribute>();

            cw.WriteLine("[assembly: System.Reflection.AssemblyCulture(\"{0}\")]", Options.Locale.Name);
            cw.WriteLine("[assembly: System.Runtime.Versioning.TargetFramework(\"{0}\")]", targetFrameworkAttribute?.FrameworkName);

            if (Options.HasNamespace)
            {
                cw.WriteLine("namespace {0}", Options.BaseName);
                cw.WriteLine("{");
                cw.Indent++;
            }

            cw.WriteLine("public class {0} : {1}",
                ClassName,
                typeof(GettextResourceSet).FullName);
            cw.WriteLine("{");
            cw.Indent++;

            // Constructor
            cw.WriteLine("public {0} () : base(GetResources())", ClassName);
            cw.WriteLine("{ }");
            cw.WriteLine();

            if (catalog.HasHeader(Catalog.PluralFormsHeader))
            {
                cw.WriteLine("public override string PluralForms {{ get {{ return {0}; }} }}", ToConstStr(catalog.GetPluralFormsHeader()));
                cw.WriteLine();
            }

            cw.WriteLine("protected static System.Resources.IResourceReader GetResources() {");
            cw.Indent++; cw.WriteLine("var table = new System.Collections.Hashtable();");
            foreach (CatalogEntry entry in catalog)
            {
                cw.WriteLine("table.Add({0}, {1});", ToMsgid(entry), ToMsgstr(entry));
            }

            cw.WriteLine($"return new {typeof(GettextResourceReader).FullName}(table);");
            cw.Indent--; cw.WriteLine("}");
            cw.WriteLine();

            // Emit the msgid_plural strings. Only used by msgunfmt.
            if (catalog.PluralFormsCount > 0)
            {
                cw.WriteLine("public static System.Collections.Hashtable GetMsgidPluralTable() {");
                cw.Indent++; cw.WriteLine("var t = new System.Collections.Hashtable();");
                foreach (CatalogEntry entry in catalog)
                {
                    if (entry.HasPlural)
                    {
                        cw.WriteLine("t.Add({0}, {1});", ToMsgid(entry), ToMsgstr(entry));
                    }
                }
                cw.WriteLine("return t;");
                cw.Indent--; cw.WriteLine("}");
            }

            cw.Indent--;
            cw.WriteLine("}");

            if (Options.HasNamespace)
            {
                cw.Indent--;
                cw.WriteLine("}");
            }
            cw.WriteLine();
        }

        private void SaveToFile()
        {
            using (StreamWriter writer = new StreamWriter(CsharpSourceFileName, false, Encoding.UTF8))
            {
                writer.WriteLine(sw.ToString());
            }

            AssemblyOutDir = Path.Combine(Path.GetFullPath(Options.OutDir), Options.Locale.Name);
            if (!Directory.Exists(AssemblyOutDir))
                Directory.CreateDirectory(AssemblyOutDir);
            if (!Directory.Exists(AssemblyOutDir))
                throw new Exception(string.Format("Error creating output directory {0}", AssemblyOutDir));
        }

        /// <summary>
        /// Generates resource dll using Roslyn compiler
        /// </summary>
        private void Compile()
        {
            // Create a syntax tree from your code
            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(File.ReadAllText(CsharpSourceFileName));
            File.Delete(CsharpSourceFileName);

            var libDir = Options.LibDir ?? Directory.GetCurrentDirectory();
            var gnuPath = Path.Combine(Path.GetFullPath(libDir), "GNU.Gettext.dll");

            // Create compilation options
            var compilationOptions = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
                .WithOptimizationLevel(OptimizationLevel.Release);

            // Create compilation
            CSharpCompilation compilation = CSharpCompilation
                .Create(GettextResourceManager.GetSatelliteAssemblyName(Options.BaseName))
                .AddSyntaxTrees(syntaxTree)
                .AddReferences(
                MetadataReference.CreateFromFile(typeof(string).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(ResourceWriter).Assembly.Location),
                MetadataReference.CreateFromFile(gnuPath),
                MetadataReference.CreateFromFile(Assembly.Load("System.Runtime").Location))
                .WithOptions(compilationOptions);


            // Emit the assembly
            EmitResult emitResult = compilation.Emit(Path.Combine(AssemblyOutDir, GettextResourceManager.GetSatelliteAssemblyName(Options.BaseName)));

            if (!emitResult.Success)
            {
                Console.WriteLine($"[lang: {Options.LocaleStr}] Error creating the DLL:");
                foreach (Diagnostic diagnostic in emitResult.Diagnostics)
                {
                    Console.WriteLine(diagnostic.ToString());
                }
            }
            else
            {
                Console.WriteLine($"[lang: {Options.LocaleStr}] DLL created successfully!");
            }
        }


        static string ToConstStr(string s)
        {
            return string.Format("@\"{0}\"", s.Replace("\"", "\"\""));
        }

        static string ToMsgid(CatalogEntry entry)
        {
            return ToConstStr(
                entry.HasContext ?
                GettextResourceManager.MakeContextMsgid(entry.Context, entry.String) : entry.String);
        }


        /// <summary>
        /// Write C# code that returns the value for a message.  If the message
        /// has plural forms, it is an expression of type System.String[], otherwise it
        /// is an expression of type System.string.
        /// </summary>
        /// <returns>
        /// The expression (string or string[]) to initialize hashtable associated object.
        /// </returns>
        /// <param name='entry'>
        /// Catalog entry.
        /// </param>
        static string ToMsgstr(CatalogEntry entry)
        {
            StringBuilder sb = new StringBuilder();
            if (entry.HasPlural)
            {
                sb.Append("new System.String[] { ");
                for (int i = 0; i < entry.TranslationsCount; i++)
                {
                    if (i > 0)
                        sb.Append(", ");
                    sb.Append(ToConstStr(entry.GetTranslation(i)));
                }
                sb.Append(" }");
            }
            else
            {
                sb.Append(ToConstStr(entry.GetTranslation(0)));
            }
            return sb.ToString();
        }

    }
}
