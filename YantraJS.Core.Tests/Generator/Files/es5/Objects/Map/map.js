var m = new Map();

m.set("a1", "1");
m.set("b1", "2");
m.set("c1", "3");

assert.strictEqual(m.get("a1"), "1");
assert.strictEqual(m.get("b1"), "2");
assert.strictEqual(m.get("c1"), "3");
assert.strictEqual(m.get("cc"), undefined);

var a = m.keys();
assert.strictEqual(a.next().value, "a1");
assert.strictEqual(a.next().value, "b1");
assert.strictEqual(a.next().value, "c1");
assert.strictEqual(a.next().value, undefined);

a = m.values();
assert.strictEqual(a.next().value, "1");
assert.strictEqual(a.next().value, "2");
assert.strictEqual(a.next().value, "3");
assert.strictEqual(a.next().value, undefined);

a = m.entries();
var [k, v] = a.next().value;
assert.strictEqual(k, "a1");
assert.strictEqual(v, "1");
var [k, v] = a.next().value;
assert.strictEqual(k, "b1");
assert.strictEqual(v, "2");
var [k, v] = a.next().value;
assert.strictEqual(k, "c1");
assert.strictEqual(v, "3");
assert.strictEqual(a.next().value, undefined);


