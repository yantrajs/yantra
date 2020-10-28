function forEachTest(value, index, array) { array[index] = value + 1 }
array = new Int8Array([1, 2, 3]);
array.forEach(forEachTest);
assert.strictEqual("2,3,4", array.toString());

array = new Int8Array([1, 2, 3]);
assert.throws(() => {
    array.forEach(true);
});

array = new Int8Array([1, 2, 3]);
assert.throws(() => {
    array.forEach(1);
});

array = new Int8Array([1, 2, 3]);
assert.throws(() => {
    array.forEach({});
});