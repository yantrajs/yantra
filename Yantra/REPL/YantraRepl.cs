using System;
using System.Collections.Generic;
using System.Text;
using WebAtoms.CoreJS;

namespace Yantra.REPL
{
    public class YantraRepl: YantraContext
    {

        public YantraRepl(): base(Environment.CurrentDirectory)
        {

        }

        public void Run()
        {
            Console.WriteLine("// Write .exit to stop..");
            while (true)
            {
                string line = Console.ReadLine();
                if (line == ".exit")
                    break;

                var v = CoreScript.Evaluate(line);
                Console.WriteLine(v);
            }

        }



    }
}
