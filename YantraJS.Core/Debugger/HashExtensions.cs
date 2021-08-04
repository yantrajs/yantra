using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace YantraJS.Core.Debugger
{
    public static class HashExtensions
    {

        public static string ComputeHash(this HashAlgorithm alg, string text) {
            var sb = new StringBuilder();
            foreach(var b in System.Text.Encoding.UTF8.GetBytes(text))
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }
    }
}
