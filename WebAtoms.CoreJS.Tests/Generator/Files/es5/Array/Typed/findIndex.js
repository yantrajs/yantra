function isNegative(element, index, array) {
    return element < 0;
}

let int8 = new Int8Array([10, -20, 30, -40, 50]);

assert.strictEqual(1, int8.findIndex(isNegative));
// expected output: 1

int8 = new Int8Array([3, 1, 4, 1, 5, 9]);
function isPresent(value, index, array) { return value > 9; }
assert.strictEqual(-1, int8.findIndex(isPresent));