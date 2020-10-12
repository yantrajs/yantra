const a = ["a", "b", "c", 2, 3];
const first = a.findIndex((x) => typeof x === "number");
assert.strictEqual(3, first);
assert.strictEqual(-1, a.findIndex((x) => typeof x === "function"));