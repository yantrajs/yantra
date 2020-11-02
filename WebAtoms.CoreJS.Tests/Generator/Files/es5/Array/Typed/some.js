function isNegative(element, index, array) {
    return element < 0;
}

const int8 = new Int8Array([-10, 20, -30, 40, -50]);
const positives = new Int8Array([10, 20, 30, 40, 50]);

assert.strictEqual(true,int8.some(isNegative));
// expected output: true

assert.strictEqual(false,positives.some(isNegative));
// expected output: false

assert.throws(() => {
    int8.some(true);
});

assert.throws(() => {
    int8.some(1);
});

assert.throws(() => {
    int8.some({});
});

// var val = int8.some(function (value, index, array) { return this == 0 }, 0)
assert.strictEqual(true,
    int8.some(
        function (value, index, array) {
            return this == 0
        }, 0));


assert.strictEqual(false,
    int8.some(
        function (value, index, array) {
            return this == 1
        }, 0));
