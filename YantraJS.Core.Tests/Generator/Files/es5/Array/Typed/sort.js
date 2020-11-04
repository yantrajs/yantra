var arr = new Int8Array([1, 21, 49, 4, 11]);
assert.strictEqual("1,4,11,21,49", arr.sort().toString());

//arr = new Int8Array([1,NaN,4,7,-5]);
//assert.strictEqual("-5,1,4,7,NaN", arr.sort().toString());


//arr = new Int8Array([-0, 0, -0, 0, -0]);
//assert.strictEqual(arr.sort().map((value) => 1/value).toString());

assert.strictEqual("1,4,11,21,49", arr.sort(((a, b) => a - b)).toString());

assert.strictEqual("49,21,11,4,1", arr.sort(((a, b) => b - a)).toString());

assert.strictEqual("1,11,21,4,49", arr.sort(((a, b) => a.toString() > b.toString() ? 1 : -1)).toString());
