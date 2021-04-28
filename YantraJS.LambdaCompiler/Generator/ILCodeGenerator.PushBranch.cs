using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection.Emit;
using System.Text;
using YantraJS.Core;

namespace YantraJS.Generator
{
    public partial class ILCodeGenerator
    {

        private void Goto(ILWriterLabel label)
        {
            il.Branch(label);
        }
    }
}
