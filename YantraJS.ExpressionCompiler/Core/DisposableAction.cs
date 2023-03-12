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

        public static DisposableAction<T> Create<T>(Action<T> action, T value)
        {
            return new DisposableAction<T>(action, value);
        }
    }

    public readonly struct DisposableAction<T> : IDisposable
    {
        private readonly Action<T> action;
        private readonly T value;

        public DisposableAction(Action<T> action, T value)
        {
            this.action = action;
            this.value = value;
        }

        public void Dispose()
        {
            action(value);
        }
    }


}
