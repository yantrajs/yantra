const uint8 = new Int8Array([10, 20, 30, 40, 50]);
const keys = uint8.keys();

keys.next();
keys.next();

assert.strictEqual(2, keys.next().value);
// expected output: 2