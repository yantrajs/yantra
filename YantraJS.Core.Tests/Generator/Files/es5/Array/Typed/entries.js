//let array1 = ['a', 'b', 'c'];

//let iterator1 = array1.entries();

//assert.strictEqual('0,"a"',iterator1.next().value.toString());
//// expected output: Array [0, "a"]

//assert.strictEqual('0,"b"',iterator1.next().value.toString());
//// expected output: Array [1, "b"]

let int8 = new Int8Array([10, 20, 30, 40, 50]);
let eArr = int8.entries();

eArr.next();
eArr.next();

assert.strictEqual("2,30",eArr.next().value.toString());
// expected output: Array [2, 30]

eArr.next();
eArr.next();
eArr.next();
var result = eArr.next();
assert.strictEqual(undefined, eArr.next().value);
assert.strictEqual(true, result.done);
assert.strictEqual("[object Array Iterator]", eArr.toString());

var uint8 = new Int8Array([1, 21, 49, 15, 4, 11]);
var en = uint8.subarray(1, 5)
    .subarray(1, 3).values();

var a = Array.from(en);
assert.strictEqual("49,15", a.toString());



