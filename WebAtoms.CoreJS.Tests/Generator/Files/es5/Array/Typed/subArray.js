let uint8 = new Int8Array([10, 20, 30, 40, 50]);

assert.strictEqual("20,30",uint8.subarray(1, 3).toString());
// expected output: Uint8Array [20, 30]

assert.strictEqual("20,30,40,50", uint8.subarray(1).toString());
// expected output: Uint8Array [20, 30, 40, 50]

uint8 = new Int8Array([1, 21, 49, 4, 11]);
assert.strictEqual("4,11", uint8.subarray(3).toString());
assert.strictEqual("49,4", uint8.subarray(2, 4).toString());

assert.strictEqual("4", uint8.subarray(-2, 4).toString());

assert.strictEqual("", uint8.subarray(-2, -4).toString());

assert.strictEqual("21,49", uint8.subarray(-4, -2).toString());

uint8 = new Int8Array([1, 21, 49, 15, 4, 11]);
assert.strictEqual("49,15",
    uint8.subarray(1, 5)
        .subarray(1, 3)
        .toString());


var a1 = new Int8Array([1, 21, 49, 4, 11]);
var a2 = a1.subarray(2);
a2[0] = 99;
assert.strictEqual("1,21,99,4,11", a1.toString());

var a1 = new Int8Array([1, 21, 49, 4, 11]);
var a2 = a1.subarray(2);
a1[3] = 99;
assert.strictEqual("49,99,11", a2.toString());

