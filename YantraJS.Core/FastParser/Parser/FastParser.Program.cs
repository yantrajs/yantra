using System;
using System.Collections.Generic;
using System.Text;

namespace YantraJS.Core.FastParser
{


    partial class FastParser
    {




        bool Program(out AstProgram program)
        {
            program = default;
            if (Block(out var block))
            {
                program = new AstProgram(block.Start, block.End, block.Statements, this.isAsync);
                program.HoistingScope = block.HoistingScope;
            }
            return true;
        }


    }

}
