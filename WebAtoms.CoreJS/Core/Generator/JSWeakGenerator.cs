﻿using System;

namespace WebAtoms.CoreJS.Core.Generator
{
    public struct JSWeakGenerator
    {
        internal readonly WeakReference<JSGenerator> generator;

        public JSWeakGenerator(JSGenerator g)
        {
            this.generator = new WeakReference<JSGenerator>(g);
        }

        public JSValue Yield(JSValue value)
        {
            if (!generator.TryGetTarget(out var g))
                throw new ObjectDisposedException("Generator has been disposed");
            return g.Yield(value);
        }

        public JSValue Delegate(JSValue value)
        {
            if (!generator.TryGetTarget(out var g))
                throw new ObjectDisposedException("Generator has been disposed");
            return g.Delegate(value);
        }

    }
}