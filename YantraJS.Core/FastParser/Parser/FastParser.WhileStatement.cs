using System;
using System.Collections.Generic;
using System.Text;

namespace YantraJS.Core.FastParser
{

    public class AstWhileStatement : AstStatement
    {
        public readonly AstExpression Test;

        public AstWhileStatement(FastToken start, FastToken end, AstExpression test)
            : base(start, FastNodeType.IfStatement, end)
        {
            this.Test = test;
        }
    }

    partial class FastParser
    {



        bool WhileStatement(out AstStatement node)
        {
            throw new NotImplementedException();
        }


    }

}
