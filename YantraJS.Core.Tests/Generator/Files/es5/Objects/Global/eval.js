assert.strictEqual(5, eval(5));
var e5 = eval("5");
assert.strictEqual(5, e5);
assert.strictEqual((5 + 6), eval("5 + 6"));