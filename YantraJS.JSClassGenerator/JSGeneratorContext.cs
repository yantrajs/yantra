using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace YantraJS.JSClassGenerator
{
    public readonly struct JSTypeInfo: IEquatable<JSTypeInfo>
    {
        public readonly string ClrClassName;
        public readonly string JSClassName;
        public readonly string? BaseClrClassName;
        public readonly string? BaseJSClassName;
        public readonly ITypeSymbol Type;
        public readonly string Name => Type.Name;
        public INamespaceSymbol ContainingNamespace => Type.ContainingNamespace;

        public override bool Equals(object obj)
        {
            if (obj is JSTypeInfo jt)
            {
                return jt.Equals(this);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Type.GetHashCode();
        }

        public bool Equals(JSTypeInfo other)
        {
            return Type == other.Type;
        }

        public JSTypeInfo(ITypeSymbol type)
        {
            Type = type;

            var attribute = type.GetAttribute()!;

            var className = type.Name;

            if (attribute.ConstructorArguments.Length > 0)
            {
                className = attribute.ConstructorArguments[0].Value?.ToString() ?? type.Name;
            }

            this.ClrClassName = type.Name;
            this.JSClassName = className;

            if (type.BaseType == null)
            {
                return;
            }

            if (type.BaseType?.Name == "JSObject")
            {
                return;
            }

            BaseClrClassName = type.BaseType!.Name;
            if (attribute.ConstructorArguments.Length > 1)
            {
                BaseJSClassName = attribute.ConstructorArguments[1].Value?.ToString() ?? BaseClrClassName;
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
                    if (AssemblyTypes.FindIndex((x) => x.JSClassName == item.BaseJSClassName) == -1)
                    {
                        RegistrationOrder.Add(item);
                        types.Remove(item);
                    }

                    // check if BaseJSClassName exists...
                    if (RegistrationOrder.FindIndex((x) => x.JSClassName == item.BaseJSClassName) != -1)
                    {
                        RegistrationOrder.Add(item);
                        types.Remove(item);
                    }

                }
            }
        }
    }
}
