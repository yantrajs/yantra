function add(a = 1) {
    return a + a;
}

assert.strictEqual(4, add(2));
assert.strictEqual(2, add());

function addAll([a, b, c] = [1, 1, 1]) {
    return a + b + c;
}

assert.strictEqual(6, addAll([1, 2, 3]));
assert.strictEqual(3, addAll());