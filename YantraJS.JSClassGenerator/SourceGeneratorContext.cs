using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Yantra.Core;

namespace YantraJS.JSClassGenerator;

internal class SourceGeneratorContext
{
    private CSharpCompilation compilation;
    private List<SyntaxTree> syntaxTrees;

    public SourceGeneratorContext()
    {
        var compilation = CSharpCompilation.Create("AnalysisAssembly")
        .AddReferences(
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(Path.Combine(RuntimeEnvironment.GetRuntimeDirectory(), "netstandard.dll")),
            MetadataReference.CreateFromFile(Path.Combine(RuntimeEnvironment.GetRuntimeDirectory(), "System.Runtime.dll")),
            MetadataReference.CreateFromFile(typeof(JSBaseClassAttribute).Assembly.Location)
        );
        this.compilation = compilation;

        this.syntaxTrees = new List<SyntaxTree>();
    }

    public void AddFiles(DirectoryInfo folder)
    {
        foreach (var entry in folder.EnumerateFileSystemInfos())
        {
            if (entry is DirectoryInfo d)
            {
                AddFiles(d);
                continue;
            }
            if (entry is FileInfo file
                && file.Extension.Equals(".cs", StringComparison.OrdinalIgnoreCase)
                && !file.Name.EndsWith(".g.cs", StringComparison.OrdinalIgnoreCase)
                )
            {
                AddFile(file);
            }
        }
    }

    private void AddFile(FileInfo file)
    {
        var code = System.IO.File.ReadAllText(file.FullName);
        SyntaxTree tree = CSharpSyntaxTree.ParseText(code, path: file.FullName);

        // compilation = compilation.AddSyntaxTrees(tree);
        this.syntaxTrees.Add(tree);
    }

    List<ITypeSymbol> GetAllTypes()
    {
        var list = new List<ITypeSymbol>();
        foreach (var st in compilation.SyntaxTrees)
        {
            var sm = compilation.GetSemanticModel(st);

            var classDeclarations = st.GetRoot()
                .DescendantNodes()
                .OfType<ClassDeclarationSyntax>();

            foreach (var cd in classDeclarations)
            {
                var type = sm.GetDeclaredSymbol(cd);
                var a = type.GetAttribute();
                if (a != null)
                {
                    if (list.Find((x) => x.Name == type.Name) == null)
                    {
                        list.Add(type);
                    }
                }
            }
        }
        return list;
    }

    public void GenerateSource(string outputFolder)
    {

        compilation = compilation.AddSyntaxTrees(this.syntaxTrees);


        var types = new List<(ITypeSymbol type, AttributeData attribute)>();
        ITypeSymbol names = null!;


        var allTypes = GetAllTypes();

        foreach (var type in allTypes)
        {
            var a = type.GetAttribute();
            if (a?.AttributeClass?.Name == null)
            {
                continue;
            }
            if (a.AttributeClass.Name.StartsWith("JSRegistration"))
            {
                names = type;
                continue;
            }
            types.Add((type, a));
        }

        if(types.Count == 0)
        {
            throw new Exception("No types found");
        }

        var allNames = new List<string>();
        foreach (var name in names.GetMembers().Where(x => x.Kind == SymbolKind.Field))
        {
            allNames.Add(name.Name);
        }

        var gc = new JSGeneratorContext(types);

        var oFolder = new System.IO.DirectoryInfo(outputFolder);
        if (!oFolder.Exists)
        {
            oFolder.Create();
        }

        foreach (var type in gc.AssemblyTypes)
        {
            var code = "";
            code = ClassGenerator.GenerateClass(type, gc);
            var typeNamespace = type.Type.ContainingNamespace.IsGlobalNamespace
                ? null
                    : $"{type.Type.ContainingNamespace}.";

            var fileName = $"{typeNamespace}{type.Type.Name}.g.cs";
            System.IO.File.WriteAllText(System.IO.Path.Join(outputFolder, fileName), code);
        }

        var c = RegistrationGenerator.GenerateNames(names, gc);
        var cNS = names.ContainingNamespace.IsGlobalNamespace
            ? null
                : $"{names.ContainingNamespace}.";

        var namesFile = $"{outputFolder}/{cNS}{names.Name}.g.cs";
        System.IO.File.WriteAllText(namesFile,c);
    }
}
