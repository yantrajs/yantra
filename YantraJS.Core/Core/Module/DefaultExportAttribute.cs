using System;

namespace YantraJS.Core
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class DefaultExportAttribute: ExportAttribute
    {
        public DefaultExportAttribute(): base("default")
        {

        }
    }
}
