using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace YantraJS.Core;

internal class AsyncCountdownEvent
{
    private readonly object _lock = new();
    private TaskCompletionSource<int> _tcs = new(TaskCreationOptions.RunContinuationsAsynchronously);
    private int _count = 0;


    // This is the property/task you can await in your messaging loop
    public Task Task => _tcs.Task;

    public void AddCount()
    {
        Interlocked.Increment(ref this._count);
    }

    public void Signal()
    {
        var n = Interlocked.Decrement(ref this._count);
        if(n == 0)
        {
            _tcs.TrySetResult(0);
        }
    }
}
