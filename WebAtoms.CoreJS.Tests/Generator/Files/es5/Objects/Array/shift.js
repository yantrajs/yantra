var a = [, 1, 2];
assert.strictEqual(a.shift(), undefined);
assert.strictEqual(a.toString(), "1,2");

a = [1, , 3];
assert.strictEqual(a.shift(), 1);
assert.strictEqual(a.toString(), ",3");