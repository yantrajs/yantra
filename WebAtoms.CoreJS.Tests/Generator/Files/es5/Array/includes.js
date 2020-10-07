const a = ["a", "b", "c", 2, 3];

assert.strictEqual(true, a.includes("b"));
assert.strictEqual(true, a.includes(2));
assert.strictEqual(false, a.includes());