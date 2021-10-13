using System.Reflection;

namespace YantraJS
{
    public class Closures
    {
        internal static FieldInfo boxesField = typeof(Closures).GetField("Boxes");
        internal static FieldInfo delegateField = typeof(Closures).GetField("Delegate");
        internal static ConstructorInfo constructor = typeof(Closures).GetConstructors()[0];

        public readonly Box[] Boxes;
        public readonly string IL;
        public readonly string Exp;
        
        public Closures(
            Box[] boxes, 
            string il,
            string exp)
        {
            this.Boxes = boxes;
            this.IL = il;
            this.Exp = exp;
        }
    }
}
