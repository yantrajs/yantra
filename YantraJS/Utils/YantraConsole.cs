using System;
using System.Collections.Generic;
using System.Text;
using YantraJS.Core;

namespace YantraJS.Utils
{
    public class YantraConsole
    {

        public static JSValue Log(in Arguments a)
        {
            for (int i = 0; i < a.Length; i++)
            {
                if (a.TryGetAt(i, out var ai))
                {
                    Console.Write(ai);
                    JSContext.CurrentContext.ReportLog(ai);
                }
            }
            Console.WriteLine();
            return JSUndefined.Value;
        }

    }
}
