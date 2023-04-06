var m = new Set();

m.add("a1");
m.add("a2");
m.add("a3");

assert.strictEqual(m.size, 3);

assert.strictEqual(m.has("a1"), true);
assert.strictEqual(m.has("a2"), true);
assert.strictEqual(m.has("a3"), true);
assert.strictEqual(m.has("4"), false);

var a = m.keys();
assert.strictEqual(a.next().value, "a1");
assert.strictEqual(a.next().value, "a2");
assert.strictEqual(a.next().value, "a3");
assert.strictEqual(a.next().value, undefined);

a = m.values();
assert.strictEqual(a.next().value, "a1");
assert.strictEqual(a.next().value, "a2");
assert.strictEqual(a.next().value, "a3");
assert.strictEqual(a.next().value, undefined);


m.delete("a2");
assert.strictEqual(m.size, 2);

assert.strictEqual(m.has("a1"), true);
assert.strictEqual(m.has("a3"), true);

a = m.values();
assert.strictEqual(a.next().value, "a1");
assert.strictEqual(a.next().value, "a3");
assert.strictEqual(a.next().value, undefined);


