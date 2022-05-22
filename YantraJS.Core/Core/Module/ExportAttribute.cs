using System;

namespace YantraJS.Core
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ExportAttribute : Attribute
    {
        public string Name { get; }

        /// <summary>
        /// Exports given Type as class
        /// </summary>
        /// <param name="name">Asterix '*' if null</param>
        public ExportAttribute(string name = null)
        {
            this.Name = name;
        }
    }
}
