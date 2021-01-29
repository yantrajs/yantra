function a(a, ...c) {
    return c.join(",");
}

assert.strictEqual("", a(1));
assert.strictEqual("2", a(1, 2));
assert.strictEqual("2,3", a(1, 2, 3));

function b(a, b, ...c) {
    return c.join(",");
}

assert.strictEqual("", b(1));
assert.strictEqual("", b(1, 2));
assert.strictEqual("3", b(1, 2, 3));
assert.strictEqual("3,4", b(1, 2, 3, 4));
var cc = [3,4];
assert.strictEqual("3,4", b(1, 2, ... cc));
