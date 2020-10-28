const uint8 = new Int8Array([10, 20, 30, 40, 50]);

assert.strictEqual("10,20,30,40,50",uint8.join());
// expected output: "10,20,30,40,50"

assert.strictEqual("1020304050",uint8.join(''));
// expected output: "1020304050"

assert.strictEqual("10-20-30-40-50",uint8.join('-'));
// expected output: "10-20-30-40-50"