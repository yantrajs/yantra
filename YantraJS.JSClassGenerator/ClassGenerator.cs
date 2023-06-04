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
        static bool IsPrimitive(string name)
        {
            switch(name)
            {
                case "Number":
                case "Boolean":
                case "String":
                case "Symbol":
                    return true;
            }
            return false;
        }

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
                    .AppendLine("using YantraJS.Extensions;");

                var ns = type.ContainingNamespace.ToString();
                if (ns != "YantraJS.Core")
                {
                    sb = sb.AppendLine("using YantraJS.Core;");
                }

                sb = sb.AppendLine($"namespace {ns} {{ ");

                sb = sb.AppendLine($"partial class {type.Name} {{");

                var className = type.JSClassName;
                var hasBaseClasse = type.BaseClrClassName != null;

                if (!type.Globals)
                {

                    names.GetOrCreateName(className);

                    if (IsPrimitive(className))
                    {
                        sb = sb.AppendLine($"protected override JSObject GetCurrentPrototype() => null;");
                    }
                    else
                    {
                        sb = sb.AppendLine($"protected override JSObject GetCurrentPrototype() => (JSContext.Current?[{names.GetOrCreateName(className)}] as JSFunction)?.prototype;");
                    }

                    sb = sb.AppendLine($"internal protected {type.Name}(JSObject prototype = null): base(prototype) {{}}");

                    // sb = sb.AppendLine($"protected {type.Name}(JSObject prototype): base(prototype ?? throw new System.ArgumentException(\"Prototype not specified...\")) {{}}");
                }

                var createClassReturnType = "JSFunction";
                if(type.InternalClass || type.Globals)
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
                } else if (type.Globals)
                {
                    sb.AppendLine($@"
                    var @class = context;");
                }
                else {
                    var l = type.ConstructorLength ?? "";
                    if (l.Length>0)
                    {
                        l = ", length:" + l;
                    }
                    var fxToString = $"function {className}() {{ [native code] }}";

                    var clrFunctionType = type.GenerateClass ? "JSClassFunction" : "JSFunction";

                    if (type.ConstructorMethod == null)
                    {

                        sb.AppendLine($@"
                        var @class = new {clrFunctionType}((in Arguments a) => new {type.Name}(in a)
                            , ""{className}""
                            , ""{fxToString}""
                            {l});
                        if (register) {{
                            context[Names.{className}] = @class;
                        }}
                        var prototype = @class.prototype;
                        ");
                    } else
                    {
                        sb.AppendLine($@"
                        var @class = new {clrFunctionType}({type.Name}.{type.ConstructorMethod}
                            , ""{className}""
                            , ""{fxToString}""
                            {l});
                        if (register) {{
                            context[Names.{className}] = @class;
                        }}
                        var prototype = @class.prototype;
                        ");

                    }
                }

                if (hasBaseClasse)
                {
                    if (type.BaseJSClassName != "Object")
                    {
                        sb.AppendLine($" var @base = context[\"{type.BaseJSClassName}\"] as JSFunction;");
                        sb = sb.AppendLine($"@class.SetPrototypeOf(@base);");
                        if (!type.InternalClass)
                        {
                            sb = sb.AppendLine($"prototype.SetPrototypeOf(@base.prototype);");
                        }
                    }
                } else
                {
                    if (className != "Object")
                    {
                        // insert in list..
                        

                        //sb.AppendLine($" var @base = context[KeyStrings.Object] as JSFunction;");
                        //sb = sb.AppendLine($"@class.SetPrototypeOf(@base);");
                        //if (!type.InternalClass)
                        //{
                        //    sb = sb.AppendLine($"prototype.SetPrototypeOf(@base.prototype);");
                        //}
                    }
                }

                foreach (var member in type.Members)
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

        private void GenerateMember(StringBuilder sb, JSExportInfo exports)
        {
            sb.AppendLine($"// Exporting {exports.Member.Name} as {exports.Name}");

            var target = exports.IsPrototypeMethod || !exports.Member.IsStatic
                    ? "prototype"
                    : "@class";

            var name = exports.Name;
            var member = exports.Member;

            switch (exports.Member.Kind)
            {
                case SymbolKind.Field:

                    GenerateField(sb, target, name, exports);

                    break;
                case SymbolKind.Method:

                    GenerateMethod(sb, target, name, exports);

                    break;
                case SymbolKind.Property:
                    GenerateProperty(sb, target, name, exports);
                    break;
            }
        }

        private void GenerateField(
            StringBuilder sb,
            string target,
            string name,
            JSExportInfo exports)
        {
            var method = exports.Field!;

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
                JSPropertyAttributes.ReadonlyValue);");
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
                JSPropertyAttributes.ConfigurableProperty);");

        }

        private void GenerateProperty(
            StringBuilder sb,
            string target,
            string name,
            JSExportInfo exports)
        {
            var method = exports.Property!;
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
                JSPropertyAttributes.ConfigurableProperty);");

        }

        private void GenerateMethod(
            StringBuilder sb,
            string target,
            string name,
            JSExportInfo exports)
        {
            bool usePrototypeTarget = exports.IsPrototypeMethod;
            var method = exports.Method!;
            var e = exports;
            if (e.IsConstructor)
            {
                return;
            }

            var keyName = names.GetOrCreateName(name);
            var fx = GenerateMethodBody(name, exports);

            if (e.Symbol == null)
            {
                sb.AppendLine($"{target}.FastAddValue({keyName}, {fx}, JSPropertyAttributes.ConfigurableValue);");
                return;
            }

            sb.AppendLine("{");
            sb.AppendLine($"var fx = {fx};");
            fx = "fx";
            sb.AppendLine($"{target}.FastAddValue({keyName}, {fx}, JSPropertyAttributes.ConfigurableValue);");
            sb.AppendLine($"{target}.FastAddValue( JSSymbol.GlobalSymbol(\"{e.Symbol}\"), {fx}, JSPropertyAttributes.ConfigurableValue);");
            sb.AppendLine("}");
        }

        private string GenerateMethodBody(
            string name,
            JSExportInfo e)
        {
            var method = e.Method!;
            
            var l = $",\"function {name}() {{ [native] }}\", createPrototype: false";
            if (e?.Length != null && e.Length.Length > 0) {
                l += ", length: " + e.Length;
            }

            var t = $"throw JSContext.Current.NewTypeError(\"Failed to convert this to {type.Name}\")";
            if (method.IsJSFunction())
            {
                var fx = $"new JSFunction({type.Name}.{method.Name}, \"{name}\" {l})";
                if (!method.IsStatic)
                {
                    fx = @$"new JSFunction((in Arguments a) =>
                    a.This is {type.Name} @this
                        ? @this.{method.Name}(in a)
                        : {t}
                        , ""{name}""
                        {l})";
                }
                return fx;
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
                calls.Add("p" + p.Name);
                var v = $"a[{i++}]".ToJSValueFromClr(p.Type, p.Name);
                fb.AppendLine($"var p{p.Name} = {v};");
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

            // sb.AppendLine($"{target}.FastAddValue({keyName}, {body}, JSPropertyAttributes.ConfigurableValue);");
            return body;
        }

    }
}
