var m = new Map();

m.set("a1", "1");
m.set("b1", "2");
m.set("c1", "3");

assert.strictEqual(m.get("a1"), "1");
assert.strictEqual(m.get("b1"), "2");
assert.strictEqual(m.get("c1"), "3");

var a = m.keys();
assert.strictEqual(a.length, 3);

assert.strictEqual(a[0], "a1");
assert.strictEqual(a[1], "b1");
assert.strictEqual(a[2], "c1");

a = m.values();
assert.strictEqual(a.length, 3);

assert.strictEqual(a[0], "1");
assert.strictEqual(a[1],  "2");
assert.strictEqual(a[2],  "3");

a = m.entries();

assert.strictEqual(a[0][0], "a1");
assert.strictEqual(a[0][1], "1");

assert.strictEqual(a[1][0], "b1");
assert.strictEqual(a[1][1], "2");

assert.strictEqual(a[2][0], "c1");
assert.strictEqual(a[2][1], "3");

