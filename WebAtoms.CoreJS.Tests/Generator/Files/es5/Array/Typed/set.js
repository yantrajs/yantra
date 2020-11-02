// create an ArrayBuffer with a size in bytes
const buffer = new ArrayBuffer(8);
const uint8 = new Int8Array(buffer);

// Copy the values into the array starting at index 3
uint8.set([1, 2, 3], 3);

assert.strictEqual("0,0,0,1,2,3,0,0",uint8.toString());
// expected output: Uint8Array [0, 0, 0, 1, 2, 3, 0, 0]