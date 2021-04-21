namespace YantraJS
{
    public abstract class Box
    {
    }

    public class Box<T> : Box
    {
        public T Value;
    }

}
