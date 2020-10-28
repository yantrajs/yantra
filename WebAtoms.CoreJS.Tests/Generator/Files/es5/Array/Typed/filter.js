function isNegative(element, index, array) {
    return element < 0;
}

let int8 = new Int8Array([-10, 20, -30, 40, -50]);
let negInt8 = int8.filter(isNegative);

//console.log(negInt8);
assert.strictEqual("-10,-30,-50", negInt8.toString());
// expected output: Int8Array [-10, -30, -50]

int8 = new Int8Array([1, 2, 3]);
assert.throws(() => {
    int8.filter(true);
});
// assert.strictEqual("TypeError", int8.filter(true));
int8 = new Int8Array([1, 2, 3]);
assert.throws(() => {
    int8.filter(1);
});

int8 = new Int8Array([1, 2, 3]);
assert.throws(() => {
    int8.filter({});
});
