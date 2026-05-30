using System;
using System.Collections.Generic;
using System.Text;

namespace YantraJS.Core;

internal static class ArrayExtensions
{

    public static string Join(this Array array, string separator = ", ")
    {
        return string.Join(separator, array);
    }

}

