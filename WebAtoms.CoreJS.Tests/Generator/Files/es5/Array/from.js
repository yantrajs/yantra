var a = Array.from("abc");
assert.strictEqual(a[0], "a");
assert.strictEqual(a[1], "b");
assert.strictEqual(a[2], "c");

a = Array.from("abc", (x) => x + 1);
assert.strictEqual(a[0], "a1", a[0]);
assert.strictEqual(a[1], "b1");
assert.strictEqual(a[2], "c1");
