using System;
using System.Collections.Generic;
using YantraJS.Core;
using YantraJS.Core.Clr;

public class ModuleBuilder
{
    private List<Type> exportedTypes = new();
    private List<(string name, JSValue value)> exportedValues = new();
    private List<(string name, JSFunction func)> exportedFunctions = new();
    private string _moduleName;

    public ModuleBuilder ExportType<T>(string name = null)
    {
        exportedTypes.Add(typeof(T));
        return this;
    }

    public ModuleBuilder(string moduleName)
    {
        _moduleName = moduleName;
    }

    public ModuleBuilder ExportType(Type type, string name = null)
    {
        exportedTypes.Add(type);
        return this;
    }

    public ModuleBuilder ExportValue(string name, object value)
    {
        exportedValues.Add((name, value.Marshal()));
        return this;
    }

    public ModuleBuilder ExportFunction(string name, JSFunction func)
    {
        exportedFunctions.Add((name, func));
        return this;
    }

    public void AddModuleToContext(JSModuleContext context)
    {
        JSObject exports = new JSObject();
        foreach (var type in exportedTypes)
        {
            exports[type.Name] = ClrType.From(type);
        }

        foreach (var (name, value) in exportedValues)
        {
            exports[name] = value;
        }

        foreach (var (name, func) in exportedFunctions)
        {
            exports[name] = func;
        }

        context.RegisterModule(_moduleName, exports);
    }
}

