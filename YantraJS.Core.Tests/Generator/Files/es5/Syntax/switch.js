function switchNode(c, n) {
    switch (c) {
        case 0:
            if (n)
                return 'n';
        case 1:
            return 'c';
    }
}

assert.strictEqual('n', switchNode(0, 1));
assert.strictEqual('c', switchNode(0, 0));