using Esprima;
using System;
using System.Collections.Generic;
using System.Text;

namespace WebAtoms.CoreJS.Parser
{
    public class CompilerException : Exception
    {
        public CompilerException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
