using System;

namespace YantraJS.Core.FastParser
{
    public interface IVariableScope
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="scoped">True if let/const/class</param>
        /// <returns></returns>
        FastVariable CreateVariable(string name, bool scoped);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="alias"></param>
        /// <returns></returns>
        FastVariable GetVariable(string alias);

    }

    public readonly struct FastVariable
    {
        // Name has to be unique within the function scope
        public readonly string Name;

        // Alias is the JavaScript variable name
        public readonly string Alias;
    }

    public class FastVariableReference : FastExpression
    {
        public readonly FastVariable Variable;

        public FastVariableReference(FastNode parent, FastVariable variable) : base(parent, FastNodeType.VariableReference, null)
        {
            this.Variable = variable;
        }


        internal override void Read(FastTokenStream stream)
        {
            throw new NotImplementedException();
        }
    }
}
