const a = ["a", "b", "c", 2, 3];

assert.strictEqual(1, a.indexOf("b"));
assert.strictEqual(3, a.indexOf(2));
assert.strictEqual(-1, a.indexOf());