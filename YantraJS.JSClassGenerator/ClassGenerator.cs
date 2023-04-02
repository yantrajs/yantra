using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace YantraJS.JSClassGenerator
{
    internal class ClassGenerator
    {
        private JSTypeInfo type;
        private readonly JSGeneratorContext gc;
        private List<string> names => gc.Names;
        public ClassGenerator(in JSTypeInfo type, JSGeneratorContext gc)
        {
            this.type = type;
            this.gc = gc;
        }

        public static string GenerateClass(in JSTypeInfo type,
            JSGeneratorContext gc)
        {
            var cg = new ClassGenerator(type, gc);
            return cg.GenerateCode();
        }

        private string GenerateCode()
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

                var className = type.JSClassName;

                sb = sb.AppendLine($"internal protected {type.Name}(): base((JSContext.Current[{names.GetOrCreateName(className)}] as JSFunction).prototype) {{}}");

                var hasBaseClasse = type.BaseClrClassName != null;

                var createClassReturnType = "JSFunction";
                if(type.InternalClass)
                {
                    createClassReturnType = "JSObject";
                }

                // generate each marked method...
                if (hasBaseClasse)
                {
                    sb.AppendLine($"public static new {createClassReturnType} CreateClass(JSContext context, bool register = true) {{");
                }
                else
                {
                    sb.AppendLine($"public static {createClassReturnType} CreateClass(JSContext context, bool register = true) {{");
                }


                if (type.InternalClass) {
                    sb.AppendLine($@"
                    var @class = new JSObject();
                    if (register) {{
                        context[Names.{className}] = @class;
                    }}
                ");
                }
                else {
                    var l = type.ConstructorLength ?? "";
                    if (l.Length>0)
                    {
                        l = ", length:" + l;
                    }
                    var fxToString = $"function {className}() {{ [native code] }}";
                    sb.AppendLine($@"
                    var @class = new JSFunction((in Arguments a) => new {type.Name}(in a)
                        , ""{className}""
                        , ""{fxToString}""
                        {l});
                    if (register) {{
                        context[Names.{className}] = @class;
                    }}
                    var prototype = @class.prototype;
                ");
                }

                if (hasBaseClasse)
                {
                    sb.AppendLine($" var @base = context[\"{type.BaseJSClassName}\"] as JSFunction;");
                    sb = sb.AppendLine($"@class.SetPrototypeOf(@base);");
                    if (!type.InternalClass)
                    {
                        sb = sb.AppendLine($"prototype.SetPrototypeOf(@base.prototype);");
                    }
                } else
                {
                    if (className != "Object")
                    {
                        // insert in list..
                        

                        sb.AppendLine($" var @base = context[KeyStrings.Object] as JSFunction;");
                        sb = sb.AppendLine($"@class.SetPrototypeOf(@base);");
                        if (!type.InternalClass)
                        {
                            sb = sb.AppendLine($"prototype.SetPrototypeOf(@base.prototype);");
                        }
                    }
                }

                foreach (var member in type.Type.GetMembers())
                {
                    GenerateMember(sb, member);
                }

                sb.AppendLine("return @class;");
                sb.AppendLine("}");

                sb = sb.AppendLine("}");
                sb = sb.AppendLine("}");
            }
            catch (Exception ex)
            {
                sb.AppendLine("/**");
                sb.AppendLine(ex.ToString());
                sb.AppendLine("*/");
            }

            return sb.ToString();
        }

        private void GenerateMember(StringBuilder sb, ISymbol member)
        {
            // needs JSExport


            var exports = member.GetAttributes().FirstOrDefault(x =>
                x.AttributeClass?.Name?.StartsWith("JSExport")
                ?? x.AttributeClass?.Name?.StartsWith("JSExportSameName")
                ?? false);

            var isStaticOrPrototype = member.IsStatic
                || member.GetAttributes().Any((x) => x.AttributeClass?.Name?.StartsWith("JSPrototypeMethod") ?? false);

            if (exports == null)
            {
                return;
            }

            sb.AppendLine($"// Begin {member.Name}");

            var name = member.Name.ToCamelCase();

            sb.AppendLine($"// Export As {name}");

            if (exports.AttributeClass?.Name?.StartsWith("JSExportSameName") ?? false)
            {
                name = member.Name;
            }
            if (exports.ConstructorArguments.Length > 0)
            {
                name = exports.ConstructorArguments.FirstOrDefault().Value?.ToString() ?? name;
            }

            sb.AppendLine($"// Generating {member.Name}");

            var target = isStaticOrPrototype
                    ? "@class"
                    : "prototype";

            switch (member.Kind)
            {
                case SymbolKind.Field:

                    GenerateField(sb, target, name, (member as IFieldSymbol)!);

                    break;
                case SymbolKind.Method:

                    GenerateMethod(sb, target, name, (member as IMethodSymbol)!, isStaticOrPrototype);

                    break;
                case SymbolKind.Property:
                    GenerateProperty(sb, target, name, (member as IPropertySymbol)!);
                    break;
            }
        }

        private void GenerateField(
            StringBuilder sb,
            string target,
            string name,
            IFieldSymbol method)
        {

            var t = $"throw JSContext.Current.NewTypeError(\"Failed to convert this to {type.Name}\")";

            var access = !method.IsStatic
                ? $"@this.{method.Name}"
                : $"{type.Name}.{method.Name}";

            var clrProxyMarshal = access.ClrProxyMarshal(method.Type, name);
            var toClr = "a[0]".ToJSValueFromClr(method.Type, name);
            var keyName = names.GetOrCreateName(name);

            if (method.IsStatic && method.IsReadOnly)
            {
                sb.AppendLine(@$"{target}.FastAddValue(
                {keyName},
                {clrProxyMarshal},
                JSPropertyAttributes.EnumerableConfigurableValue);");
                return;
            }
            string setter = "null";
            string getter = "null";


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
                JSPropertyAttributes.EnumerableConfigurableProperty);");

        }

        private void GenerateProperty(
            StringBuilder sb,
            string target,
            string name,
            IPropertySymbol method)
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
                JSPropertyAttributes.EnumerableConfigurableProperty);");

        }

        private void GenerateMethod(
            StringBuilder sb,
            string target,
            string name,
            IMethodSymbol method,
            bool isStaticOrPrototype)
        {
            if (method.IsConstructor())
            {
                return;
            }

            var e = method.GetExportAttribute();

            var l = $",\"function {name}() {{ [native] }}\", createPrototype: false";
            if (e?.Length != null && e.Length.Length > 0) {
                l += ", length: " + e.Length;
            }

            var t = $"throw JSContext.Current.NewTypeError(\"Failed to convert this to {type.Name}\")";
            var keyName = names.GetOrCreateName(name);
            if (method.IsJSFunction())
            {
                var fx = $"new JSFunction({type.Name}.{method.Name}, \"{name}\" {l})";
                if (!isStaticOrPrototype)
                {
                    fx = @$"new JSFunction((in Arguments a) =>
                    a.This is {type.Name} @this
                        ? @this.{method.Name}(in a)
                        : {t}
                        , ""{name}""
                        {l})";
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
                {l}
            )";

            sb.AppendLine($"{target}.FastAddValue({keyName}, {body}, JSPropertyAttributes.EnumerableConfigurableValue);");
        }

    }
}
