let uint8 = new Int8Array([0, 0, 0, 0]);
// (value, start position, end position);
uint8.fill(4, 1, 3);

//console.log(uint8);
assert.strictEqual("0,4,4,0", uint8.toString());
// expected output: Uint8Array [0, 4, 4, 0]

uint8 = new Int8Array([1, 2, 3, 4]);
// (value, start position, end position);
uint8.fill(0);
assert.strictEqual("0,0,0,0", uint8.toString());


uint8 = new Int8Array([1, 2, 3, 4]);
// (value, start position, end position);
uint8.fill(0, 1);
assert.strictEqual("1,0,0,0", uint8.toString());

uint8 = new Int8Array([1,2, 3, 4, 5, 6]);
// (value, start position, end position);
uint8.fill('test', 1, 4);
assert.strictEqual("1,0,0,0,5,6", uint8.toString());

uint8 = new Int8Array([1, 2, 3, 4, 5, 6]);
// (value, start position, end position);
uint8.fill(0, -3, -1);
assert.strictEqual("1,2,3,0,0,6", uint8.toString());



