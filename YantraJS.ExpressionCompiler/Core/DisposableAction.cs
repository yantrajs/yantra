using System;
using System.Collections.Generic;
using System.Text;

namespace YantraJS.Core
{
    public readonly struct DisposableAction : IDisposable
    {
        public static readonly IDisposable Empty = new EmptyDisposable();
        private readonly Action action;

        public DisposableAction(Action action)
        {
            this.action = action;
        }

        public void Dispose()
        {
            action();
        }

        private readonly struct EmptyDisposable : IDisposable
        {
            public void Dispose()
            {
                
            }
        }
    }
}
