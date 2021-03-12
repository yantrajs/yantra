using System;

namespace YantraJS.Core
{
    public class CancellableDisposableAction: IDisposable
    {
        private Action action;
        public CancellableDisposableAction(Action action)
        {
            this.action = action;
        }

        public void Cancel()
        {
            action = null;
        }

        public void Dispose()
        {
            action?.Invoke();
        }
    }

    public readonly struct DisposableAction : IDisposable
    {
        readonly Action action;
        public DisposableAction(Action action)
        {
            this.action = action;
        }

        public void Dispose()
        {
            action();
        }
    }
}
