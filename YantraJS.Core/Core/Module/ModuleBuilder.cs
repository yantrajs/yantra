using System;
using System.Collections.Generic;
using YantraJS.Core;
using YantraJS.Core.Clr;

public class ModuleBuilder
{
    private List<(string name,object value)> exportedObjects = new ();
    private string _moduleName;

    public ModuleBuilder ExportType<T>()
    {
        exportedObjects.Add((typeof(T).Name, typeof(T)));
        return this;
    }

    public ModuleBuilder(string moduleName)
    {
        _moduleName = moduleName;
    }

    public ModuleBuilder ExportType(Type type, string name = null)
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
            globalExport[name] = value switch
            {
                Type type => ClrType.From(type),
                JSFunctionDelegate @delegate => new JSFunction(@delegate),
                JSValue jsValue => globalExport[name] = jsValue,
                _ => ClrProxy.Marshal(value)
            };
            
        }
        context.RegisterModule(_moduleName, globalExport);
    }
}

