const uint8 = new Int8Array([1, 2, 3]);
uint8.reverse();

assert.strictEqual("3,2,1", uint8.toString());
// expected output: Uint8Array [3, 2, 1]
