// create an ArrayBuffer with a size in bytes
const buffer = new ArrayBuffer(8);
var uint8 = new Int8Array(buffer);

// Copy the values into the array starting at index 3
uint8.set([1, 2, 3], 3);

assert.strictEqual("0,0,0,1,2,3,0,0",uint8.toString());
// expected output: Uint8Array [0, 0, 0, 1, 2, 3, 0, 0]



var uint8 = new Int8Array([1, 21, 49, 15, 4, 11]);
    uint8.subarray(1, 5)
        .subarray(1, 3)
    .set([1, 2]);
assert.strictEqual("1,21,1,2,4,11", uint8.toString());


uint8 = new Int8Array([1, 21, 49, 15, 4, 11, 55, 66, 88, 99, 100]);

uint8.subarray(1, 9)
    .subarray(1, 6)
    .set([0, 2], 2)
    ;
assert.strictEqual("1,21,49,15,0,2,55,66,88,99,100", uint8.toString());