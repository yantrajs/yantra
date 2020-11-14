const uint8 = new Int8Array([10, 20, 50, 50, 50, 60]);

assert.strictEqual(4, uint8.lastIndexOf(50, 5));
// expected output: 4

assert.strictEqual(3, uint8.lastIndexOf(50, 3));
// expected output: 3

assert.strictEqual(1, Int8Array.prototype.lastIndexOf.length);

assert.strictEqual(-1, uint8.lastIndexOf(50, -5));

assert.strictEqual(4, uint8.lastIndexOf(50, -2));

assert.strictEqual(4, uint8.lastIndexOf(50, -2));

assert.strictEqual(4, uint8.lastIndexOf(50, 8));

let arr = new Int8Array([3, 2, 1]);
assert.strictEqual(1, arr.lastIndexOf(2, -1));
assert.strictEqual(-1, arr.lastIndexOf(1, undefined));