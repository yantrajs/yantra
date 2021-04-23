#nullable enable
using System;
using System.Collections.Generic;
using System.Text;

namespace YantraJS.Core
{
    public class DisposableList : IDisposable
    {

        private List<IDisposable>? list;

        public void Register(IDisposable d)
        {
            list = list ?? new List<IDisposable>();
            list.Add(d);
        }

        public void Dispose()
        {
            var l = list;
            list = null;
            if (l != null) {
                foreach (var i in l)
                {
                    i.Dispose();
                }
            }
        }
    }
}
