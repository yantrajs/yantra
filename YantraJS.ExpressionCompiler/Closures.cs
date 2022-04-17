using System.Reflection;

namespace YantraJS
{
    public class Closures
    {
        internal static FieldInfo repositoryField = typeof(Closures).GetField(nameof(Repository));
        internal static FieldInfo boxesField = typeof(Closures).GetField(nameof(Boxes));
        internal static ConstructorInfo constructor = typeof(Closures).GetConstructors()[0];

        public readonly IMethodRepository Repository;
        public readonly Box[] Boxes;
        public readonly string IL;
        public readonly string Exp;
        
        public Closures(
            IMethodRepository repository,
            Box[] boxes, 
            string il,
            string exp)
        {
            this.Repository = repository;
            this.Boxes = boxes;
            this.IL = il;
            this.Exp = exp;
        }
    }
}
