const a = ["a", "b", "c", 2, 3];
const f = a.filter((x) => typeof x === "number");
assert.strictEqual("2,3", f.join());