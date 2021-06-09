using System;
using System.Collections.Generic;
using System.Text;

namespace Test262Runner
{
    static class ConsoleHelper
    {

        public static void WriteLineOnTop(string text)
        {
            var left = Console.CursorLeft;
            int top = Console.CursorTop;

            Console.SetCursorPosition(0, 0);
            Console.SetCursorPosition(0, Console.WindowTop);
            Console.WriteLine(text);
            Console.SetCursorPosition(left, top);
        }

    }
}
