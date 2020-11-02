var x = new Int8Array([1, 2, 3, 4]);
var y = x.slice(0, 2);
assert.strictEqual(1, y[0]);
assert.strictEqual(2, y[1]);
assert.strictEqual(2, y.length);

y = x.slice(1);
assert.strictEqual(2, y[0]);
assert.strictEqual(3, y[1]);
assert.strictEqual(4, y[2]);
assert.strictEqual(3, y.length);

y = x.slice(-2, 10);
assert.strictEqual(3, y[0]);
assert.strictEqual(4, y[1]);
assert.strictEqual(2, y.length);

y = x.slice(0, -2);
assert.strictEqual(1, y[0]);
assert.strictEqual(2, y[1]);
assert.strictEqual(2, y.length);

y = x.slice(2, 3);

y[0] = 5;

assert.strictEqual(1, x[0]);
assert.strictEqual(2, x[1]);
assert.strictEqual(3, x[2]);
assert.strictEqual(4, x[3]);

assert.strictEqual(4, x.length);
