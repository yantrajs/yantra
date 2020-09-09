var m = new Map();

m.set("a1", "1");
m.set("b1", "2");
m.set("c1", "3");

assert(m.get("a1") === "1");
assert(m.get("b1") === "2");
assert(m.get("c1") === "3");

var a = m.keys();
assert(a.length === 3);

assert(a[0] === "a1");
assert(a[1] === "b1");
assert(a[2] === "c1");

a = m.values();
assert(a.length === 3);

assert(a[0] === "1");
assert(a[1] === "2");
assert(a[2] === "3");

a = m.entries();

assert(a[0][0] === "a1");
assert(a[0][1] === "1");

assert(a[1][0] === "b1");
assert(a[1][1] === "2");

assert(a[2][0] === "c1");
assert(a[2][1] === "3");

