using System;
using YantraJS.Core.Clr;

namespace YantraJS.Core;

public class ModuleBuilder
{
    private readonly KeyString _moduleName;
    private readonly JSModuleContext _moduleContext;
    private readonly JSObject _exportobj;

    internal ModuleBuilder(KeyString moduleName,JSModuleContext moduleContext)
    {
        _moduleName = moduleName;
        _moduleContext = moduleContext;
        _exportobj = new JSObject();
    }

    public ModuleBuilder ExportType<T>()
    {
        string name = typeof(T).Name;
        var type = ClrType.From(typeof(T));
        _exportobj[name] = type;
        return this;
    }

    public ModuleBuilder ExportValue(in KeyString name, object value)
    {
        _exportobj[name] = value.Marshal();
        return this;
    }

    public ModuleBuilder ExportFunction(string name, JSFunctionDelegate func)
    {
        _exportobj[name] = new JSFunction(func);
        return this;
    }

    internal void Build()
    {
        _moduleContext.RegisterModule(_moduleName, _exportobj);
    }
}