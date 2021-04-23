using System;
using System.Collections.Generic;
using System.Text;
using YantraJS;
using YantraJS.Core;

namespace YantraJS.REPL
{
    internal class YantraRepl: YantraContext
    {

        public YantraRepl(): base(Environment.CurrentDirectory)
        {
            var defModule = new JSModule(new JSObject(), "repl");

            this[KeyStrings.require] = new JSFunction((in Arguments a1) => {
                var r = this.LoadModule(defModule, a1);
                return r;
            });
        }

        public void Run()
        {
            InteractivePrompt.Run((command, listCommand, completions) => {
                if (command == ".exit")
                    return null;
                return CoreScript.Evaluate(command).ToString();
            }, "Yantra:>", "// Write .exit to stop..");
            //while (true)
            //{
            //    string line = Console.ReadLine();
            //    if (line == ".exit")
            //        break;

            //    Console.KeyAvailable && Console.ReadKey()

            //    var v = CoreScript.Evaluate(line);
            //    Console.WriteLine(v);
            //}

        }



    }
}
