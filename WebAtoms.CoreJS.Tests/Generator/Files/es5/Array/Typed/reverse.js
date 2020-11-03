var uint8 = new Int8Array([1, 2, 3]);
uint8.reverse();

assert.strictEqual("3,2,1", uint8.toString());
// expected output: Uint8Array [3, 2, 1]


uint8 = new Int8Array([1, 21, 49, 15, 4, 11]);
assert.strictEqual("15,49",
    uint8.subarray(1, 5)
        .subarray(1, 3)
        .reverse()
        .toString());


assert.strictEqual("1,21,15,49,4,11", uint8.toString());