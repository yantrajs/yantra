const uint8 = new Int8Array([10, 20, 30, 40, 50]);
const array1 = uint8.values();

array1.next();
array1.next();

assert.strictEqual(30,array1.next().value);
// expected output: 30


var i = new Int8Array([11, 7]).values();
var result = i.next();
assert.strictEqual(11, result.value);
assert.strictEqual(false, result.done);

var result = i.next();
assert.strictEqual(7, result.value);
assert.strictEqual(false, result.done);

var result = i.next();
assert.strictEqual(undefined, result.value);
assert.strictEqual(true, result.done);

assert.throws(() => {
    i.values.toString();
});