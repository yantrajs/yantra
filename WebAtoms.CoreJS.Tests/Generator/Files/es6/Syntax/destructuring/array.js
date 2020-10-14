var a = [1, 2, 3, 4];

var [a1, a2, ...all] = a;

assert.strictEqual(1, a1);
assert.strictEqual(2, a2);
assert.strictEqual("3,4", all.toString());