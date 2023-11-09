using System;
using System.Collections.Generic;
using YantraJS.Core;
using YantraJS.Core.Clr;

public class ModuleBuilder
{
    private List<(string name,object value)> exportedObjects = new List<(string name, object value)>();
    private string _moduleName;

    public ModuleBuilder ExportType<T>(string? name = null)
    {
        exportedObjects.Add((name ?? typeof(T).Name, typeof(T)));
        return this;
    }

    public ModuleBuilder(string moduleName)
    {
        _moduleName = moduleName;
    }

    public ModuleBuilder ExportType(Type type, string? name = null)
    {
        exportedObjects.Add((type.Name, type));
        return this;
    }

    public ModuleBuilder ExportValue(string name, object value)
    {
        exportedObjects.Add((name, value.Marshal()));
        return this;
    }

    public ModuleBuilder ExportFunction(string name, JSFunctionDelegate func)
    {
        exportedObjects.Add((name, func));
        return this;
    }
    public void AddModuleToContext(JSModuleContext context)
    {
        JSObject globalExport = new JSObject();
        foreach ((string name, object value)  in exportedObjects)
        {
            switch (value)
            {
                case Type type:
                    globalExport[name] = ClrType.From(type);
                    break;
                case JSFunctionDelegate @delegate:
                    globalExport[name] = new JSFunction(@delegate);
                    break;
                case JSValue jsValue:
                    globalExport[name] = globalExport[name] = jsValue;
                    break;
                default:
                    globalExport[name] = ClrProxy.Marshal(value);
                    break;
            }
        }

        globalExport[KeyStrings.@default] = globalExport;
        context.RegisterModule(_moduleName, globalExport);
    }
}

