using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace YantraJS.JSClassGenerator
{
    public class JSTypeInfo
    {
        public readonly string ClrClassName;
        public readonly string JSClassName;
        public readonly string? BaseClrClassName;
        public readonly string? BaseJSClassName;
        public readonly ITypeSymbol Type;
        public string Name => Type.Name;
        public INamespaceSymbol ContainingNamespace => Type.ContainingNamespace;

        public readonly bool InternalClass;

        public readonly string? ConstructorLength;
        public readonly string? ConstructorMethod;
        public readonly bool GenerateClass;
        public readonly List<JSExportInfo> Members;
        public readonly bool Register = true;
        public readonly bool Globals = false;

        public JSTypeInfo(ITypeSymbol type)
        {
            Type = type;
            this.ConstructorLength = null;

            var className = type.Name;

            this.InternalClass = false;

            foreach(var attribute in type.GetAttributes())
            {
                switch(attribute.AttributeClass?.Name)
                {
                    case "JSClassGenerator":
                    case "JSClassGeneratorAttribute":
                        if(attribute.ConstructorArguments.Length > 0)
                        {
                            className = attribute.ConstructorArguments[0].Value?.ToString() ?? className;
                        }
                        GenerateClass = true;
                        break;
                    case "JSFunctionGenerator":
                    case "JSFunctionGeneratorAttribute":
                        if (attribute.ConstructorArguments.Length > 0)
                        {
                            className = attribute.ConstructorArguments[0].Value?.ToString() ?? className;
                        }
                        if(attribute.NamedArguments.Length > 0)
                        {
                            var register = attribute.NamedArguments.FirstOrDefault(x => x.Key == "Register");
                            if (register.Key == "Register")
                            {
                                Register = register.Value.ToCSharpString() != "false";
                            }
                            var globals = attribute.NamedArguments.FirstOrDefault(x => x.Key == "Globals");
                            if (globals.Key == "Globals")
                            {
                                Globals = globals.Value.ToCSharpString() == "true";
                            }
                        }
                        break;
                    case "JSBaseClass":
                    case "JSBaseClassAttribute":
                        if(attribute.ConstructorArguments.Length > 0)
                        {
                            BaseJSClassName = attribute.ConstructorArguments[0].Value?.ToString();
                        }
                        break;
                    case "JSInternalObject":
                    case "JSInternalObjectAttribute":
                        InternalClass = true;
                        break;
                }
            }

            this.ClrClassName = type.Name;
            this.JSClassName = className;
            var members = new List<JSExportInfo>();

            foreach(var m in type.GetMembers())
            {
                var e = m.GetExportAttribute();
                if (e != null)
                {
                    if (m is IMethodSymbol method)
                    {
                        if (method.IsConstructor())
                        {
                            this.ConstructorLength = e.Length;
                            continue;
                        }
                        if (e.IsConstructor)
                        {
                            this.ConstructorLength = e.Length;
                            this.ConstructorMethod = method.Name;
                            continue;
                        }
                    }
                    members.Add(e);
                }
            }

            this.Members = members;

            if (type.BaseType == null)
            {
                return;
            }

            if (type.BaseType?.Name == "JSObject")
            {
                return;
            }

            BaseClrClassName = type.BaseType!.Name;
            if (string.IsNullOrWhiteSpace(BaseJSClassName))
            {
                BaseJSClassName = BaseClrClassName;
            }
        }
    }

    public class JSGeneratorContext
    {
        public readonly List<string> Names = new List<string>();

        public readonly List<JSTypeInfo> RegistrationOrder = new List<JSTypeInfo>();

        public List<JSTypeInfo> AssemblyTypes;

        public JSGeneratorContext(List<(ITypeSymbol type, AttributeData attribute)> types)
        {
            this.AssemblyTypes = types.Select((x) => {
                return new JSTypeInfo(x.type);
            }).ToList();

            BuildOrder(AssemblyTypes.ToList());
        }

        private void BuildOrder(List<JSTypeInfo> types)
        {
            while(types.Count > 0)
            {
                var all = types.ToList();
                foreach(var item in all)
                {
                    if(item.BaseClrClassName == null)
                    {
                        RegistrationOrder.Add(item);
                        types.Remove(item);
                        continue;
                    }

                    // if BaseJSClassName does not exist in AssmeblyTypes...
                    if (AssemblyTypes.FindIndex((x) => x.ClrClassName == item.BaseClrClassName) == -1)
                    {
                        RegistrationOrder.Add(item);
                        types.Remove(item);
                        continue;
                    }

                    // check if BaseJSClassName exists...
                    if (RegistrationOrder.FindIndex((x) => x.ClrClassName== item.BaseClrClassName) != -1)
                    {
                        RegistrationOrder.Add(item);
                        types.Remove(item);
                    }

                }
            }
        }
    }
}
