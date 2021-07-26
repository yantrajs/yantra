function a(b = 2) {
    return b;
}

assert.strictEqual(a(), 2);
assert.strictEqual(a(1), 1);

var b = {};
b[b["A"] = 1] = "A";

assert.strictEqual(b[1], "A");
assert.strictEqual(b["A"], 1);