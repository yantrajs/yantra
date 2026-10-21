using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace YantraJS.Core;

internal class AsyncCountdownEvent
{
    private readonly object _lock = new();
    private TaskCompletionSource<int> _tcs = new(TaskCreationOptions.RunContinuationsAsynchronously);
    private int _count;

    public AsyncCountdownEvent(int initialCount = 0)
    {
        _count = initialCount;
        if (initialCount == 0)
        {
            _tcs.SetResult(0);
        }
    }

    // This is the property/task you can await in your messaging loop
    public Task Task => _tcs.Task;

    public void AddCount(int count = 1)
    {
        if (count <= 0) return;

        lock (_lock)
        {
            // If we were previously at 0, reset the task completion source for a new batch
            if (_count == 0)
            {
                _tcs = new (TaskCreationOptions.RunContinuationsAsynchronously);
            }
            _count += count;
        }
    }

    public void Signal()
    {
        lock (_lock)
        {
            if (_count <= 0) return;

            _count--;
            if (_count == 0)
            {
                _tcs.TrySetResult(0);
            }
        }
    }
}
