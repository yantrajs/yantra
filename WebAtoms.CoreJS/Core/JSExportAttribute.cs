using System;
using System.Collections.Generic;
using System.Text;

namespace WebAtoms.CoreJS.Core
{

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property)]
    public class JSExportAttribute : Attribute
    {
        public readonly string Name;

        public readonly JSPropertyType PropertyType;

        public readonly bool Static;

        public JSExportAttribute(
            string name, 
            JSPropertyType type = JSPropertyType.None,
            bool isStatic = false)
        {
            this.Name = name;
            this.PropertyType = type;
            this.Static = isStatic;
        }


    }

    public enum JSPropertyType
    {
        None = 0,
        Get,
        Set
    }
}
