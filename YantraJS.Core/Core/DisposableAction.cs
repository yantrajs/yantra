﻿using System;

namespace YantraJS.Core
{
    public struct DisposableAction : IDisposable
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