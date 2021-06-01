using System;
using System.Collections.Generic;
using System.Text;

namespace YantraJS
{
    [AttributeUsage(AttributeTargets.Method)]
    public class LocationAttribute: Attribute
    {
        public readonly string Location;
        public readonly string Name;
        public readonly int Line;
        public readonly int Column;

        public LocationAttribute(
            string location, 
            string name,
            int line, 
            int column)
        {
            this.Location = location;
            this.Name = name;
            this.Line = line;
            this.Column = column;
        }

    }
}
