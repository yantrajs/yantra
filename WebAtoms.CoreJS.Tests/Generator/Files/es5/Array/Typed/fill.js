let uint8 = new Int8Array([0, 0, 0, 0]);
// (value, start position, end position);
uint8.fill(4, 1, 3);

//console.log(uint8);
assert.strictEqual("0,4,4,0", uint8.toString());
// expected output: Uint8Array [0, 4, 4, 0]
