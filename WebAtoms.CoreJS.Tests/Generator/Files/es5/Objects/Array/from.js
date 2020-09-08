var a = Array.from("abc");
assert(a[0] === "a", a[0]);
assert(a[1] === "b");
assert(a[2] === "c");

a = Array.from("abc", (x) => x + 1);
assert(a[0] === "a1", a[0]);
assert(a[1] === "b1");
assert(a[2] === "c1");
