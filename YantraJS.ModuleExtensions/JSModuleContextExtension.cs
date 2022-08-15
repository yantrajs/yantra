using System;
using System.Reflection;
using YantraJS.Core;

namespace YantraJS.ModuleExtensions
{
    public static class JSModuleContextExtension
    {
        /// <summary>
        /// An analogue of the <see cref="RegisterModule"/> with fluent interface of creating module
        /// </summary>
        /// <param name="moduleName">Unique module name</param>
        /// <param name="builder">Action delegate with <see cref="ModuleBuilder"/> object that use for configuring</param>
        public static void CreateModule(this JSModuleContext context, string moduleName, Action<ModuleBuilder> builder)
        {
            var mb = new ModuleBuilder(moduleName);
            builder(mb); 
            mb.AddModuleToContext(context);
        }
        /// <summary>
        /// Return JSValue which is a module in js script (require function for c# code side)
        /// </summary>
        /// <param name="name">Module name</param>
        /// <returns cref="JSValue">Module object</returns>
        /// <exception cref="ArgumentException">If module not found</exception>
        public static JSValue ImportModule(this JSModuleContext context, in KeyString name)
        {
            FieldInfo fi = typeof(JSModuleContext).GetField("moduleCache", BindingFlags.NonPublic | BindingFlags.Instance);
            var moduleCache = (ModuleCache) fi.GetValue(context);
            var n = name.Value;
            return moduleCache.GetOrCreate(n, () => throw new ArgumentException($"module {n} not found"));
        }
    }
}