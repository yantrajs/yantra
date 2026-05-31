using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace YantraJS.Core.CodeGen
{
    public class ScriptInfo
    {

        public string FileName;

        public string Code;

        public KeyString[] Indices;

        public object[] Functions;

        public void Init(int capacity)
        {
            Indices = new KeyString[capacity];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Set(int index, string span)
        {
            Indices[index] = KeyStrings.Instance.GetOrCreate(span);
        }


    }
}
