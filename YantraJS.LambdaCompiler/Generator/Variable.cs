using System.Reflection.Emit;

namespace YantraJS.Generator
{
    public class Variable
    {
        public readonly LocalBuilder LocalBuilder;
        public readonly bool IsArgument;
        public readonly short Index;
        public readonly bool IsReference;
        public readonly string Name;

        public Variable(LocalBuilder builder , bool isArg, short index, bool isReference, string name)
        {
            this.LocalBuilder = builder;
            this.IsArgument = isArg;
            this.Index = index;
            this.IsReference = isReference;
            this.Name = name;
        }
    }
}
