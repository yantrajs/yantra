// let uint16 = new Int8Array;
let uint16 = Int8Array.from('12345');
// let output = [1, 2, 3, 4, 5];
assert.strictEqual(1, uint16[0]);
assert.strictEqual(2, uint16[1]);
assert.strictEqual(3, uint16[2]);
assert.strictEqual(4, uint16[3]);
assert.strictEqual(5, uint16[4]);
assert.strictEqual("1,2,3,4,5", uint16.toString());