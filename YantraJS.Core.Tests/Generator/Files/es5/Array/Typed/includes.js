let uint8 = new Int8Array([10, 20, 30, 40, 50]);

assert.strictEqual(true, uint8.includes(20));
// expected output: true

// check from position 3
assert.strictEqual(false,uint8.includes(20, 3));
// expected output: false