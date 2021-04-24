using System;

namespace YantraJS.Expressions
{
    public class YLabelTarget
    {
        public readonly string Name;
        public readonly Type LabelType;

        public YLabelTarget(string v, Type type)
        {
            this.Name = v;
            this.LabelType = type;
        }
    }
}