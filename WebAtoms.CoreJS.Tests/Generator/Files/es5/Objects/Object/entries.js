var a = {
    a: 1,
    b: 2
};

var entries = Object.entries(a);
assert(Array.isArray(entries));
assert(entries.length === 2);
assert(entries[0][0] === "a");
assert(entries[0][1] === 1);
var b = entries[1];
assert(b[0] === "b");
assert(b[1] === 2);
