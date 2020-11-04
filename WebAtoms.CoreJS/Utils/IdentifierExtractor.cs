using Esprima.Ast;
using System;
using System.Collections.Generic;
using System.Text;

namespace WebAtoms.CoreJS.Utils
{
    internal static class IdentifierExtractor
    {

        public static List<string> Names(VariableDeclaration vd)
        {
            List<string> list = new List<string>();
            foreach(var d in vd.Declarations)
            {
                Visit(d.Id, list);
            }
            return list;
        }
        static void Visit(Expression d, List<string> list)
        {
            switch(d)
            {
                case Identifier id when id.Name != null:
                    list.Add(id.Name);
                    break;
                case AssignmentPattern asp:
                    Visit(asp.Left, list);
                    break;
                case ObjectPattern op:
                    foreach(Property p in op.Properties)
                    {
                        Visit(p.Value, list);
                    }
                    break;
                case ArrayPattern ap:
                    foreach(var e in ap.Elements)
                    {
                        Visit(e, list);
                    }
                    break;
                case SpreadElement spe:
                    Visit(spe.Argument, list);
                    break;

            }
        }

    }
}
