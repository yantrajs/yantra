using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Security.Authentication.ExtendedProtection;
using System.Text;
using System.Threading;

namespace YantraJS.JSClassGenerator
{

    [Generator]
    public class JSClassGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var jsClasses = context.SyntaxProvider
                .CreateSyntaxProvider(CouldBeJSClassAsync, GetJSClassTypeOrNull)
                .Where(x => x is not null)
                .Collect();

            context.RegisterSourceOutput(jsClasses, GenerateCode);
        }

        private static bool CouldBeJSClassAsync(
            SyntaxNode syntaxNode,
            CancellationToken cancellationToken)
        {
            if (syntaxNode is not AttributeSyntax attribute)
                return false;

            var name = ExtractName(attribute.Name);
            if (name == null)
            {
                return false;
            }
            return name.StartsWith("JSNameG") || name.StartsWith("JSClassG");
        }

        private static string? ExtractName(NameSyntax? name)
        {
            return name switch
            {
                SimpleNameSyntax ins => ins.Identifier.Text,
                QualifiedNameSyntax qns => qns.Right.Identifier.Text,
                _ => null
            };
        }

        private static ITypeSymbol? GetJSClassTypeOrNull(
            GeneratorSyntaxContext context,
            CancellationToken cancellationToken)
        {
            var attributeSyntax = (AttributeSyntax)context.Node;

            // "attribute.Parent" is "AttributeListSyntax"
            // "attribute.Parent.Parent" is a C# fragment the attributes are applied to
            if (attributeSyntax.Parent?.Parent is not ClassDeclarationSyntax classDeclaration)
                return null;

            var type = context.SemanticModel.GetDeclaredSymbol(classDeclaration) as ITypeSymbol;

            return type is null || !IsEnumeration(type) ? null : type;
        }

        private static bool IsEnumeration(ISymbol type)
        {
            return GetAttribute(type) != null;
        }

        private static AttributeData? GetAttribute(ISymbol type)
        {
            var aName = type.GetAttributes().FirstOrDefault(x => x.AttributeClass?.Name != null);
            var name = aName?.AttributeClass?.Name;
            if (name == null)
                return null;
            if (name.StartsWith("JSClassGenerator") || name.StartsWith("JSName"))
                return aName;
            return null;
            //return type.GetAttributes()
            //           .FirstOrDefault(a => (a.AttributeClass?.Name == "JSClassGeneratorAttribute" || a.AttributeClass?.Name == "JSClassGenerator")
            //                    //&& a.AttributeClass.ContainingNamespace is
            //                    // {
            //                    //     Name: "Yantra.Core",
            //                    //     // ContainingNamespace.IsGlobalNamespace: true
            //                    // }
            //                     );
        }

        private static void GenerateCode(
            SourceProductionContext context,
            ImmutableArray<ITypeSymbol> enumerations)
        {
            if (enumerations.IsDefaultOrEmpty)
                return;

            var types = new List<(ITypeSymbol type, AttributeData attribute)>();
            ITypeSymbol names = null!;
            

            

            // first lets build all classes

            // then build names...

            foreach (var type in enumerations)
            {
                var a = GetAttribute(type);
                if (a?.AttributeClass?.Name == null)
                {
                    continue;
                }
                if (a.AttributeClass.Name.StartsWith("JSName"))
                {
                    names = type;
                    continue;
                }
                types.Add((type,a));
            }

            var allNames = new List<string>();
            foreach (var name in names.GetMembers().Where(x => x.Kind == SymbolKind.Field))
            {
                allNames.Add(name.Name);
            }


            foreach(var (type,a) in  types)
            {
                var code = "";
                code = GenerateCode(type, a, allNames);
                var typeNamespace = type.ContainingNamespace.IsGlobalNamespace
                    ? null
                        : $"{type.ContainingNamespace}.";

                context.AddSource($"{typeNamespace}{type.Name}.g.cs", code);
            }

            var c = GenerateNames(names, allNames);
            var cNS = names.ContainingNamespace.IsGlobalNamespace
                ? null
                    : $"{names.ContainingNamespace}.";

            context.AddSource($"{cNS}{names.Name}.g.cs", c);
        }

        private static  string GenerateNames(ITypeSymbol type, List<string> names)
        {
            var sb = new StringBuilder();

            sb = sb.AppendLine("using System.Collections.Generic;")
                .AppendLine("using System.Runtime.CompilerServices;")
                .AppendLine("using System.Text;")
                .AppendLine("using YantraJS.Core.Clr;")
                .AppendLine("using YantraJS.Core.Runtime;")
                .AppendLine("using YantraJS.Extensions;");

            var ns = type.ContainingNamespace.ToString();
            if (ns != "YantraJS.Core")
            {
                sb = sb.AppendLine("using YantraJS.Core;");
            }

            sb = sb.AppendLine($"namespace {ns} {{ ");

            sb = sb.AppendLine($"partial class {type.Name} {{");

            foreach(var name in names)
            {
                if (type.GetMembers(name).Any(x => x.Name == name))
                    continue;
                sb.AppendLine($"public static readonly KeyString {name};");
            }

            sb.AppendLine($"static {type.Name}() {{");
            foreach(var name in names)
            {
                sb.AppendLine($"{type.Name}.{name} = \"{name}\";");
            }
            sb.AppendLine("}");

            sb.AppendLine("}");
            sb.AppendLine("}");


            return sb.ToString();
        }

        private static string GenerateCode(ITypeSymbol type, AttributeData ad, List<string> names)
        {
            // get attribute info...

            var sb = new StringBuilder();

            try
            {

                sb = sb.AppendLine("using System.Collections.Generic;")
                    .AppendLine("using System.Runtime.CompilerServices;")
                    .AppendLine("using System.Text;")
                    .AppendLine("using YantraJS.Core.Clr;")
                    .AppendLine("using YantraJS.Core.Runtime;")
                    .AppendLine("using YantraJS.Extensions;");

                var ns = type.ContainingNamespace.ToString();
                if (ns != "YantraJS.Core")
                {
                    sb = sb.AppendLine("using YantraJS.Core;");
                }

                sb = sb.AppendLine($"namespace {ns} {{ ");

                sb = sb.AppendLine($"partial class {type.Name} {{");

                var className = type.Name;

                if (ad.ConstructorArguments.Length > 0)
                {
                    className = ad.ConstructorArguments[0].Value?.ToString() ?? type.Name;
                }

                sb = sb.AppendLine($"internal protected {type.Name}(): base((JSContext.Current[{names.GetOrCreateName(type.Name)}] as JSFunction).prototype) {{}}");

                // generate each marked method...
                sb.AppendLine("#pragma warning disable CS0109");
                sb.AppendLine("public static new void RegisterClass(JSContext context) {");
                sb.AppendLine("#pragma warning restore CS0109");

                sb.AppendLine($@"
                var @class = new JSFunction((in Arguments a) => new {type.Name}(in a), ""{className}"");
                context[Names.{className}] = @class;
                var prototype = @class.prototype;
            ");

                foreach (var member in type.GetMembers())
                {
                    GenerateMember(sb, member, type, names);
                }

                sb.AppendLine("}");

                sb = sb.AppendLine("}");
                sb = sb.AppendLine("}");
            } catch (Exception ex)
            {
                sb.AppendLine("/**");
                sb.AppendLine(ex.ToString());
                sb.AppendLine("*/");
            }

            return sb.ToString();
        }

        private static void GenerateMember(StringBuilder sb, ISymbol member, ITypeSymbol type, List<string> names)
        {
            // needs JSExport


            var exports = member.GetAttributes().FirstOrDefault(x =>
                x.AttributeClass?.Name?.StartsWith("JSExport")
                ?? x.AttributeClass?.Name?.StartsWith("JSExportSameName")
                ?? false);

            if (exports == null)
            {
                return;
            }

            sb.AppendLine($"// Begin {member.Name}");

            var name = member.Name.ToCamelCase();

            sb.AppendLine($"// Export As {name}");

            if (exports.AttributeClass?.Name?.StartsWith("vJSExportSameName") ?? false)
            {
                name = member.Name;
            }
            if(exports.ConstructorArguments.Length > 0)
            {
                name = exports.ConstructorArguments.FirstOrDefault().Value?.ToString() ?? name;
            }

            sb.AppendLine($"// Generating {member.Name}");

            var target = member.IsStatic
                    ? "@class"
                    : "prototype";

            switch (member.Kind)
            {
                case SymbolKind.Field:

                    GenerateField(sb, target, name, (member as IFieldSymbol)!, type, names);

                    break;
                case SymbolKind.Method:

                    GenerateMethod(sb, target, name, (member as IMethodSymbol)!, type, names);

                    break;
                case SymbolKind.Property:
                    GenerateProperty(sb, target, name, (member as IPropertySymbol)!, type, names);
                    break;
            }
        }

        private static void GenerateField(
            StringBuilder sb,
            string target,
            string name,
            IFieldSymbol method,
            ITypeSymbol type, List<string> names)
        {
            var t = $"throw JSContext.Current.NewTypeError(\"Failed to convert this to {type.Name}\")";
            var keyName = names.GetOrCreateName(name);

            string setter = "null";
            string getter = "null";

            var access = !method.IsStatic
                ? $"@this.{method.Name}"
                : $"{type.Name}.{method.Name}";

            var clrProxyMarshal = access.ClrProxyMarshal(method.Type, name);
            var toClr = "a[0]".ToJSValueFromClr(method.Type, name);

            if (!method.IsStatic)
            {
                getter = @$"new JSFunction((in Arguments a) =>
                    a.This is {type.Name} @this
                        ? {clrProxyMarshal}
                        : {t} ,
                ""get {name}"")";
                if (!method.IsReadOnly)
                {
                    setter = @$"new JSFunction((in Arguments a) => {{
                    if(a.This is {type.Name} @this) {{
                         {access} = {toClr};
                    }}
                        else {t};
                    return JSUndefined.Value;
                }},
                ""set {name}"")";
                }
            }
            else
            {
                getter = @$"new JSFunction((in Arguments a) =>
                    {clrProxyMarshal},
                ""get {name}"")";
                if (!method.IsReadOnly)
                {
                    setter = @$"new JSFunction((in Arguments a) => {{
                    {access} = {toClr};
                    return JSUndefined.Value;
                }},
                ""set {name}"")";
                }
            }

            sb.AppendLine(@$"{target}.FastAddProperty(
                {keyName},
                {getter},
                {setter},
                JSPropertyAttributes.EnumerableConfigurableValue);");

        }

        private static void GenerateProperty(
            StringBuilder sb,
            string target,
            string name,
            IPropertySymbol method,
            ITypeSymbol type, List<string> names)
        {
            var t = $"throw JSContext.Current.NewTypeError(\"Failed to convert this to {type.Name}\")";
            var keyName = names.GetOrCreateName(name);

            string setter = "null";
            string getter = "null";

            var access = !method.IsStatic
                ? $"@this.{method.Name}"
                : $"{type.Name}.{method.Name}";

            var clrProxyMarshal = access.ClrProxyMarshal(method.Type, name);
            var toClr = "a[0]".ToJSValueFromClr(method.Type, name);

            if (!method.IsStatic)
            {
                getter = @$"new JSFunction((in Arguments a) =>
                    a.This is {type.Name} @this
                        ? {clrProxyMarshal}
                        : {t} ,
                ""get {name}"")";
                if (!method.IsReadOnly)
                {
                    setter = @$"new JSFunction((in Arguments a) => {{
                    if(a.This is {type.Name} @this) {{
                         {access} = {toClr};
                    }}
                        else {t};
                    return JSUndefined.Value;
                }},
                ""set {name}"")";
                }
            }
            else
            {
                getter = @$"new JSFunction((in Arguments a) =>
                    {clrProxyMarshal},
                ""get {name}"")";
                if (!method.IsReadOnly)
                {
                    setter = @$"new JSFunction((in Arguments a) => {{
                    {access} = {toClr};
                    return JSUndefined.Value;
                }},
                ""set {name}"")";
                }
            }

            sb.AppendLine(@$"{target}.FastAddProperty(
                {keyName},
                {getter},
                {setter},
                JSPropertyAttributes.EnumerableConfigurableValue);");

        }

        private static void GenerateMethod(
            StringBuilder sb, 
            string target, 
            string name,
            IMethodSymbol method, 
            ITypeSymbol type, List<string> names)
        {
            var t = $"throw JSContext.Current.NewTypeError(\"Failed to convert this to {type.Name}\")";
            var keyName = names.GetOrCreateName(name);
            if (method.IsJSFunction())
            {
                var fx = $"new JSFunction({type.Name}.{method.Name}, \"{name}\")";
                if (!method.IsStatic)
                {
                    fx = @$"new JSFunction((in Arguments a) =>
                    a.This is {type.Name} @this
                        ? @this.{method.Name}(in a)
                        : {t} ,
                ""{name}"")";
                }
                sb.AppendLine($"{target}.FastAddValue({keyName}, {fx}, JSPropertyAttributes.EnumerableConfigurableValue);");
                return;
            }

            // sb.AppendLine($"{method.Name} not generated due to incompatible parameter types");


            var fb = new StringBuilder();
            fb.AppendLine($"(in Arguments a) => {{");
            var calls = new List<string>();
            int i = 0;
            var callee = type.Name;
            if (!method.IsStatic)
            {
                callee = "@this";
                fb.AppendLine($"if(!(a.This is {type.Name} @this))\r\n\t\t\t\t{t};");
            }
            foreach (var p in method.Parameters)
            {
                calls.Add(p.Name);
                var v = $"a[{i}]".ToJSValueFromClr(p.Type, p.Name);
                fb.AppendLine($"var {p.Name} = {v};");
            }
            var args = string.Join(", ", calls);

            if (method.ReturnType.Name == "Void")
            {
                fb.AppendLine($"{callee}.{method.Name}({args});");
                fb.AppendLine("return JSUndefined.Value;");
            }
            else if (method.ReturnType.Name == "JSValue")
            {
                fb.AppendLine($"var @return = {callee}.{method.Name}({args});");
                fb.AppendLine("return @return;");
            }
            else
            {
                fb.AppendLine($"var @return = {callee}.{method.Name}({args});");
                fb.AppendLine($"return ClrProxy.Marshal(@return);");
            }
            fb.AppendLine("}");

            var body = @$"new JSFunction({fb.ToString().Replace("\n", "\n\t\t\t")},
                ""{method.Name}""
            )";

            sb.AppendLine($"{target}.FastAddValue({keyName}, {body}, JSPropertyAttributes.EnumerableConfigurableValue);");
        }
    }
}
