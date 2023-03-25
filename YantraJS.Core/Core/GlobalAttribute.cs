#nullable enable
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using YantraJS.Core;
using YantraJS.Core.Clr;

namespace Yantra.Core
{
    public class GlobalAttribute: Attribute
    {
        public readonly string? Name;

        public GlobalAttribute(string? name = null)
        {
            this.Name = name;
        }

    }

    public static class GlobalAttributeExtensions
    {
        public static void RegisterTypes(this JSContext context, Assembly assembly)
        {
            foreach (var type in assembly.GetTypes())
            {
                var ga = type.GetCustomAttribute<GlobalAttribute>();
                if (ga == null)
                {
                    continue;
                }
                context[ga.Name ?? type.Name] = ClrType.From(type);
            }
        }
    }
}
