function a(b = 2) {
    return b;
}

assert.strictEqual(a(), 2);
assert.strictEqual(a(1), 1);