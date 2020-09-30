const a = [1, 2, 3];

assert.strictEqual(a.unshift(4, 5), 5);
assert.strictEqual("4,5,1,2,3", a.toString());