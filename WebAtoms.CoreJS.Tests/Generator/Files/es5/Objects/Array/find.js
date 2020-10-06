const a = ["a", "b", "c", 2, 3];
const first = a.find((x) => typeof x === "number");
assert.strictEqual(2, first);