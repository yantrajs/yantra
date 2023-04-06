using Microsoft.CodeAnalysis;
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

        public readonly bool GenerateClass;

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

            foreach(var m in type.GetMembers())
            {
                if(m is IMethodSymbol method && method.IsConstructor())
                {
                    var e = method.GetExportAttribute();
                    if (e != null)
                    {
                        this.ConstructorLength = e.Length;
                        break;
                    }
                }
            }

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
