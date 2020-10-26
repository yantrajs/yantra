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