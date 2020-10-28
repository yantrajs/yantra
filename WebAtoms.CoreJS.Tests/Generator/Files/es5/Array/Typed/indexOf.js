// create a TypedArray with a size in bytes
const typedArray1 = new Int8Array(8);
typedArray1[0] = 32;

const typedArray2 = new Int8Array(typedArray1);
typedArray2[1] = 42;

assert.strictEqual("32,0,0,0,0,0,0,0",typedArray1.toString());
// expected output: Int8Array [32, 0, 0, 0, 0, 0, 0, 0]

assert.strictEqual("32,42,0,0,0,0,0,0", typedArray2.toString());
// expected output: Int8Array [32, 42, 0, 0, 0, 0, 0, 0]

assert.strictEqual(1, Int8Array.prototype.indexOf.length);

let arr = new Int8Array([3, 2, 1]);
assert.strictEqual(2, arr.indexOf(1, undefined));

assert.strictEqual(-1, arr.indexOf(4, 1));

assert.strictEqual(2, arr.indexOf(1, 1));