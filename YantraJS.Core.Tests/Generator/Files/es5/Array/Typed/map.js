const uint8 = new Int8Array([25, 36, 49]);
const roots = uint8.map(Math.sqrt);

assert.strictEqual("5,6,7",roots.toString());
// expected output: Uint8Array [5, 6, 7]