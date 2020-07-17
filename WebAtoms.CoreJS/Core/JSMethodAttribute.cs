using System;
using System.Collections.Generic;
using System.Text;

namespace WebAtoms.CoreJS.Core
{
    [AttributeUsage(AttributeTargets.Method)]
    public class JSMethodAttribute: Attribute
    {
        public readonly string Name;
        public JSMethodAttribute(string name = null)
        {
            this.Name = name;
        }


    }
}
