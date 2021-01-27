function a(a, ...c) {
    return c.join(",");
}

assert.strictEqual("", a(1));
assert.strictEqual("2", a(1, 2));
assert.strictEqual("2,3", a(1, 2, 3));