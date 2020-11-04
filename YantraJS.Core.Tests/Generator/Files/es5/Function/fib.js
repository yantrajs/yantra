function fib(n) {
    if (n <= 1)
        return 0;
    if (n === 2)
        return 1;
    return fib(n - 1) + fib(n - 2);
}

assert.strictEqual(fib(5),3);