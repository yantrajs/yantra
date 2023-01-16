class Operation {
    root = 1;
    add(a, b = this.root) {
        return a + b;
    }
}

var a = new Operation();

assert.strictEqual(4, a.add(3));
assert.strictEqual(3, a.add(1,2));
